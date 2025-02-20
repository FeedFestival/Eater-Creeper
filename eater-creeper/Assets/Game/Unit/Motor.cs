using DG.Tweening;
using Game.Shared.Interfaces;
using Game.Shared.Utils;
using R3;
using R3.Triggers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using OnlyLayer = Game.Shared.Constants.Layer.LayerConstants;

namespace Game.Unit {
    public class Motor : MonoBehaviour, IMotor {
        [Tooltip("NavMeshAgent.BaseOffset + 0.14")]
        [SerializeField]
        private float _moveSpeedMultiplier = 3.7f;
        [SerializeField]
        private float _maxSprintSpeedMultiplier = 7.2f;
        [SerializeField]
        [Range(0.33f, 0.99f)]
        private float _accelerationTime = 0.66f;
        [SerializeField]
        [Range(0.33f, 0.66f)]
        private float _decelarationTime = 0.44f;
        [SerializeField]
        private float _stopThreshold = .125f;
        [SerializeField]
        private float _sprintStopThreshold = .333f;
        [SerializeField]
        private float _remainingDistSprintStop = 2.25f;
        [SerializeField]
        private float _remainingDistStop = 0.75f;
        [SerializeField]
        private float _agentAcceleration = 1f;
        [SerializeField]
        private bool _agentAutoBreaking = true;

        private Tweener _sprintTweener;
        private Tweener _moveTweener;
        private float _speedMultiplier;
        private NavMeshAgent _navMeshAgent;
        private IDisposable _motorLateUpdateObs;
        private Tween _rotateTween;
        private Vector3? _nextCornerPos;
        private ITrigger _movementTargetTriggerRef;
        private Animator _animatorRef;
        private bool _isSprinting;
        private bool _decelaratingTowardsEnd;

        private IEnumerator _didWeMiss;
        private bool _destinationReached;
        private bool _hasDestination;

        //----------------------------      PUBLIC      ------------------------------
        // All public function defined by interfaces
        #region PUBLIC

        public bool ActiveMotor { get; private set; }
        public Action DestinationReached { get; set; }

        public void Init(Animator animator, bool enableNavMeshAgent = true) {
            _animatorRef = animator;
            if (_animatorRef != null) {
                _animatorRef.applyRootMotion = true;
            }

            _navMeshAgent = GetComponent<NavMeshAgent>();
            if (enableNavMeshAgent == false) {
                _navMeshAgent.enabled = false;
            }

            Sprint(false);
            DestinationReached += destinationReached;
        }

        public void SetActiveMotor() {
            ActiveMotor = true;
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.stoppingDistance = _stopThreshold;
            _navMeshAgent.acceleration = _agentAcceleration;
            _navMeshAgent.autoBraking = _agentAutoBreaking;
        }

        public void StopMotor() {
            var wasActive = ActiveMotor == true;

            if (wasActive) {
                _navMeshAgent.isStopped = true;
                _movementTargetTriggerRef.transform.position = transform.position;
                _navMeshAgent.SetDestination(transform.position);
                _navMeshAgent.ResetPath();
                _nextCornerPos = null;

                _moveTweener?.Kill();
                _sprintTweener?.Kill();
                _rotateTween?.Kill();

                forceDestinationReached();

                _navMeshAgent.acceleration = 100;
                _navMeshAgent.autoBraking = false;

                // we might need to do more here to properly reset
            }
            ActiveMotor = false;
        }

        public void SetMovementTarget(ITrigger trigger) {
            _movementTargetTriggerRef = trigger;
        }

        public void MoveTo(Vector3 pos) {

            ActiveMotor = true;

            lerpMoveSpeed();
            NavmeshUtils.stayOnNavMesh(ref pos);
            moveToTarget(ref pos);
        }

        public void Sprint(bool isSprinting) {

            if (_decelaratingTowardsEnd) { return; }

            if (_moveTweener != null) {
                _moveTweener.Kill();
            }
            if (_sprintTweener != null) {
                _sprintTweener.Kill();
            }
            _navMeshAgent.stoppingDistance = _sprintStopThreshold;

            _isSprinting = isSprinting;
            float to = !_isSprinting ? _moveSpeedMultiplier : _maxSprintSpeedMultiplier;

            _sprintTweener = DOVirtual.Float(
                _speedMultiplier,
                to,
                isSprinting
                    ? _decelarationTime
                    : _accelerationTime,
                changeSpeedMultiplier
            );
        }

        public void Teleport(Vector3 position, bool onNavMesh = false, bool smooth = false) {

            ActiveMotor = true;

            if (onNavMesh) {
                NavMeshHit closestHit;
                if (NavMesh.SamplePosition(position, out closestHit, 500f, NavMesh.AllAreas)) {
                    position = closestHit.position;
                } else {
                    Debug.LogError("Could not find position on NavMesh!");
                }
            }

            var correctedPos = position + new Vector3(0, _navMeshAgent.baseOffset, 0);
            if (smooth) {
                transform.DOMove(correctedPos, 0.66f).SetEase(Ease.OutQuint);
            } else {
                transform.position = correctedPos;
            }
        }

        public void EnableNavMeshAgent(bool enable = true) {
            _navMeshAgent.enabled = enable;
        }
        #endregion

        //----------------------------      LIFECYCLE HOOKS      ----------------------
        //
        private void LateUpdate() {
            
            if (!ActiveMotor) return;

            if (_navMeshAgent.enabled == false) return;

            if (_destinationReached) return;

            checkIfReachedDestination();
        }

        private void OnAnimatorMove() {

            if (!ActiveMotor) return;

            var rootPosition = _animatorRef.rootPosition;
            rootPosition.y = _navMeshAgent.nextPosition.y;
            transform.position = rootPosition;
            _navMeshAgent.nextPosition = rootPosition;

        }

        //-------------------------------------------------     PRIVATE      ------------------------------
        //
        private void moveToTarget(ref Vector3 pos) {

            float distance = Vector3.Distance(pos, transform.position);
            if (distance < 0.7f) { return; }

            if (_navMeshAgent.isStopped) {
                _navMeshAgent.isStopped = false;
            }

            _movementTargetTriggerRef.transform.position = pos;
            _movementTargetTriggerRef.Enable();
            _navMeshAgent.SetDestination(pos);
            _destinationReached = false;
            _hasDestination = true;

            _motorLateUpdateObs?.Dispose();
            _motorLateUpdateObs = this.LateUpdateAsObservable()
                .ThrottleFirst(TimeSpan.FromMilliseconds(100))
                .Do(_ => rotateTowardsNextPointOnNavMesh())
                .Subscribe();
        }

        private void rotateTowardsNextPointOnNavMesh() {

            Vector3? nextCornerPos;

            try {
                nextCornerPos = _navMeshAgent.path.corners[1];
            } catch (Exception) {
#if UNITY_EDITOR
                var s = string.Empty;
                foreach (var item in _navMeshAgent.path.corners) {
                    s += item + "\n";
                }
                //Debug.Log("_navMeshAgent.path.corners: " + s);
#endif
                nextCornerPos = null;
            }

            if (nextCornerPos.HasValue == false || _nextCornerPos == nextCornerPos) return;

            _nextCornerPos = nextCornerPos;

            var lookAt = Quaternion.LookRotation(nextCornerPos.Value - transform.position);

            _rotateTween?.Kill();

            _rotateTween = DOVirtual
            .Float(0, 1, 0.33f, (value) => {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, value);
            });
        }

        private void checkIfReachedDestination() {

            if (!_navMeshAgent.pathPending) {

                /*
                 * If we sprint, we should slow down so we don't miss the goal
                 */
                if (_isSprinting) {
                    if (_navMeshAgent.remainingDistance <= _remainingDistSprintStop) {
                        _decelaratingTowardsEnd = true;
                        Sprint(false);
                    }
                }

                /*
                 * Once we are half a meter away from the destination we should slow down
                 */
                if (_navMeshAgent.remainingDistance <= _remainingDistStop) {
                    /*
                     * - if we are sprinting while we try to decelerate slower then a walk speed
                     * - we should kill it so it doesn't modify `_speedMultiplier`
                     */
                    if (_isSprinting && _sprintTweener != null) {
                        _sprintTweener.Kill();
                    }
                    _decelaratingTowardsEnd = true;
                    lerpMoveSpeed(false, 1f);
                    /*
                    * - it might happen that even with all of the above we still might miss the spot
                    * - in that case we can wait for half a second and if we don't stop, we stop.
                    */
                    _didWeMiss = checkIfWeMissed();
                    StartCoroutine(_didWeMiss);
                }

                if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) {
                    if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f) {
                        DestinationReached?.Invoke();
                        return;
                    }
                }

                var raycastOrigin = transform.position + Vector3.up * 0.5f;
                var ray = new Ray(raycastOrigin, Vector3.down);
                float maxRaycastDistance = 2.0f;

                if (Physics.Raycast(ray, out var hit, maxRaycastDistance, OnlyLayer.MOVEMENT)) {
                    var trigger = hit.transform.GetComponent<ITrigger>();
                    if (trigger != null && _movementTargetTriggerRef.ID == trigger.ID) {
                        DestinationReached?.Invoke();
                    }
                    return;
                }
            }
        }

        private IEnumerator checkIfWeMissed() {
            yield return new WaitForSeconds(0.5f);
            if (_destinationReached == false) {
                DestinationReached?.Invoke();
            }
        }

        private void lerpMoveSpeed(bool increasing = true, float min = 0, bool instant = false) {

            if (_moveTweener != null) {
                _moveTweener.Kill();
            }

            _navMeshAgent.stoppingDistance = _stopThreshold;

            float to = min;
            if (increasing) {
                _speedMultiplier = min;
                to = _moveSpeedMultiplier;
            } else {
                _speedMultiplier = _moveSpeedMultiplier;
                to = min;
            }

            if (instant) {
                changeSpeedMultiplier(to);
                lerpMoveSpeedComplete(increasing);
                return;
            }

            _moveTweener = DOVirtual.Float(
                _speedMultiplier,
                to,
                increasing
                    ? _accelerationTime
                    : _decelarationTime,
                changeSpeedMultiplier
            )
            .OnComplete(() => lerpMoveSpeedComplete(increasing));
        }

        private void lerpMoveSpeedComplete(bool increasing) {
            if (!increasing) {
                _motorLateUpdateObs?.Dispose();
                _navMeshAgent.isStopped = true;
                _decelaratingTowardsEnd = false;
                _hasDestination = false;
            }
        }

        private void changeSpeedMultiplier(float speed) {

            if (!ActiveMotor) return;

            _speedMultiplier = speed;

            if (_hasDestination) {
                _navMeshAgent.speed = _speedMultiplier;
                _animatorRef.SetFloat("locomotion", _navMeshAgent.speed);
                _animatorRef.SetFloat("speedMultiplier", _isSprinting ? 1.2f : 1f);
            }
        }

        private void destinationReached() => forceDestinationReached(false);

        private void forceDestinationReached(bool instant = true) {
            _destinationReached = true;

            if (_didWeMiss != null) {
                StopCoroutine(_didWeMiss);
                _didWeMiss = null;
            }

            lerpMoveSpeed(false, 0, instant);
            _movementTargetTriggerRef.Enable(false);
        }
    }
}
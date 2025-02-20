using DG.Tweening;
//#if UNITY_EDITOR
using Game.Shared.Constants;
using Game.Shared.Core.Store;
//#endif
using Game.Shared.Interfaces;
using Game.Shared.Utils.World;
using R3;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Unit {
    public class PlayMotor : MonoBehaviour, IPlayMotor {

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
        private float _sprintStopThreshold = .333f;

        private Tweener _sprintTweener;
        private float _speedMultiplier;

        private NavMeshAgent _navMeshAgent;
        private Animator _animatorRef;
        private bool _isSprinting;
        private bool _decelaratingTowardsEnd;

        [Header("Play")]
        [Tooltip("Acceleration and deceleration")]
        [SerializeField]
        private float _deltaTimeMultiplier = 100;
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        [SerializeField]
        private float _rotationSmoothTime = 0.12f;
        [SerializeField]
        private float _inputStopThreshold = .025f;

        private float _targetRotation = 0.0f;
        private float _rotationVelocity;

        private UnitState _unitState;

        private Vector2 _lerpedInputDirection;
        private Tweener _moveTweener;

        private Vector3 _3DInputDirection;
        private Vector3 _lerped3DInputDirection;

        private PlayerSensors _playerSensors;

        //----------------------------      PUBLIC      ------------------------------
        // All public function defined by interfaces

        public bool AnalogControl { get; set; }

        public bool ActiveMotor { get; private set; }

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

            Store.Select<UnitState>(Selector.UnitState)
                .DistinctUntilChanged()
                .Do(onUnitStateChange)
                .Subscribe();

            _playerSensors = new PlayerSensors();
        }

        public void SetActiveMotor() {
            ActiveMotor = true;

            _navMeshAgent.updatePosition = true;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.stoppingDistance = _inputStopThreshold;
        }

        public void StopMotor() {
            var wasActive = ActiveMotor == true;
            ActiveMotor = false;

            if (!wasActive) return;


        }

        public void EnableNavMeshAgent(bool enable = true) {
            _navMeshAgent.enabled = enable;
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

        public void Move(Vector2 move, bool cancel = false) {

            SetActiveMotor();

            var inputDirection = new Vector2((float)Math.Round(move.x, 2), (float)Math.Round(move.y, 2));
            var camRotation = getCameraRot();

            if (!cancel) {
                // Rotate the input direction to be relative to the camera
                _3DInputDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
                _3DInputDirection.Normalize();
            }

            moveLerp(inputDirection, cancel);
        }

        //----------------------------      LIFECYCLE HOOKS      ----------------------
        //

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Update() {

            if (!ActiveMotor) return;

            if (_unitState == UnitState.Hidden || _unitState == UnitState.Interacting) return;

            if (_navMeshAgent.enabled == false) return;

            if (_lerpedInputDirection.sqrMagnitude < _inputStopThreshold) {
                return;
            }

            moveOnNavmesh();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void FixedUpdate() {

            if (!ActiveMotor) return;

            if (_unitState == UnitState.Hidden || _unitState == UnitState.Interacting) return;

            if (_navMeshAgent.enabled == false) return;

            _playerSensors
                .checkWhatsInFront(_navMeshAgent, transform, _lerped3DInputDirection);

            if (_lerpedInputDirection.sqrMagnitude < _inputStopThreshold) return;

            calculateSpeed();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void LateUpdate() {

            if (!ActiveMotor) return;

            if (_unitState == UnitState.Hidden || _unitState == UnitState.Interacting) return;

            if (_navMeshAgent.enabled == false) return;

            if (_lerpedInputDirection.sqrMagnitude < _inputStopThreshold) return;

            var camRotation = getCameraRot();
            _lerped3DInputDirection = camRotation * new Vector3(_lerpedInputDirection.x, 0, _lerpedInputDirection.y);

            Debug.DrawRay(transform.position, camRotation * _3DInputDirection, Color.cyan);
            Debug.DrawRay(transform.position, _lerped3DInputDirection, Color.red);

            rotateByInputDirection();
        }

        /* Info
         * We need to keep this here, in order for the `_animatorRef.deltaPosition` to work properly
         */
        private void OnAnimatorMove() { }

        //------------------------------    PRIVATE      ------------------------------
        //

        private void moveLerp(Vector2 moveSimple, bool cancel = false) {

            if (_moveTweener != null) {
                _moveTweener.Kill();
            }

            _moveTweener = DOVirtual.Vector3(
                _lerpedInputDirection,
                moveSimple,
                cancel
                    ? _decelarationTime
                    : _accelerationTime,
                (Vector3 value) => {
                    _lerpedInputDirection = value;
                });

            _moveTweener.onComplete = () => {
                if (cancel) {
                    var locomotionValue = _animatorRef.GetFloat("locomotion");
                    if (locomotionValue > 0.05f) {
                        _moveTweener = DOVirtual.Float(
                            locomotionValue,
                            0,
                            _decelarationTime,
                            (float value) => _animatorRef.SetFloat("locomotion", value)
                        );
                    }
                }
            };
        }

        private void moveOnNavmesh() {
            if (_lerped3DInputDirection == Vector3.zero) { return; }

            var velocity = new Vector2(_lerped3DInputDirection.x, _lerped3DInputDirection.z) * _navMeshAgent.speed;

            _animatorRef.SetFloat("locomotion", velocity.magnitude);
            _animatorRef.SetFloat("speedMultiplier", _isSprinting ? 1.2f : 1f);

            _navMeshAgent.Move(_animatorRef.deltaPosition);
        }

        private void rotateByInputDirection() {
            var camRotation = getCameraRot();
            var inputDirection = camRotation * _3DInputDirection;
            float rad = Mathf.Atan2(
                inputDirection.x + _playerSensors.CollideAndSlideDir.x,
                inputDirection.z + _playerSensors.CollideAndSlideDir.z
            );
            _targetRotation = rad * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                _targetRotation,
                ref _rotationVelocity,
                _rotationSmoothTime
            );

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        private void calculateSpeed() {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _speedMultiplier;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_lerpedInputDirection.sqrMagnitude < _inputStopThreshold) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_navMeshAgent.velocity.x, 0.0f, _navMeshAgent.velocity.z).magnitude;

            float speed = targetSpeed;
            float speedOffset = 0.1f;
            float inputMagnitude = 1;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset) {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * _deltaTimeMultiplier);

                // round speed to 3 decimal places
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            _navMeshAgent.speed = speed * _playerSensors.SpeedSlowAmmount;
        }

        private void changeSpeedMultiplier(float speed) {
            _speedMultiplier = speed;
        }

        private void onUnitStateChange(UnitState unitState) {
            _unitState = unitState;
            Debug.Log("_unitState: " + _unitState);

            switch (unitState) {
                case UnitState.Cautios:
                case UnitState.Crouching:
                case UnitState.Relaxed:
                case UnitState.Sprinting:

                    _navMeshAgent.radius = 0.5f;
                    _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                    _navMeshAgent.enabled = true;

                    break;
                case UnitState.Interacting:

                    //_navMeshAgent.radius = 0_stopThreshold;
                    _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                    _navMeshAgent.enabled = false;

                    break;
                case UnitState.Hidden:
                default:

                    //_navMeshAgent.radius = 0_stopThreshold;
                    _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                    _navMeshAgent.enabled = false;

                    break;
            }
        }

        private Quaternion getCameraRot() {
            // Get the camera's forward vector (ignoring Y axis)
            var cameraForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
            // Create a rotation that faces the camera's forward direction
            var cameraRotation = Quaternion.LookRotation(cameraForward);
            return cameraRotation;
        }
    }
}
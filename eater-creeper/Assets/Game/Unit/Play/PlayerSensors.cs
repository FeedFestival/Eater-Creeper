using DG.Tweening;
using Game.Shared.Utils.World;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using OnlyLayer = Game.Shared.Constants.Layer.LayerConstants;

namespace Game.Unit {
    public class ObstacleCheck {
        public string name;
        public bool hasObstacle;
        public Vector2 dir;
    }

    public class PlayerSensors {

        private enum FRONT_CHECK {
            LEFT_45 = -3,
            LEFT_30 = -2,
            LEFT_15 = -1,
            FRONT = 0,
            RIGHT_15 = 1,
            RIGHT_30 = 2,
            RIGHT_45 = 3,
        };
        private readonly int[] _checks = new int[7] { 0, -1, 1, -2, 2, -3, 3 };
        private Dictionary<int, int> _checkDegrees = new Dictionary<int, int> {
            { 0, 0 },
            { -1, -15 },
            { 1, 15 },
            { -2, -30 },
            { 2, 30 },
            { -3, -45 },
            { 3, 45 },
        };
        private Dictionary<int, ObstacleCheck> _obstacleCheck = new Dictionary<int, ObstacleCheck> {
            { 0, new ObstacleCheck() },
            { -1, new ObstacleCheck() },
            { 1, new ObstacleCheck() },
            { -2, new ObstacleCheck() },
            { 2, new ObstacleCheck() },
            { -3, new ObstacleCheck() },
            { 3, new ObstacleCheck() },
        };

        private Tweener _lerpTweener;
        private Vector3 _collideAndSlideDir;
        private float _obstacleSlowAmmount = 1f;

        private bool _isObstacleToTheLeft;

        internal Vector3 CollideAndSlideDir;
        internal float SpeedSlowAmmount = 1f;

        internal void checkWhatsInFront(NavMeshAgent navMeshAgent, Transform transform, Vector3 isoInputDirection) {
            var distance = navMeshAgent.radius;
            var origin = new Vector3(transform.position.x, 0.4f, transform.position.z);

            foreach (var check in _checks) {
                var rDir = Quaternion.Euler(0, _checkDegrees[check], 0) * transform.forward;
                _obstacleCheck[check].dir = WorldUtils.ToVector2(rDir);
                _obstacleCheck[check].hasObstacle = checkWithRay(rDir, origin, distance);
            }

            var hasObstacleInFront = checkHasObstacleInFront(isoInputDirection);

            if (!hasObstacleInFront) {
                lerpCollideAndSlide(Vector3.zero, 1f);
                return;
            }

            RaycastHit hitInfo;
            //var minThreshold = 0.15f;
            var minThreshold = 0.1f;
            var p1 = transform.position + new Vector3(0, 0.4f, 0);

            // Cast a sphere wrapping character controller 10 meters forward
            // to see if it is about to hit anything.
            if (Physics.SphereCast(p1, distance / 3, isoInputDirection, out hitInfo, distance / 2, OnlyLayer.OBSTACLE)) {
            //if (Physics.Raycast(origin, isoInputDirection, out hitInfo, distance, OnlyLayer.OBSTACLE)) {

                // Get the normal of the wall (from hitInfo)
                var wallNormal = hitInfo.normal;

                // Calculate the slide direction by finding the perpendicular direction
                var slideDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;

                // Determine whether to slide left or right
                float slideDirectionDot = Vector3.Dot(isoInputDirection, slideDirection);

                // Slide to the right (relative to the character)
                var adjustedDir = slideDirection;

                if (slideDirectionDot < -minThreshold) {
                    // Slide to the left (relative to the character)
                    adjustedDir = -slideDirection;
                } else if (slideDirectionDot < minThreshold) {
                    // Depending on current orientation
                    // - this is so we don't rotate in place without moving
                    if (_isObstacleToTheLeft == false) {
                        adjustedDir = -slideDirection;
                    }
                }

                // Draw a line in the direction of the slideDirection for 3 meters
                Debug.DrawLine(origin, origin + adjustedDir * 2f, Color.green);

                var moveSpeedSlow = Mathf.Abs(slideDirectionDot);

                lerpCollideAndSlide(
                    adjustedDir - isoInputDirection,
                    moveSpeedSlow > minThreshold ? moveSpeedSlow : minThreshold
                );
                return;
            }

            lerpCollideAndSlide(Vector3.zero, 1f);
        }

        internal bool checkHasObstacleInFront(Vector3 isometricDirection) {
            bool hasObstacleInFront = false;
            for (var i = 0; i < 3; i++) {
                int r_i = i + 1;
                int op_i = i * -1;
                var r_isBlocked = _obstacleCheck[i].hasObstacle || _obstacleCheck[r_i].hasObstacle;
                int l_i = (i * -1) - 1;
                var l_isBlocked = _obstacleCheck[op_i].hasObstacle || _obstacleCheck[l_i].hasObstacle;
                hasObstacleInFront = r_isBlocked || l_isBlocked;
                if (hasObstacleInFront) {
                    _isObstacleToTheLeft = l_isBlocked;
                    break;
                }
            }

            return hasObstacleInFront;
        }

        //internal bool checkHasObstacleInFront(Vector3 isometricDirection, ref int blockedIndex) {
        //    bool hasObstacleInFront = false;
        //    for (var i = 0; i < 3; i++) {
        //        int r_i = i + 1;
        //        int op_i = i * -1;
        //        var r_isBlocked = false;
        //        var r_isBetween = WorldUtils.IsDirectionBetween(isometricDirection, _obstacleCheck[i].dir, _obstacleCheck[r_i].dir);
        //        if (r_isBetween) {
        //            r_isBlocked = _obstacleCheck[i].hasObstacle || _obstacleCheck[r_i].hasObstacle;
        //            if (r_isBlocked) {
        //                //blockedIndex = _obstacleCheck[i].hasObstacle ? i : r_i;
        //                //Debug.Log("<b>R</b> [" + i + "].(" + _obstacleCheck[i].hasObstacle + "): "
        //                //    + _obstacleCheck[i].dir + " - [" + r_i + "] " + _obstacleCheck[r_i].dir + ".("
        //                //    + _obstacleCheck[r_i].hasObstacle + ")");
        //            }
        //        }
        //        int l_i = (i * -1) - 1;
        //        var l_isBlocked = false;
        //        var isBetweenL = WorldUtils.IsDirectionBetween(isometricDirection, _obstacleCheck[op_i].dir, _obstacleCheck[l_i].dir);
        //        if (isBetweenL) {
        //            l_isBlocked = _obstacleCheck[op_i].hasObstacle || _obstacleCheck[l_i].hasObstacle;
        //            if (l_isBlocked) {
        //                //blockedIndex = _obstacleCheck[op_i].hasObstacle ? op_i : l_i;
        //                //Debug.Log("<b>L</b> [" + op_i + "].(" + _obstacleCheck[op_i].hasObstacle + "): "
        //                //    + _obstacleCheck[op_i].dir + " - [" + l_i + "] " + _obstacleCheck[l_i].dir + ".("
        //                //    + _obstacleCheck[l_i].hasObstacle + ")");
        //            }
        //        }
        //        hasObstacleInFront = r_isBlocked || l_isBlocked;
        //        if (hasObstacleInFront) break;
        //    }
        //    return hasObstacleInFront;
        //}

        private bool checkWithRay(Vector3 rayDirection, Vector3 origin, float distance) {
            RaycastHit hitInfo;
            if (Physics.Raycast(origin, rayDirection, out hitInfo, distance, OnlyLayer.OBSTACLE)) {
                return true;
            }

            return false;
        }

        private void lerpCollideAndSlide(Vector3 dir, float obstacleSlow) {

            if (_collideAndSlideDir == dir) return;

            _collideAndSlideDir = dir;
            _obstacleSlowAmmount = obstacleSlow;

            if (_lerpTweener != null) {
                _lerpTweener.Kill();
            }

            _lerpTweener = DOVirtual
                .Float(0, 1, 0.3f,
                t => {
                    CollideAndSlideDir = Vector3.Lerp(CollideAndSlideDir, _collideAndSlideDir, t);
                    SpeedSlowAmmount = Mathf.Lerp(SpeedSlowAmmount, _obstacleSlowAmmount, t);
                });

        }
    }
}
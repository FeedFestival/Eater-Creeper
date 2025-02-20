using Game.Shared.Constants.Store;
using Game.Shared.Core.Store;
using R3;
using System;
using Unity.Cinemachine;
using UnityEngine;


namespace Game.Player {
    public class CameraTarget : MonoBehaviour, ICameraTarget {
        [SerializeField]
        private CinemachineCamera _cinemachineCamera;
        [SerializeField]
        private Transform _cameraDesiredPosT;
        [SerializeField]
        private Transform _followTarget;
        [SerializeField]
        private Transform _poleXRot;
        [SerializeField]
        private float _rotationSpeed = 5.0f;
        [SerializeField]
        private int _minVertical = 33;

        [Header("Zoom Settings")]
        [SerializeField]
        private int _currentZoom = 14;
        [SerializeField]
        private int _maxZoom = 24;
        [SerializeField]
        private int _minZoom = 12;
        [SerializeField]
        private int _changeZoom = 4;

        [Header("Move Camera Target")]
        [SerializeField]
        private float _speedMultiplier = 4;
        [SerializeField]
        public float _edgeThresholdPercent = 15.0f;

        private float edgeThresholdWidth;
        private float edgeThresholdHeight;

        private bool _doFollowTarget = true;
        private Vector3? _moveDir;

        private CinemachineFollow _transposer;
        private CinemachineRotationComposer _composer;
        private IDisposable _gamestateSelector;

        public Transform Transform => transform;

        public void Move(Vector2 pos, bool cancel) {

            if (cancel) {
                _moveDir = null;
                return;
            }

            _moveDir = getCameraOrientedPos(pos);
        }

        //---------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void Start() {
            _transposer = _cinemachineCamera.GetComponent<CinemachineFollow>();
            _composer = _cinemachineCamera.GetComponent<CinemachineRotationComposer>();

            _composer.TargetOffset = Vector3.zero;
            //_composer.m_TrackedObjectOffset = Vector3.zero;

            _composer.Lookahead.Enabled = false;
            //_composer.m_LookaheadTime = 0;
            //_composer.m_LookaheadSmoothing = 0;

            _composer.Damping = Vector3.zero;
            //_composer.m_HorizontalDamping = 0;
            //_composer.m_VerticalDamping = 0;


            _transposer.FollowOffset = _cameraDesiredPosT.position - transform.position;
            //_transposer.m_FollowOffset = _cameraDesiredPosT.position - transform.position;

            edgeThresholdWidth = Screen.width * (_edgeThresholdPercent / 100);
            edgeThresholdHeight = Screen.height * (_edgeThresholdPercent / 100);

            _gamestateSelector = Store.Select<GameplayState>(Selector.GameplayState)
                    .DistinctUntilChanged()
                    .Do(onGameStateChange)
                    .Subscribe();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        void Update() {

            if (_doFollowTarget) {
                transform.position = _followTarget.position; //+ (_followTarget.forward * 0.3f);
            }
            if (Input.GetMouseButton(1)) {
                changeVirtualCameraRotation();
            }

            var mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if (mouseWheel > 0) {
                zoom();
            } else if (mouseWheel < 0) {
                zoom(false);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void FixedUpdate() {

            if (_doFollowTarget) return;

            if (_moveDir.HasValue) {
                moveCameraTarget(_moveDir.Value);
            }

            var moveCameraByEdge = calculateCameraMovement();
            if (moveCameraByEdge.HasValue) {
                moveCameraTarget(moveCameraByEdge.Value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void OnDestroy() {
            _gamestateSelector.Dispose();
        }

        void onGameStateChange(GameplayState gameplayState) {
            _doFollowTarget = gameplayState != GameplayState.StrategicExploration;
        }

        private void moveCameraTarget(Vector3 moveDir) {
            var zoomMultiplier = ((_currentZoom - _minZoom) / _changeZoom) + 1;
            transform.position += moveDir * _speedMultiplier * zoomMultiplier * Time.deltaTime;
        }

        private Vector3? calculateCameraMovement() {
            Vector2 movement = Vector3.zero;

            // Get mouse position in screen space
            Vector2 mousePosition = Input.mousePosition;
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // Scaling factor based on how close the mouse is to the edge
            float scaleX = 0f;
            float scaleY = 0f;

            // Check horizontal edges
            if (mousePosition.x < edgeThresholdWidth) { // Near the left edge
                scaleX = 1 - (mousePosition.x / edgeThresholdWidth);
                movement.x = -1;
            } else if (mousePosition.x > screenWidth - edgeThresholdWidth) { // Near the right edge
                scaleX = 1 - ((screenWidth - mousePosition.x) / edgeThresholdWidth);
                movement.x = 1;
            }

            // Check vertical edges
            if (mousePosition.y < edgeThresholdHeight) { // Near the bottom edge
                scaleY = 1 - (mousePosition.y / edgeThresholdHeight);
                movement.y = -1;
            } else if (mousePosition.y > screenHeight - edgeThresholdHeight) { // Near the top edge
                scaleY = 1 - ((screenHeight - mousePosition.y) / edgeThresholdHeight);
                movement.y = 1;
            }

            float scale = Mathf.Max(scaleX, scaleY);
            scale = Mathf.Clamp(scale, 0.1f, 1.0f);

            if (movement == Vector2.zero) {
                return null;
            }
            return getCameraOrientedPos(movement.normalized * scale);
        }


        private void zoom(bool zoomIn = true) {
            _currentZoom += zoomIn ? -_changeZoom : _changeZoom;

            if (_currentZoom > _maxZoom) _currentZoom = _maxZoom;
            if (_currentZoom < _minZoom) _currentZoom = _minZoom;

            _cameraDesiredPosT.localPosition = new Vector3(0, 0, -_currentZoom);

            _transposer.FollowOffset = _cameraDesiredPosT.position - transform.position;
            //_transposer.m_FollowOffset = _cameraDesiredPosT.position - transform.position;
        }

        private void changeVirtualCameraRotation() {

            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");

            var newEulerAngles = _poleXRot.localEulerAngles + (Quaternion.Euler(-mouseY, mouseX, 0).eulerAngles * _rotationSpeed);

            if (newEulerAngles.x < _minVertical) {
                newEulerAngles = new Vector3(_minVertical + 1, newEulerAngles.y, 0);
            }
            //if (newEulerAngles.x > _maxVertical) {
            //    newEulerAngles = new Vector3(_maxVertical - 1, newEulerAngles.y, 0);
            //}

            _poleXRot.localEulerAngles = newEulerAngles;

            _transposer.FollowOffset = _cameraDesiredPosT.position - transform.position;
            //_transposer.m_FollowOffset = _cameraDesiredPosT.position - transform.position;
        }

        private Quaternion getCameraRot() {
            // Get the camera's forward vector (ignoring Y axis)
            var cameraForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
            // Create a rotation that faces the camera's forward direction
            var cameraRotation = Quaternion.LookRotation(cameraForward);
            return cameraRotation;
        }

        private Vector3 getCameraOrientedPos(Vector2 pos) {
            var camRotation = getCameraRot();
            return camRotation * new Vector3(pos.x, 0, pos.y);
        }
    }
}
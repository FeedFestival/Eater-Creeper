using Game.Shared.Bus.InputEvents;
using Game.Shared.Bus;
using Game.Shared.Constants.Layer;
using Game.Shared.Core.Store;
using Game.Shared.Interfaces;
using UnityEngine;
using Game.Player.UI;
using Unity.Cinemachine;

namespace Game.Player {
    public class CameraController : MonoBehaviour, ICameraController {
        [Header("Refferences")]
        [SerializeField]
        private CinemachineCamera _cinemachineCamera;
        [SerializeField]
        private Camera _camera;

        [Header("Mouse Ray Check")]
        [SerializeField]
        private float _checkRange = 5f;
        private int _lastFocusedTriggerId = -1;

        private UIManager _uIManager;

        internal void Init(ICameraTarget cameraTarget, UIManager uIManager) {

            _uIManager = uIManager;

            _cinemachineCamera.Target.TrackingTarget = cameraTarget.Transform;
            //_cinemachineCamera.Follow = cameraTarget.Transform;
            //_cinemachineCamera.LookAt = cameraTarget.Transform;

            __.InputBus.On(InputEvt.MOUSE_POSITION, mousePositionChanged);
        }

        internal void CheckWhatsAtTheMousePosition(Vector2 mousePosition) {
            var ray = _camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out var hit, _checkRange, LayerConstants.INTERACT)) {
                onHit(ray, hit);
            } else {
                onMiss(ray);
            }
        }

        //--------------------------------------    LIFECYCLE HOOKS    ----------------------------
        //

        private void OnDestroy() {
            __.InputBus.UnregisterByEvent(InputEvt.MOUSE_POSITION, mousePositionChanged);
        }

        //--------------------------------------    PRIVATE    ------------------------------------
        //

        private void mousePositionChanged(object obj) {
            var mousePosition = obj as Vector2?;
            if (mousePosition.HasValue) {

                _uIManager.HUD.ChangeCursorPosition(mousePosition.Value);

                CheckWhatsAtTheMousePosition(mousePosition.Value);
            }
        }

        private void onHit(Ray ray, RaycastHit hit) {
            var focusTrigger = hit.transform.GetComponent<IFocusTrigger>();
            if (focusTrigger != null) {
                if (_lastFocusedTriggerId == -1 || _lastFocusedTriggerId != focusTrigger.ID) {
                    _lastFocusedTriggerId = focusTrigger.ID;
                    Store.Dispatch(StoreAction.SetFoccusedInteractable, _lastFocusedTriggerId);
                }
            }
        }

        private void onMiss(Ray ray) {
            if (_lastFocusedTriggerId != -1) {
                _lastFocusedTriggerId = -1;
                Store.Dispatch(StoreAction.SetFoccusedInteractable, -1);
            }
        }
    }
}
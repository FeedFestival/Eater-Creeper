#if UNITY_EDITOR
using Game.Shared.Bus;
using Game.Shared.Bus.GameEvents;
using UnityEngine.UI;
#endif
using UnityEngine;

namespace Game.Player.Debug {
    public class DebugView : MonoBehaviour {
#if UNITY_EDITOR

        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private Image _arrowImage;

        [Header("Colors")]
        [SerializeField]
        private Color _validColor;
        [SerializeField]
        private Color _warningColor;
        [SerializeField]
        private Color _invalidColor;

        void Awake() {

            __.GameBus.On(GameEvt.SHOW_MOVE_DIR_ARROW, showMoveDirArrow);
        }

        private void showMoveDirArrow(object payloadObj) {
            var payload = ((bool canWalk, Vector3 worldPos))payloadObj;

            var screenPos = getScreenPos(payload.worldPos);

            _arrowImage.rectTransform.localPosition = screenPos;
            if (payload.canWalk) {
                _arrowImage.color = _validColor;
            } else {
                _arrowImage.color = _invalidColor;
            }
        }

        void Update() {

        }

        void OnDestroy() {
            __.GameBus.UnregisterByEvent(GameEvt.SHOW_MOVE_DIR_ARROW, showMoveDirArrow);

        }

        private Vector2 getScreenPos(Vector3 worldPos) {
            // Convert world position to screen space
            var screenPosition = Camera.main.WorldToScreenPoint(worldPos);
            Vector2 screenPoint;

            // Convert screen space position to Canvas local position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                screenPosition,
                _canvas.worldCamera,
                out screenPoint);

            return screenPoint;
            // Set the UI Image's RectTransform position to the local point
            //uiImage.rectTransform.localPosition = localPoint;
        }

#else
        private void OnEnable() {
            Destroy(gameObject);
        }
#endif
    }
}
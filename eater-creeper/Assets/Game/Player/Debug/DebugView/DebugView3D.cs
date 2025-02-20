#if UNITY_EDITOR
using Game.Shared.Bus;
using Game.Shared.Bus.GameEvents;
using Game.Shared.Utils.World;
#endif
using UnityEngine;

namespace Game.Player.Debug {
    public class DebugView3D : MonoBehaviour {
#if UNITY_EDITOR

        [SerializeField]
        private Transform _dirIndicator;
        private MeshRenderer _meshrndDirIndicator;

        [SerializeField]
        private Transform _closesPointIndctor;
        [SerializeField]
        private Transform _dirClosesPointIndctor;

        [Header("Colors")]
        [SerializeField]
        private Material _validMat;
        [SerializeField]
        private Material _warningMat;
        [SerializeField]
        private Material _invalidMat;

        void Awake() {

            _meshrndDirIndicator = _dirIndicator.GetComponent<MeshRenderer>();

            __.GameBus.On(GameEvt.SHOW_MOVE_DIR_ARROW, showMoveDirArrow);
            __.GameBus.On(GameEvt.CLOSEST_POINT_INDICATOR, showClosestPoint);
        }

        private void showMoveDirArrow(object payloadObj) {
            var payload = ((bool canWalk, Vector3 worldPos))payloadObj;

            _dirIndicator.position = payload.worldPos;
            if (payload.canWalk) {
                _meshrndDirIndicator.material = _validMat;
            } else {
                _meshrndDirIndicator.material = _invalidMat;
            }
        }

        private void showClosestPoint(object payloadObj) {
            var payload = ((float y, Vector2 closestPoint, Vector2 dirClosestPoint))payloadObj;

            _dirClosesPointIndctor.position = WorldUtils.ToVector3(payload.dirClosestPoint, payload.y);
            _closesPointIndctor.position = WorldUtils.ToVector3(payload.closestPoint, payload.y);
        }

        void Update() {

        }

        void OnDestroy() {
            __.GameBus.UnregisterByEvent(GameEvt.SHOW_MOVE_DIR_ARROW, showMoveDirArrow);
            __.GameBus.UnregisterByEvent(GameEvt.CLOSEST_POINT_INDICATOR, showMoveDirArrow);

        }

#else
        private void OnEnable() {
            Destroy(gameObject);
        }
#endif
    }
}
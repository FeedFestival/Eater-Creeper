using Game.Shared.Bus;
using Game.Shared.Bus.GameEvents;
using Game.Shared.Core.Store;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Scene {
    public class GameScene : MonoBehaviour, IGameScene {
        [SerializeField]
        private GameObject _unitManagerGo;
        [SerializeField]
        private GameObject _interactableManagerGo;
        [SerializeField]
        private GameObject _zoneManagerGo;
        //[SerializeField]
        //private GameObject _soundManagerGo;

        protected IUnitManager _unitManager;
        protected IInteractManager _interactManager;
        //protected IZoneManager _zoneManager;
        //protected ISoundManager _soundManager;
        protected IPlayer _player;

        private void Awake() {
            if (_unitManagerGo != null) {
                _unitManager = _unitManagerGo?.GetComponent<IUnitManager>();
            }
            if (_interactableManagerGo != null) {
                _interactManager = _interactableManagerGo?.GetComponent<IInteractManager>();
            }
            //if (_zoneManagerGo != null) {
            //    _zoneManager = _zoneManagerGo?.GetComponent<IZoneManager>();
            //}
            //if (_soundManagerGo != null) {
            //    _soundManager = _soundManagerGo?.GetComponent<ISoundManager>();
            //}
        }

        void OnDestroy() {
            //__.GameBus.UnregisterByEvent(GameEvt.PLAYER_INTERACTED, onPlayerInteracted);
            //__.GameBus.UnregisterByEvent(GameEvt.PLAYER_INTERACTED_WITH_UNIT, onPlayerInteractedUnit);
            //__.GameBus.UnregisterByEvent(GameEvt.PLAYER_ATTACKED_WITH_UNIT, onPlayerAttackedUnit);
            //__.GameBus.UnregisterByEvent(GameEvt.PLAY_SFX, onPlaySfx);
        }

        void OnApplicationQuit() {
            Debug.Log("ApplicationQuit - Reseting Static Properties");
            __.ClearAll();
            Store.CleanUp();
        }

        protected virtual void Start() {
            Debug.Log("GameScene  -> Start -> GameEvt.GAME_SCENE_LOADED");

            _unitManager?.Init();
            _interactManager?.Init();
            //_zoneManager?.Init();
            //_soundManager?.Init();

            __.GameBus.Emit(GameEvt.GAME_SCENE_LOADED, this);

            //__.GameBus.On(GameEvt.PLAYER_INTERACTED, onPlayerInteracted);
            //__.GameBus.On(GameEvt.PLAYER_INTERACTED_WITH_UNIT, onPlayerInteractedUnit);
            //__.GameBus.On(GameEvt.PLAYER_ATTACKED_WITH_UNIT, onPlayerAttackedUnit);

            //__.GameBus.On(GameEvt.PLAY_SFX, onPlaySfx);
        }

        public void SetPlayer(IPlayer player) {
            _player = player;
        }

        public virtual void StartScene() { }

        //protected virtual void onPlayerInteracted(object obj) {
        //    PlayerInteracted?.Invoke((int)obj);
        //}
        //protected virtual void onPlayerInteractedUnit(object obj) {
        //    PlayerInteractedUnit?.Invoke((int)obj);
        //}
        //protected virtual void onPlayerAttackedUnit(object obj) {
        //    Debug.Log("onPlayerAttackedUnit: ");
        //    PlayerAttackedUnit?.Invoke((int)obj);
        //}
        //protected virtual void onPlaySfx(object obj) {
        //    (_soundManager as ISFXSoundManager)?.PlaySoundFx((SFXName)obj);
        //}
    }
}
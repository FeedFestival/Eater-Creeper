using Game.Shared.Bus;
using Game.Shared.Core.Store;
using Game.Shared.Interfaces;
using System;
using R3;
using UnityEngine;
using Game.Shared.Bus.GameEvents;
using Game.Player.UI;

namespace Game.Player {
    public class Player : MonoBehaviour, IPlayer {

        [SerializeField]
        private GameObject _unitGo;

        [SerializeField]
        private CameraTarget _cameraTarget;

        //-----------------------------------------------------------------------------------------------

        public UIManager UIManager;
        public CameraController CameraController;

        public IUnit Unit { get; private set; }

        public IPlayerControl PlayerControl { get; set; }

        //-----------------------------------------------------------------------------------------------

        void Awake() {

            Unit = _unitGo.GetComponent<IUnit>();
            _unitGo = null;

            Store.InitStore();

            //_ambientSoundManager = _ambientSoundManagerGo.GetComponent<IAmbientSoundManager>();
            //_ambientSoundManagerGo = null;

            __.GameBus.On(GameEvt.GAME_SCENE_LOADED, gameSceneLoaded);
        }

        void Start() {

            //_ambientSoundManager.Init();
            Unit.Init();
            Unit.InitMovementTarget(transform);

            var inputMouse = GetComponent<InputMouse>();

            PlayerControl = new PlayerControl(this, inputMouse, _cameraTarget);
            UIManager.constructor();
            CameraController.Init(_cameraTarget, UIManager);

            //_cameraController.OnCameraFocussedInteractable += UI.SetContextAction;
        }

        void OnDestroy() {
            Store.CleanUp();

            //_gameStateDisposable.Dispose();

            __.GameBus.UnregisterByEvent(GameEvt.GAME_SCENE_LOADED, gameSceneLoaded);
            //__.GameBus.UnregisterByEvent(GameEvt.PLAY_AMBIENT, onPlayAmbient);

        }

        private void gameSceneLoaded(object obj) {
            var gameScene = obj as IGameScene;
            Debug.Log("__.GameBus.On -> GameEvt.GAME_SCENE_LOADED");
            gameScene.SetPlayer(this);
            gameScene.StartScene();
        }

        private void onPlayAmbient(object obj) {
            //_ambientSoundManager.PlayAmbient((AmbientSFXName)obj);
        }
    }
}

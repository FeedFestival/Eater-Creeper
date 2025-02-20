using Game.Player.UI;
using Game.Shared.Bus;
using Game.Shared.Bus.InputEvents;
using Game.Shared.Constants.Store;
using Game.Shared.Core.Store;
using Game.Shared.Interfaces;
using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player {
    public class PlayerControl : IPlayerControl {

        public Action ExitInteraction { get; set; }

        private IMotor _motor;
        private IPlayMotor _playerMotor;
        private ICameraTarget _cameraTarget;
        private InputMouse _inputMouse;
        private UIManager _uIManager;
        private InputManager _input;

        private Subject<(Vector2 pos, bool cancel)> _movePerformed_s = new Subject<(Vector2 pos, bool cancel)>();

        private IDisposable _gamestateSelector;

        public PlayerControl(IPlayer player, InputMouse inputMouse, ICameraTarget cameraTarget) {
            _motor = player.Unit.Motor;
            _playerMotor = (player.Unit as IPlayUnit).PlayerMotor;
            _uIManager = (player as Player).UIManager;
            _cameraTarget = cameraTarget;

            _movePerformed_s
                .Debounce(TimeSpan.FromMilliseconds(50))
                .Do(((Vector2 pos, bool cancel) payload) => {

                    var gameplayState = Store.State.GameState.gameplayState;

                    if (gameplayState == GameplayState.StrategicExploration) {
                        _cameraTarget.Move(payload.pos, payload.cancel);
                    } else {
                        _playerMotor.Move(payload.pos, payload.cancel);
                    }
                })
                .Subscribe();

            _inputMouse = inputMouse;

            _input = new InputManager();
            _input.Enable();

            _input.PlayerLook.Look.performed += lookPerformed;
            _input.PlayerLook.Look.canceled += lookCanceled;

            /// PLAYER

            _input.Player.Movement.performed += movementPerformed;
            _input.Player.Movement.canceled += movementCanceled;
            _input.Player.Sprint.performed += sprintPerformed;
            _input.Player.Fire.performed += firePerformed;  // TODO: rename to Interact
            _input.Player.StrategicControl.performed += strategicControlPerformed;
            _input.Player.Options.performed += optionsPerformed;

            _input.Player.InspectorView.performed += inspectorViewPerformed;
            _input.Player.InspectorView.canceled += inspectorViewPerformed;

            /// MENUS

            _input.Menus.Select.performed += menuSelectionPerformed;

            /// GLOBAL

            _input.Global.PauseMenu.performed += pauseMenuPerformed;

            _playerMotor.AnalogControl = false;

            _gamestateSelector = Store.Select<GameplayState>(Selector.GameplayState)
                .DistinctUntilChanged()
                .Do(onGameStateChange)
                .Subscribe();
        }

        public void Teleport(Vector3 position, bool smooth = false)
            => _motor.Teleport(position, smooth);
        public void Sprint(bool value) => _motor.Sprint(value);
        public void MoveTo(Vector3 pos) {
            _playerMotor.StopMotor();
            _motor.SetActiveMotor();
            _motor.MoveTo(pos);
        }

        //---------------------------------------------------------------------------------------------

        private void lookPerformed(InputAction.CallbackContext context) {
            if (context.control.device is Keyboard || context.control.device is Mouse) {
                _inputMouse.MouseMoved(true);
            } else {
                // Here we want to get the value from the joystick and move an
                // UI Element by that amount
                // but we probably should use _inputMouse for that
                //_uIManager.HUD.SetLookScreenPosition(context.ReadValue<Vector2>());
            }
        }

        private void lookCanceled(InputAction.CallbackContext context) {
            if (context.control.device is Keyboard || context.control.device is Mouse) {
                _inputMouse.MouseMoved(false);
            } else {
                //_uIManager.HUD.SetLookScreenPosition(context.ReadValue<Vector2>());
            }
        }

        /// PLAYER
        //---------------------------------------------------------------------------------------------

        private void strategicControlPerformed(InputAction.CallbackContext context) {
            var strategicPressed = context.ReadValueAsButton();
            if (!strategicPressed) { return; }

            var state = Store.State.GameState;
            if (state.gameplayState == GameplayState.FreePlay) {
                
                Store.Dispatch(StoreAction.SetGameplayState, GameplayState.StrategicExploration);

            } else if (state.gameplayState == GameplayState.StrategicExploration) {

                Store.Dispatch(StoreAction.SetGameplayState, GameplayState.FreePlay);
            }
        }

        private void firePerformed(InputAction.CallbackContext context) {
            var firePressed = context.ReadValueAsButton();
            if (!firePressed) { return; }

            var state = Store.State.GameState;
            if (state.gameplayState == GameplayState.FreePlay || state.gameplayState == GameplayState.StrategicExploration) {

                __.InputBus.Emit(InputEvt.CLICK_PERFORMED);

            } else if (state.gameplayState == GameplayState.BrowsingMenus) {

                if (state.uiInteractionScreen == UIInteractionScreen.WheelSelectionView) {

                    __.InputBus.Emit(InputEvt.MENU_SELECTION_PERFORMED);

                }
            }
        }

        //private IEnumerator dragUpdate() {
        //    var clickValue = _input.Player.Fire.ReadValue<float>();
        //    //Debug.Log("__.InputBus.Emit(InputEvt.DO_ACTION_PRESSED): " + clickValue);
        //    var isPressing = clickValue != 0;

        //    while (isPressing) {
        //        Debug.Log("isPressing: " + isPressing);
        //    }
        //}

        private void movementPerformed(InputAction.CallbackContext context) {

            _motor.StopMotor();
            _playerMotor.SetActiveMotor();

            var position = context.ReadValue<Vector2>();
            _movePerformed_s.OnNext((position, false));
        }

        private void movementCanceled(InputAction.CallbackContext context) {

            _motor.StopMotor();
            _playerMotor.SetActiveMotor();

            var position = context.ReadValue<Vector2>();
            _movePerformed_s.OnNext((position, true));
        }

        private void sprintPerformed(InputAction.CallbackContext context) {
            var sprintPressed = context.ReadValueAsButton();
            _playerMotor.Sprint(sprintPressed); // TODO get back
        }

        private void optionsPerformed(InputAction.CallbackContext context) {
            var optionsPressed = context.ReadValueAsButton();
            if (!optionsPressed) {
                return;
            }

            var storeGameState = Store.State.GameState;
            if (storeGameState.gamePhase == GamePhase.InGame) {

                if (storeGameState.gameplayState == GameplayState.FreePlay) {

                    Store.Dispatch<(GameplayState gameplayState, UIInteractionScreen uiInteractionScreen)>(
                        StoreAction.SetGameplay_UIInteractionScreen,
                        (GameplayState.BrowsingMenus, UIInteractionScreen.WheelSelectionView)
                    );
                } else if (storeGameState.gameplayState == GameplayState.BrowsingMenus
                    && storeGameState.uiInteractionScreen == UIInteractionScreen.WheelSelectionView) {

                    Store.Dispatch<(GameplayState gameplayState, UIInteractionScreen uiInteractionScreen)>(
                        StoreAction.SetGameplay_UIInteractionScreen,
                        (GameplayState.FreePlay, UIInteractionScreen.None)
                    );
                }
            }
        }

        private void inspectorViewPerformed(InputAction.CallbackContext context) {
            var inspectPressed = context.ReadValueAsButton();
            __.InputBus.Emit(InputEvt.INSPECTOR_VIEW_PERFORMED, inspectPressed);
        }

        /// MENUS
        //---------------------------------------------------------------------------------------------



        private void menuSelectionPerformed(InputAction.CallbackContext context) {
            var pressed = context.ReadValueAsButton();
            if (!pressed) { return; }

            Debug.Log("__.InputBus.Emit(InputEvt.MENU_SELECTION_PERFORMED);: " + 0);
            __.InputBus.Emit(InputEvt.MENU_SELECTION_PERFORMED);
        }



        /// GLOBAL
        //---------------------------------------------------------------------------------------------

        private void pauseMenuPerformed(InputAction.CallbackContext context) {
            var pauseMenuPressed = context.ReadValueAsButton();
            if (!pauseMenuPressed) {
                return;
            }

            var store = Store.State.GameState;

            if (store.gameplayState == GameplayState.FreePlay) {

                if (store.playerState == PlayerState.Interacting) {

                    // Exit Interaction
                } else {
                    Store.Dispatch<(GameplayState gameplayState, UIInteractionScreen uiInteractionScreen)>(
                        StoreAction.SetGameplay_UIInteractionScreen,
                        (GameplayState.BrowsingMenus, UIInteractionScreen.MainMenuView)
                    );
                }
            } else if (store.gameplayState == GameplayState.BrowsingMenus
                || store.gameplayState == GameplayState.StrategicExploration) {
                Store.Dispatch(StoreAction.SetGameplayState, GameplayState.FreePlay);
            }
        }

        private void onGameStateChange(GameplayState gameplayState) {
            switch (gameplayState) {
                case GameplayState.FreePlay:
                    _input.Player.Enable();
                    _input.Menus.Disable();
                    _input.PlayerLook.Enable();
                    break;
                case GameplayState.BrowsingMenus:
                    _input.Player.Disable();
                    _input.Menus.Enable();
                    _input.PlayerLook.Disable();
                    break;
                case GameplayState.StrategicExploration:
                    _input.Player.Enable();
                    _input.Menus.Disable();
                    _input.PlayerLook.Enable();
                    break;
                default:
                case GameplayState.Loading:
                    _input.Player.Disable();
                    _input.Menus.Disable();
                    _input.PlayerLook.Disable();
                    break;
            }
        }
    }
}
using Game.Shared.Constants;
using Game.Shared.Constants.Store;
using R3;
using UnityEngine;

namespace Game.Shared.Core.Store {

    public enum StoreAction {
        SetGamePhase,
        SetGameplayState,
        SetPlayerState,
        SetUnitState,
        SetUIInteractionScreen,
        //
        SetGameplayAndPlayerState,
        SetGameplay_UIInteractionScreen,
        //
        SetFoccusedInteractable
    }

    public struct GameState {
        public GamePhase gamePhase { get; set; }
        public GameplayState gameplayState { get; set; }
        public PlayerState playerState { get; set; }
        public UnitState unitState { get; set; }
        public UIInteractionScreen uiInteractionScreen { get; set; }

        //

        public int FocusedTriggerID { get; set; }
    }

    internal static class Reducers {

        internal static GameState InitialState() {
            return new GameState() {
                FocusedTriggerID = -1
            };
        }

        internal static void Init() {

            Store.Reducer(StoreAction.SetGamePhase, (GameState store, GamePhase payload) => {
                store.gamePhase = payload;

                return store;
            });
            Store.Reducer(StoreAction.SetGameplayState, (GameState store, GameplayState payload) => {
                store.gameplayState = payload;

                return store;
            });
            Store.Reducer(StoreAction.SetUnitState, (GameState store, UnitState payload) => {
                store.unitState = payload;

                return store;
            });
            //Store.Reducer(StoreAction.SetPlayerState, (GameState store, UnitState payload) => {
            //    store.unitState = payload;

            //    return store;
            //});
            Store.Reducer(StoreAction.SetGameplayAndPlayerState, (GameState store, (GameplayState gameplayState, PlayerState playerState) payload) => {
                store.gameplayState = payload.gameplayState;
                store.playerState = payload.playerState;

                return store;
            });

            Store.Reducer(StoreAction.SetUIInteractionScreen, (GameState store, UIInteractionScreen payload) => {
                store.uiInteractionScreen = payload;

                return store;
            });

            Store.Reducer(StoreAction.SetGameplay_UIInteractionScreen,
                (GameState store,
                (GameplayState gameplayState, UIInteractionScreen uiInteractionScreen) payload
                ) => {
                    store.gameplayState = payload.gameplayState;
                    store.uiInteractionScreen = payload.uiInteractionScreen;

                    return store;
                });

            //

            Store.Reducer(StoreAction.SetFoccusedInteractable, (GameState store, int focusedTrigger) => {
                store.FocusedTriggerID = focusedTrigger;
                return store;
            });
        }
    }
}
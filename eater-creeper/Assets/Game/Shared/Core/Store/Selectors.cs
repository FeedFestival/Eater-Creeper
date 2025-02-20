using Game.Shared.Constants.Store;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Core.Store {

    public enum Selector {
        Store,
        GamePhase,
        GameplayState,
        GamePhase_Gameplay_InteractionScreen,
        PlayerState,
        GameplayStateAndPlayerState,
        UnitState,
        UIInteractionScreen,
        GameplayState_UIInteractionScreen,

        //

        FoccusedInteractable
    }

    internal static class Selectors {

        internal static void Init() {
            Store.AddSelector("Store", Selector.Store, (store) => store);

            Store.AddSelector("GamePhase", Selector.GamePhase, (store) => store.gamePhase);

            Store.AddSelector("GameplayState", Selector.GameplayState, (store) => store.gameplayState);

            Store.AddSelector<(GamePhase gamePhase, GameplayState gameplayState, UIInteractionScreen uiInteractionScreen)>(
                Selector.GamePhase_Gameplay_InteractionScreen,
                (store) => (store.gamePhase, store.gameplayState, store.uiInteractionScreen)
            );

            Store.AddSelector("PlayerState", Selector.PlayerState, (store) => store.playerState);

            Store.AddSelector<(GameplayState, PlayerState)>(
                Selector.GameplayStateAndPlayerState,
                (store) => (store.gameplayState, store.playerState)
            );

            Store.AddSelector<(GameplayState gameplayState, UIInteractionScreen uiInteractionScreen)>(
                Selector.GameplayState_UIInteractionScreen,
                (store) => (store.gameplayState, store.uiInteractionScreen)
            );

            Store.AddSelector("UnitState", Selector.UnitState, (store) => store.unitState);

            //

            Store.AddSelector(
                Selector.FoccusedInteractable,
                (store) => {
                    return store.FocusedTriggerID;
                }
            );
        }
    }
}
using Game.Scene;
using Game.Shared.Bus.InputEvents;
using Game.Shared.Bus;
using Game.Shared.Constants.Store;
using Game.Shared.Constants;
using Game.Shared.Core.Store;
using UnityEngine;
using Game.Shared.Interfaces;

public class GameScene_TheDungeon : GameScene
{

    public override void StartScene() {

        //ftLightmaps.RefreshFull();
        LightProbes.Tetrahedralize();

        Store.Dispatch(StoreAction.SetGamePhase, GamePhase.InGame);
        Store.Dispatch(StoreAction.SetGameplayState, GameplayState.FreePlay);
        Store.Dispatch(StoreAction.SetUnitState, UnitState.Cautios);

        __.InputBus.On(InputEvt.CLICK_PERFORMED, onClickPerformed);

    }

    private void onClickPerformed() {
        var focusedTrigger = Store.State.GameState.FocusedTriggerID;
        var entityType = EntityDefinition.GetEntityTypeFromId(focusedTrigger);
        //switch (entityType) {
        //    case EntityType.Unit:
        //        break;
        //    case EntityType.Interactable:

        //        var interactable = _interactManager.Interactables[focusedTrigger] as IDefaultInteraction;
        //        interactable.DoDefaultInteraction(_player);

        //        break;
        //    case EntityType.Zone:
        //    case EntityType.General:
        //    case EntityType.Item:
        //    case EntityType.Traversable:
        //    case EntityType.Unset:
        //    default:
        //        break;
        //}
    }
}

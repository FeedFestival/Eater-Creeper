using Game.Scene;
using Game.Shared.Bus;
using Game.Shared.Bus.InputEvents;
using Game.Shared.Constants;
using Game.Shared.Constants.Store;
using Game.Shared.Core.Store;
using Game.Shared.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chapter.Playground {
    public class GameScene_Playground : GameScene {

        private IUnit _npcTestUnit;
        private IUnit _batmanUnit;

        [Header("Playground")]
        [SerializeField]
        private Transform _patrolT;
        private int _posI;
        private int _batman_posI;
        private List<Transform> _points;

        public override void StartScene() {

            //ftLightmaps.RefreshFull();
            LightProbes.Tetrahedralize();

            Store.Dispatch(StoreAction.SetGamePhase, GamePhase.InGame);
            Store.Dispatch(StoreAction.SetGameplayState, GameplayState.FreePlay);
            Store.Dispatch(StoreAction.SetUnitState, UnitState.Cautios);

            __.InputBus.On(InputEvt.CLICK_PERFORMED, onClickPerformed);

            testNpcAndBatman();
        }


        //-----------------------------------------------------------------------------------------

        private void onClickPerformed() {
            var focusedTrigger = Store.State.GameState.FocusedTriggerID;
            var entityType = EntityDefinition.GetEntityTypeFromId(focusedTrigger);
            switch (entityType) {
                case EntityType.Unit:
                    break;
                case EntityType.Interactable:

                    var interactable = _interactManager.Interactables[focusedTrigger] as IDefaultInteraction;
                    interactable.DoDefaultInteraction(_player);

                    break;
                case EntityType.Zone:
                case EntityType.General:
                case EntityType.Item:
                case EntityType.Traversable:
                case EntityType.Unset:
                default:
                    break;
            }
        }

        private void OnDestroy() {
            __.InputBus.UnregisterByEvent(InputEvt.CLICK_PERFORMED, onClickPerformed);
        }

        private void testNpcAndBatman() {
            _npcTestUnit = _unitManager.Units[2];
            _batmanUnit = _unitManager.Units[3];

            _npcTestUnit.Motor.DestinationReached += moveNext;
            _batmanUnit.Motor.DestinationReached += batman_moveNext;

            _points = new List<Transform>();
            foreach (Transform childT in _patrolT) {
                _points.Add(childT);
            }

            _posI = -1;
            moveNext();
            _batman_posI = -1;
            batman_moveNext();
        }

        private void moveNext() {
            _posI++;
            if (_posI >= _points.Count) {
                _posI = 0;
            }
            StartCoroutine(moveTo(_npcTestUnit, _posI, 0.5f, 2f));
        }

        private void batman_moveNext() {
            _batman_posI++;
            if (_batman_posI >= _points.Count) {
                _batman_posI = 0;
            }
            StartCoroutine(moveTo(_batmanUnit, _batman_posI, 1f, 1));
        }

        private IEnumerator moveTo(IUnit unit, int i, float wait, float untilSprint) {

            yield return new WaitForSeconds(wait);

            unit.UnitControl.Sprint(false);
            unit.UnitControl.MoveTo(_points[i].position);

            if (untilSprint >= 0) {

                yield return new WaitForSeconds(untilSprint);

                unit.UnitControl.Sprint(true);
            }
        }
    }
}
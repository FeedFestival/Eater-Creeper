using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;
using R3;
using Game.Shared.Core.Store;

namespace Game.Unit {
    public class PlayUnit : Unit, IPlayUnit {

        public IPlayMotor PlayerMotor { get; set; }

        public override bool Init() {
            base.initAnimator();
            base.initMotor(enableNavMeshAgent: false);

            PlayerMotor = gameObject.GetComponent<IPlayMotor>();
            PlayerMotor.Init(Animator, enableNavMeshAgent: false);

            onUnitStateChange(Store.State.GameState.unitState);

            Store.Select<UnitState>(Selector.UnitState)
                .Do(onUnitStateChange)
                .Subscribe();

            return base.initEntityId();
        }

        public override void InitMovementTarget(Transform movementIndicatorT) {
            var trigger = base.createMovementTargetTrigger(movementIndicatorT);
            Motor.SetMovementTarget(trigger);
        }

        private void onUnitStateChange(UnitState unitState) {

            if (unitState == UnitState.Hidden) {

                gameObject.SetActive(false);
                Motor.EnableNavMeshAgent(false);

            } else {

                gameObject.SetActive(true);
                Motor.EnableNavMeshAgent();

                switch (unitState) {
                    case UnitState.Relaxed:
                        break;
                    case UnitState.Cautios:
                        break;
                    case UnitState.Crouching:
                        break;
                    case UnitState.Sprinting:
                        break;
                    case UnitState.Interacting:
                        break;
                    case UnitState.Hidden:
                    default:
                        break;
                }
            }
        }
    }
}
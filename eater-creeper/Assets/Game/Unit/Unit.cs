using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class Unit : MonoBehaviour, IUnit {

        [SerializeField]
        protected UnitState _unitState;
        
        // --

        public int ID { get; set; }
        public Transform Transform { get => transform; }
        public Animator Animator { get; private set; }
        public IUnitControl UnitControl { get; private set; }
        public IMotor Motor { get; private set; }

        public virtual bool Init() {
            initAnimator();
            initMotor();

            UnitControl = new UnitControl(Motor);

            return initEntityId();
        }

        public virtual void InitMovementTarget(Transform movementIndicatorT) {
            var trigger = createMovementTargetTrigger(movementIndicatorT);
            Motor.SetMovementTarget(trigger);
        }

        public virtual void SetUnitState(UnitState unitState) {
            if (_unitState == unitState) { return; }
            setUnitState(unitState);
        }

        //------

        protected void initAnimator() {
            Animator = GetComponent<Animator>();
        }

        protected void initMotor(bool enableNavMeshAgent = true) {
            Motor = gameObject.GetComponent<IMotor>();
            Motor.Init(Animator, enableNavMeshAgent);
        }

        protected bool initEntityId() {
            var entity = gameObject.GetComponent<IEntityID>();
            ID = entity.ID;
            var isGeneral = entity.IsGeneral;
            entity.DestroyComponent();
            return isGeneral;
        }

        protected void setUnitState(UnitState unitState) {
            _unitState = unitState;

            switch (_unitState) {
                case UnitState.Relaxed:
                case UnitState.Interacting:
                    //Actor.gameObject.SetActive(true);
                    break;
                case UnitState.Hidden:
                default:
                    //Actor.gameObject.SetActive(false);
                    break;
            }
        }

        protected ITrigger createMovementTargetTrigger(Transform parent) {
            var go = new GameObject();
            go.transform.SetParent(parent);
            go.transform.localScale = new Vector3(0.3f, 0.2f, 0.3f);
            var collider = go.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            go.layer = 8;   // MOVEMENT layer
            var movementTarget = go.AddComponent<MovementTarget>();
            movementTarget.Init(ID);
            movementTarget.Enable(false);
            return movementTarget;
        }
    }
}
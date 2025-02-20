using Game.Shared.Interfaces;
using System;
using UnityEngine;

namespace Game.Interact {
    public class BaseInteractable : MonoBehaviour, IFocusable, IInteractable, IDefaultInteraction {

        [SerializeField]
        private GameObject _focusTriggerGo;


        //-----------------------------------------------------------------------------------

        public int ID { get; private set; }
        public Action OnInteracted { get; set; }
        public Transform Transform { get => transform; }
        public virtual bool Init() {
            var isGeneral = initEntityId();
            initFocusTrigger();

            SetFocused(false);

            return isGeneral;
        }

        public void SetFocused(bool focus = true) {
            
        }

        public virtual void DoDefaultInteraction(IPlayer player) {
            OnInteracted?.Invoke();
        }

        public virtual void DoDefaultInteraction(IUnit unit) {
            OnInteracted?.Invoke();
        }

        //-----------------------------------------------------------------------------------

        void OnDestroy() { }

        //-----------------------------------------------------------------------------------

        protected bool initEntityId() {
            var entity = gameObject.GetComponent<IEntityID>();
            ID = entity.ID;
            var isGeneral = entity.IsGeneral;
            entity.DestroyComponent();
            return isGeneral;
        }

        protected void initFocusTrigger() {
            if (_focusTriggerGo != null) {
                var focusTrigger = _focusTriggerGo.GetComponent<IFocusTrigger>();
                if (focusTrigger != null) {
                    focusTrigger.Init(ID);
                }
            }
            _focusTriggerGo = null;
        }
    }
}

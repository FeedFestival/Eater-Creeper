using Game.Shared.Interfaces;
using Game.Shared.Constants;
using UnityEngine;

namespace Game.Shared.Core {

    public class EntityID : MonoBehaviour, IEntityID {
        public int ID { get => calculateId(); }
        public bool IsGeneral { get; protected set; }
#if UNITY_EDITOR
        public GameObject go => gameObject;

        public virtual void SetName() {
            gameObject.name = EntityDefinition.DecodeId(ID);
        }
#endif

        public void DestroyComponent() {
            Destroy(this);
        }

        protected virtual int calculateId() { return -1; }
    }
}

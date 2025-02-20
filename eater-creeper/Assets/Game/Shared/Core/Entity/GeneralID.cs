using Game.Shared.Constants;
using Game.Shared.Constants.EntityID;
using UnityEngine;

namespace Game.Shared.Core {
    public class GeneralID : EntityID {
        [SerializeField]
        private EntityType _entityType;

        [SerializeField]
        private InteractType _interactType;

        protected override int calculateId() {
            base.IsGeneral = true;
            return EntityDefinition.EncodeId(_entityType, _interactType);
        }
    }
}

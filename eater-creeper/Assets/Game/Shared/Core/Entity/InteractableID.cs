using UnityEngine;
using Game.Shared.Constants;

namespace Game.Shared.Core {
    public class InteractableID : EntityID {
        [SerializeField]
        private InteractableName _interactable;

        protected override int calculateId() {
            return EntityDefinition.EncodeId(_interactable);
        }
    }
}

using Game.Shared.Constants;
using UnityEngine;

namespace Game.Shared.Core {
    public class ItemID : EntityID {
        [SerializeField]
        private ItemType _itemType;

        [SerializeField]
        private Item _item;

        [SerializeField]
        private ItemFor _itemFor;

        protected override int calculateId() {
            return EntityDefinition.EncodeId(_itemType, _item, _itemFor);
        }
    }
}

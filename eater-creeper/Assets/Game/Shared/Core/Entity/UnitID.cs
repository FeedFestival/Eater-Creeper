using UnityEngine;
using Game.Shared.Constants;

namespace Game.Shared.Core {
    public class UnitID : EntityID {
        [SerializeField]
        private UnitName _unitName;

        protected override int calculateId() {
            return EntityDefinition.EncodeId(_unitName);
        }
    }
}
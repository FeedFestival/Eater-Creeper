using Game.Shared.Constants;
using UnityEngine;

namespace Game.Shared.Core {
    public class ZoneID : EntityID {
        [SerializeField]
        private Zone _zone;

        [SerializeField]
        private SceneName _scene;

        protected override int calculateId() {
            return EntityDefinition.EncodeId(_zone, _scene);
        }
    }
}

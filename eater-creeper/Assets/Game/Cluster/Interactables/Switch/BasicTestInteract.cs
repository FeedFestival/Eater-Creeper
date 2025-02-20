using Game.Shared.Interfaces;
using UnityEngine;
using Game.Interact;

namespace Game.Cluster {
    public class BasicTestInteract : BaseInteractable {

        [SerializeField]
        private Vector3 _interactionPoint;

        public override bool Init() {
            //initAnimator();
            // init other stuff

            return base.Init();
        }

        public override void DoDefaultInteraction(IPlayer player) {

            player.PlayerControl.MoveTo(_interactionPoint);

            base.DoDefaultInteraction(player);
        }
    }
}

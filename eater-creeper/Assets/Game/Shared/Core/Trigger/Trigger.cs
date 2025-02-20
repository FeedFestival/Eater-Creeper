using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Shared.Core {
    public class Trigger : MonoBehaviour, ITrigger {
        public int ID { get; private set; }

        public void Init(int id) {
            ID = id;

            gameObject.name = "MovementTarget (" + ID + ")";
        }

        public void Enable(bool enable = true) => gameObject.SetActive(enable);
    }
}

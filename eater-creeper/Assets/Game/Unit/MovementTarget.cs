using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class MovementTarget : MonoBehaviour, ITrigger {
        public int ID { get; private set; }
        public void Init(int id) {
            ID = id;

            gameObject.name = "MovementTarget (" + ID + ")";
        }
        public void Enable(bool enable = true) => gameObject.SetActive(enable);
    }
}

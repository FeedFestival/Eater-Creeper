using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using UnityEngine;

namespace Game.Shared.Core {
    public class FocusTrigger : MonoBehaviour, IFocusTrigger {
        public int ID { get; private set; }
        public InteractWith Type { get; private set; }

        public void Init(int id, InteractWith interactType = InteractWith.Interactable) {
            ID = id;
            Type = interactType;
        }
        public void Enable(bool enable = true) {
            GetComponent<BoxCollider>().enabled = enable;
        }
    }
}
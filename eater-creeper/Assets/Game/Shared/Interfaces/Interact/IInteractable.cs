using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IInteractable {
        int ID { get; }
        Action OnInteracted { get; set; }
        Transform Transform { get; }

        bool Init();
    }
}

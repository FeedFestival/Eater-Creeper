using Game.Shared.Constants;
using System;

namespace Game.Shared.Interfaces {
    public interface IFocusTrigger {
        int ID { get; }
        InteractWith Type { get; }

        void Init(int id, InteractWith interactType = InteractWith.Interactable);
        void Enable(bool enable = true);
    }
}
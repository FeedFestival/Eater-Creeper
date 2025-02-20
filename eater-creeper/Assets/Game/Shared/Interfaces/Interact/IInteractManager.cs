using System.Collections.Generic;

namespace Game.Shared.Interfaces {
    public interface IInteractManager {
        public Dictionary<int, IInteractable> Interactables { get; }
        void Init();
    }
}

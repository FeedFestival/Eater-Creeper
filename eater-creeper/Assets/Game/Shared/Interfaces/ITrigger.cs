using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface ITrigger {
        int ID { get; }
        Transform transform { get; }
        void Init(int entityId);
        void Enable(bool enable = true);
    }
}

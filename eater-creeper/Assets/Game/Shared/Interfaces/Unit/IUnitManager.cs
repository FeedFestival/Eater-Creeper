using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUnitManager {
        Dictionary<int, IUnit> Units { get; }

        void Init();
    }
}
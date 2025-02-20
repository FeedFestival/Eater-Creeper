using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUnitControl {
        void Teleport(Vector3 position, bool smooth = false);
        void Sprint(bool value);
        void MoveTo(Vector3 pos);
    }
}
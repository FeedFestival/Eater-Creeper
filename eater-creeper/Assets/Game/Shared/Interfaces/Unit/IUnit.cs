using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUnit {
        int ID { get; set; }
        Transform Transform { get; }
        IUnitControl UnitControl { get; }
        IMotor Motor { get; }

        bool Init();
        void InitMovementTarget(Transform movementIndicatorT);
    }

    public interface IPlayUnit {
        IPlayMotor PlayerMotor { get; set; }
    }
}
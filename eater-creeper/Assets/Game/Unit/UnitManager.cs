using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Unit {
    public class UnitManager : MonoBehaviour, IUnitManager {

        public Dictionary<int, IUnit> Units { get; private set; }

        private int _generalUnitIndex = 0;

        public void Init() {
            Units = new Dictionary<int, IUnit>();

            var movementIndicatorT = createMovementIndicatorsParen();

            foreach (Transform t in transform) {
                var unit = t.GetComponent<IUnit>();

                if (unit == null) { continue; }

                var isGeneral = unit.Init();
                if (isGeneral) {
                    unit.ID += _generalUnitIndex;
                    unit.Transform.gameObject.name += " " + EntityDefinition.DecodeId(unit.ID) + "(" + unit.ID + ")";
                    _generalUnitIndex++;
                }
                unit.InitMovementTarget(movementIndicatorT);
                unit.Motor.SetActiveMotor();

                Units.Add(unit.ID, unit);
            }
        }

        private Transform createMovementIndicatorsParen() {
            var go = new GameObject("MovementIndicators");
            go.transform.SetParent(transform);
            return go.transform;
        }
    }
}

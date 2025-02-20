using DG.Tweening;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class UnitControl : IUnitControl {

        IMotor _motor;

        public UnitControl(IMotor motor) {
            _motor = motor;
        }

        public void Fire() {
            //if (CanFire && _firing == false) {

            //    __.GameBus.Emit(GameEvt.PLAY_SFX, SFXName.Slurp);

            //    _firing = true;
            //    var unitHit = _cameraController.GetAimHitUnit();

            //    (_actor as IFireable).FireInDirection(unitHit.origin, unitHit.direction);
            //}
        }

        public void Teleport(Vector3 position, bool smooth = false) => _motor.Teleport(position, onNavMesh: true, smooth);
        public void Sprint(bool value) => _motor.Sprint(value);
        public void MoveTo(Vector3 pos) => _motor.MoveTo(pos);

        //----------------------------------------------------------------------------------------
    }
}

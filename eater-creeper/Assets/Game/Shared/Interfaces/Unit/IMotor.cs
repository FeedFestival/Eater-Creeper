using System;
using UnityEngine;

namespace Game.Shared.Interfaces {

    public interface IBaseMotor {
        public bool ActiveMotor { get; }
        void Init(Animator animator, bool enableNavMeshAgent = true);
        void EnableNavMeshAgent(bool enable = true);
        void SetActiveMotor();
        void StopMotor();
        void Sprint(bool isSprinting);
    }

    public interface IMotor: IBaseMotor {
        Action DestinationReached { get; set; }
        void Init(Animator animator, bool enableNavMeshAgent = true);
        void SetMovementTarget(ITrigger trigger);
        void Teleport(Vector3 position, bool onNavMesh = false, bool smooth = false);
        void Sprint(bool isSprinting);
        void MoveTo(Vector3 pos);
    }
    public interface IPlayMotor: IBaseMotor {
        bool AnalogControl { get; set; }

        void Move(Vector2 move, bool cancel = false);
    }
}
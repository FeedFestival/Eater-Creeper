using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class PlayerMotor : MonoBehaviour, IPlayMotor {
        public bool AnalogControl { get; set; }
        public bool ActiveMotor { get; private set; }

        public void EnableNavMeshAgent(bool enable = true) {
            
        }

        public void Init(Animator animator, bool enableNavMeshAgent = true) {

        }

        public void Move(Vector2 move, bool cancel = false) {

        }

        public void SetActiveMotor() {
            ActiveMotor = true;
        }

        public void Sprint(bool isSprinting) {

        }

        public void StopMotor() {
            ActiveMotor = false;
        }
    }
}
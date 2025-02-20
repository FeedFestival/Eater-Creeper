using Game.Shared.Bus.InputEvents;
using Game.Shared.Bus;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player {
    public class InputMouse : MonoBehaviour {
        [Header("Refferences")]
        [SerializeField]
        private Camera _camera;
        private bool _mouseMoved;

        internal void MouseMoved(bool mouseMoved) {
            _mouseMoved = mouseMoved;
        }

        private void FixedUpdate() {
            if (_mouseMoved) {
                __.InputBus.Emit(InputEvt.MOUSE_POSITION, Mouse.current.position.ReadValue() as Vector2?);
            }
        }
    }
}

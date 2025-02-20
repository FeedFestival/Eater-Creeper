using Game.Shared.Bus.GameEvents;
using Game.Shared.Bus.InputEvents;

namespace Game.Shared.Bus {
    public static class __ {
        public static IGameBus GameBus {
            get {
                if (_gameBus == null) {
                    _gameBus = new GameBus();
                }
                return _gameBus;
            }
        }
        private static IGameBus _gameBus;

        public static IInputBus InputBus {
            get {
                if (_inputBus == null) {
                    _inputBus = new InputBus();
                }
                return _inputBus;
            }
        }
        private static InputBus _inputBus;

        public static void ClearAll() {
            _gameBus = null;
            _inputBus = null;
        }
    }
}

using System;

namespace Game.Shared.Bus.InputEvents {

    public interface IInputBus {
        bool Emit(InputEvt evt, object data = null);
        IEvtPackage On(InputEvt evt, Action handler);
        IEvtPackage On(InputEvt evt, Action<object> handler);
        void UnregisterByEvent(InputEvt evt);
        void UnregisterByEvent(InputEvt evt, Action handler);
        void UnregisterByEvent(InputEvt evt, Action<object> handler);
    }

    public class InputBus : GSBus, IInputBus {
        public bool Emit(InputEvt evt, object data = null) {
            return base.Emit(evt, data);
        }
        public IEvtPackage On(InputEvt evt, Action handler) {
            return base.On(evt, handler) as IEvtPackage;
        }
        public IEvtPackage On(InputEvt evt, Action<object> handler) {
            return base.On(evt, handler) as IEvtPackage;
        }
        public void UnregisterByEvent(InputEvt evt) {
            base.UnregisterByEvent(evt);
        }
        public void UnregisterByEvent(InputEvt evt, Action handler) {
            base.UnregisterByEvent(evt, handler);
        }
        public void UnregisterByEvent(InputEvt evt, Action<object> handler) {
            base.UnregisterByEvent(evt, handler);
        }
    }
}

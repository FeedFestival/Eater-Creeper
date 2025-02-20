using System;

namespace Game.Shared.Bus.GameEvents {
    public interface IGameBus {
        bool Emit(GameEvt evt, object data = null);
        IEvtPackage On(GameEvt evt, Action handler);
        IEvtPackage On(GameEvt evt, Action<object> handler);
        void UnregisterByEvent(GameEvt evt);
        void UnregisterByEvent(GameEvt evt, Action handler);
        void UnregisterByEvent(GameEvt evt, Action<object> handler);
    }
}

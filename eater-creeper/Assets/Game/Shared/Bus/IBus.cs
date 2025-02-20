using System;

namespace Game.Shared.Bus {
    public interface IBus {
        IEvtPackage On(object evt, Action handler);
        IEvtPackage On(object evt, Action<object> handler);
        IEvtPackage On(int eventId, Action<object> handler, TimeProvider runOn);
        void Register(object evtPackage, string evt, Action<object, object> handler);
        void Register(object evtPackage, string evt, Action<object, object> handler, TimeProvider runOn);
        void UnregisterByEvent(object evt);
        void UnregisterByEvent(object evt, Action handler);
        void UnregisterByEvent(object evt, Action<object> handler);
        void Unregister(string evt);
        void Unregister(object evtPackage);
        void UnregisterAll();
        bool Emit(object evt, object data = null);
        bool Call(string evt, object data = null);
        void Call(string evt, long delay, object data = null);
        void Call(long delay, Action callback);
        IDisposable Interval(long ticks, Action callback);
        void ClearInterval(IDisposable d);
        void ClearAllInterval();
    }
}

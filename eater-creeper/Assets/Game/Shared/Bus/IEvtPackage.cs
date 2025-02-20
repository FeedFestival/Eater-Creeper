using System;
using R3;

namespace Game.Shared.Bus {
    public interface IEvtPackage {
        Subject<object> subject { get; set; }
        Subject<bool> subjectBool { get; set; }
        IDisposable disposable { get; set; }
        Observable<object> observable { get; }
        Observable<bool> observableBool { get; }
        string busId { get; set; }
        int eventId { get; set; }
        string id { get; set; }
    }
}
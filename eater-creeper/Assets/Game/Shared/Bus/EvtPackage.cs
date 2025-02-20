using R3;
using System;

namespace Game.Shared.Bus {
    public class EvtPackage : IEvtPackage {
        public Subject<object> subject { get; set; }
        public Subject<bool> subjectBool { get; set; }
        public IDisposable disposable { get; set; }
        public Observable<object> observable { get { return subject; } }
        public Observable<bool> observableBool { get { return subjectBool; } }
        public string busId { get; set; }
        public int eventId { get; set; }
        public string id { get; set; }

        public EvtPackage() {
            subject = new Subject<object>();
            subjectBool = new Subject<bool>();
            disposable = null;
        }
    }
}
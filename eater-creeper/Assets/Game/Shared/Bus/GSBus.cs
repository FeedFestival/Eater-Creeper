using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Game.Shared.Bus {
    public class GSBus : IBus {
        private object s_stationLock;
        private object s_intervalLock;
        private Dictionary<int, List<IEvtPackage>> e_station;
        private Dictionary<string, List<IEvtPackage>> s_station;
        private List<IDisposable> s_intervals;

        public GSBus() {
            s_stationLock = new object();
            s_intervalLock = new object();
            e_station = new Dictionary<int, List<IEvtPackage>>();
            s_station = new Dictionary<string, List<IEvtPackage>>();
            s_intervals = new List<IDisposable>();
        }

        public virtual IEvtPackage On(object evt, Action handler) {
            int eventId = GSBus.GetEventId(evt);
            return OnEmpty(eventId, handler, TimeProvider.System);
        }

        public virtual IEvtPackage On(object evt, Action<object> handler) {
            int eventId = GSBus.GetEventId(evt);
            return On(eventId, handler, TimeProvider.System);
        }

        public virtual IEvtPackage On(int eventId, Action<object> handler, TimeProvider runOn) {
            var newEvtPackage = new EvtPackage {
                eventId = eventId
            };
            newEvtPackage.disposable = newEvtPackage.observable
                // .ObserveOn(runOn)
                .Subscribe(obj => handler(obj));

            lock (e_station) {
                if (e_station.ContainsKey(eventId)) {
                    e_station[eventId].Add(newEvtPackage as IEvtPackage);
                } else {
                    e_station[eventId] = new List<IEvtPackage>() { newEvtPackage as IEvtPackage };
                }
            }
            return newEvtPackage as IEvtPackage;
        }

        private IEvtPackage OnEmpty(int eventId, Action emptyHandler, TimeProvider runOn) {
            var newEvtPackage = new EvtPackage {
                eventId = eventId
            };
            newEvtPackage.disposable = newEvtPackage.observable
                // .ObserveOn(runOn)
                .Subscribe(obj => emptyHandler());

            lock (e_station) {
                if (e_station.ContainsKey(eventId)) {
                    e_station[eventId].Add(newEvtPackage as IEvtPackage);
                } else {
                    e_station[eventId] = new List<IEvtPackage>() { newEvtPackage as IEvtPackage };
                }
            }
            return newEvtPackage as IEvtPackage;
        }

        public void Register(object evtPackage, string evt, Action<object, object> handler) {
            Register(evtPackage, evt, handler, TimeProvider.System);
        }

        public void Register(object evtPackage, string evt, Action<object, object> handler, TimeProvider runOn) {
            var newEvtPackage = new EvtPackage {
                id = evtPackage.GetType().FullName,
                busId = evt
            };
            newEvtPackage.disposable = newEvtPackage.observable
                // .ObserveOn(runOn)
                .Subscribe(obj => handler(evtPackage, obj));

            lock (s_stationLock) {
                if (s_station.ContainsKey(evt)) {
                    var evtPackages = s_station[evt];
                    var shouldAdd = true;
                    foreach (var op in evtPackages) {
                        if (op.id == newEvtPackage.id) {
                            shouldAdd = false;
                            break;
                        }
                    }
                    if (shouldAdd) {
                        evtPackages.Add(newEvtPackage as IEvtPackage);
                    }
                } else {
                    s_station[evt] = new List<IEvtPackage>() { newEvtPackage as IEvtPackage };
                }
            }
        }

        public void UnregisterByEvent(object evt) {
            int eventId = GSBus.GetEventId(evt);
            lock (s_stationLock) {
                if (e_station.ContainsKey(eventId)) {
                    List<IEvtPackage> evtPackages = e_station[eventId];
                    evtPackages.ForEach(p => p.disposable?.Dispose());
                    evtPackages.Clear();
                    e_station.Remove(eventId);
                } else {
                    Debug.LogWarning("[Event Bus] try to unregister event id [" + evt + "] but not found");
                }
            }
        }

        public void UnregisterByEvent(object evt, Action handler) {
            int eventId = GSBus.GetEventId(evt);
            lock (s_stationLock) {
                if (e_station.ContainsKey(eventId)) {
                    List<IEvtPackage> evtPackages = e_station[eventId];
                    evtPackages.ForEach(p => p.disposable?.Dispose());
                    evtPackages.Clear();
                    e_station.Remove(eventId);
                } else {
                    Debug.LogWarning("[Event Bus] try to unregister event id [" + evt + "] but not found");
                }
            }
        }

        public void UnregisterByEvent(object evt, Action<object> handler) {
            int eventId = GSBus.GetEventId(evt);
            lock (s_stationLock) {
                if (e_station.ContainsKey(eventId)) {
                    List<IEvtPackage> evtPackages = e_station[eventId];
                    evtPackages.ForEach(p => p.disposable?.Dispose());
                    evtPackages.Clear();
                    e_station.Remove(eventId);
                } else {
                    Debug.LogWarning("[Event Bus] try to unregister event id [" + evt + "] but not found");
                }
            }
        }

        public void Unregister(string evt) {
            lock (s_stationLock) {
                if (s_station.ContainsKey(evt)) {
                    var evtPackages = s_station[evt];
                    evtPackages.ForEach(p => p.disposable?.Dispose());
                    evtPackages.Clear();
                    s_station.Remove(evt);
                } else {
                    Debug.LogWarning("[Event Bus] try to unregister event id [" + evt + "] but not found");
                }
            }
        }

        public void Unregister(object evtPackage) {
            lock (s_stationLock) {
                foreach (var evt in s_station.Keys) {
                    var evtPackages = s_station[evt];
                    foreach (var p in evtPackages) {
                        if (p.id == evtPackage.GetType().FullName) {
                            p.disposable?.Dispose();
                            evtPackages.Remove(p);
                            break;
                        }
                    }
                    if (evtPackages.Count == 0) {
                        s_station.Remove(evt);
                        break;
                    }
                }
            }
        }

        public void UnregisterAll() {
            lock (s_stationLock) {
                foreach (var evt in s_station.Keys) {
                    var evtPackages = s_station[evt];
                    foreach (var p in evtPackages) {
                        p.disposable?.Dispose();
                    }
                    evtPackages.Clear();
                }
                s_station.Clear();
            }
        }

        public bool Emit(object evt, object data = null) {
            int eventId = GSBus.GetEventId(evt);
            lock (e_station) {
                if (e_station.ContainsKey(eventId)) {
                    var evtPackages = e_station[eventId];
                    evtPackages.ForEach(p => p.subject.OnNext(data));
                    return true;
                } else {
                    Debug.LogWarning("[Event Bus] try to call event id [" + evt + "] but it's not register in anywhere.");
                }
            }
            return false;
        }

        public bool Call(string evt, object data = null) {
            lock (e_station) {
                if (s_station.ContainsKey(evt)) {
                    var evtPackages = s_station[evt];
                    evtPackages.ForEach(p => p.subject.OnNext(data));
                    return true;
                } else {
                    Debug.LogWarning("[Event Bus] try to call event id [" + evt + "] but it's not register in anywhere.");
                }
            }
            return false;
        }

        public void Call(string evt, long delay, object data = null) {
            Call(delay, () => {
                Call(evt, data);
            });
        }

        public void Call(long delay, Action callback) {
            var timer = Observable.Create<Unit>(o => {
                var d = Observable.Timer(new TimeSpan(delay * TimeSpan.TicksPerMillisecond)).Subscribe(o);
                return Disposable.Create(() => {
                    d.Dispose();
                });
            });
            timer.Subscribe(ticks => callback.Invoke());
        }

        public IDisposable Interval(long ticks, Action callback) {
            var d = Observable.Interval(new TimeSpan(ticks * TimeSpan.TicksPerMillisecond)).Subscribe(v => callback.Invoke());
            lock (s_intervalLock) {
                s_intervals.Add(d);
            }
            return d;
        }

        public void ClearInterval(IDisposable d) {
            lock (s_intervalLock) {
                s_intervals.Remove(d);
            }
            d.Dispose();
        }

        public void ClearAllInterval() {
            lock (s_intervalLock) {
                foreach (var d in s_intervals) {
                    d.Dispose();
                }
                s_intervals.Clear();
            }
        }

        public static int GetEventId(object evt) {
            if (evt != null) {
                if (evt.GetType().IsEnum) {
                    return (int)evt;
                } else {
                    throw new ArgumentException("Invalid event ID type");
                }
            } else {
                throw new ArgumentNullException(nameof(evt));
            }
        }
    }
}
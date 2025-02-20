using Game.Shared.Core.Store;
using Game.Shared.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using R3;
using Game.Shared.Bus.InputEvents;
using Game.Shared.Bus;

namespace Game.Interact {
    public class InteractManager : MonoBehaviour, IInteractManager {

        public Dictionary<int, IInteractable> Interactables { get; private set; }

        private IDisposable _foccusedInteractableSelect;
        private int _lastFocusedTriggerId = -1;
        private bool _highlightAll;

        public void Init() {

            Interactables = new Dictionary<int, IInteractable>();
            foreach (Transform ct in transform) {
                if (ct.gameObject.activeSelf == false) { continue; }

                var interactable = ct.GetComponent<IInteractable>();
                interactable?.Init();
                Interactables.Add(interactable.ID, interactable);
            }

            _foccusedInteractableSelect = Store.Select<int>(Selector.FoccusedInteractable)
                .DistinctUntilChanged()
                .Do(onFoccusedInteractable)
                .Subscribe();

            __.InputBus.On(InputEvt.INSPECTOR_VIEW_PERFORMED, highlightAll);
        }

        private void onFoccusedInteractable(int focusedTriggerId) {
            if (_lastFocusedTriggerId != focusedTriggerId) {
                if (_lastFocusedTriggerId != -1) {
                    if (_highlightAll == false) {
                        (Interactables[_lastFocusedTriggerId] as IFocusable).SetFocused(false);
                    }
                }
                _lastFocusedTriggerId = focusedTriggerId;
                if (_lastFocusedTriggerId != -1) {
                    (Interactables[_lastFocusedTriggerId] as IFocusable).SetFocused(true);
                }
            }
        }

        private void highlightAll(object payload) {
            _highlightAll = (bool)payload;
            var skipFocusedOne = _highlightAll == false && _lastFocusedTriggerId >= 0;
            foreach (var kvp in Interactables) {
                if (skipFocusedOne && kvp.Key == _lastFocusedTriggerId) {
                    continue;
                }
                (kvp.Value as IFocusable).SetFocused(_highlightAll);
            }
        }

        private void OnDestroy() {
            _foccusedInteractableSelect?.Dispose();
            __.InputBus.UnregisterByEvent(InputEvt.INSPECTOR_VIEW_PERFORMED, highlightAll);
        }
    }
}

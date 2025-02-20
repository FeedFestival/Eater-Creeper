using R3;
using System;
using System.Collections.Generic;

namespace Game.Shared.Core.Store {
#pragma warning disable IDE1006 // Naming Styles
    public static class Store {

        public static StoreState State { get; private set; }

        private static Dictionary<Selector, object> _selectors;
        private static Dictionary<StoreAction, object> _reducerActions;
        private static Dictionary<StoreAction, object> _effects;
        private static Dictionary<string, Selector> _typeSelectorMappings;

        private static Observable<GameState> _storeValueChange;

        public static void InitStore(StoreState storeState = null) {
            if (storeState == null) {
                State = new StoreState();
            } else {
                State = storeState;
            }
            _selectors = new Dictionary<Selector, object>();
            _reducerActions = new Dictionary<StoreAction, object>();
            _effects = new Dictionary<StoreAction, object>();
            _typeSelectorMappings = new Dictionary<string, Selector>();

            _storeValueChange = Observable.EveryValueChanged(State, it => it.GameState);

            Reducers.Init();
            Effects.Init();
            Selectors.Init();
        }

        public static void CleanUp() {
            _selectors = null;
            _reducerActions = null;
            _effects = null;
            _typeSelectorMappings = null;
        }

        // ---------------------------------    SETUP REDUCER    ------------------------------------
        internal static void Reducer<T>(StoreAction storeAction, Func<GameState, T, GameState> reducerAction) {
            _reducerActions.Add(storeAction, reducerAction);
        }

        // ---------------------------------    SETUP EFFECTS    ------------------------------------

        internal static void AddEffect<T>(StoreAction storeAction, Action<GameState, T> effect) {
            _effects.Add(storeAction, effect);
        }

        // ---------------------------------    SETUP SELECTORS    ------------------------------------

        internal static void AddSelector<T>(Selector selector) {
            _selectors.Add(selector, new Subject<T>());
        }
        internal static void AddSelector<T>(string preMappedName, Selector selector, Func<GameState, T> map) {
            _typeSelectorMappings.Add(preMappedName, selector);
            AddSelector(selector, map);
        }
        internal static void AddSelector<T>(Selector selector, Func<GameState, T> map) {
            _selectors.Add(selector, new Subject<T>());
            _storeValueChange
                .Select(map)
                .DistinctUntilChanged()
                .Do(curValue => notifyListeners(selector, curValue))
                .Subscribe();
        }

        // ---------------------------------    DISPATCH    ------------------------------------

        public static void Dispatch<T>(StoreAction action, T payload) {
            var hasEffect = _effects.ContainsKey(action);
            if (hasEffect) {
                var effect = _effects[action];
                var specificEffect = effect as Action<GameState, T>;
                specificEffect.Invoke(State.GameState, payload);
            }

            var hasReducerAction = _reducerActions.ContainsKey(action);
            if (hasReducerAction) {
                var reducerAction = _reducerActions[action];
                var specificTypeTReducer = reducerAction as Func<GameState, T, GameState>;
                updateStoreState(specificTypeTReducer.Invoke(State.GameState, payload));
            }
        }

        public static void Dispatch(StoreAction action, int payload) {
            var hasEffect = _effects.ContainsKey(action);
            if (hasEffect) {
                var effect = _effects[action];
                var specificEffect = effect as Action<GameState, int>;
                specificEffect.Invoke(State.GameState, payload);
            }

            var hasReducerAction = _reducerActions.ContainsKey(action);
            if (hasReducerAction) {
                var reducerAction = _reducerActions[action];
                var specificTypeTReducer = reducerAction as Func<GameState, int, GameState>;
                updateStoreState(specificTypeTReducer.Invoke(State.GameState, payload));
            }
        }

        // ---------------------------------    SELECT    ------------------------------------

        public static Subject<T> Select<T>(Selector? selector = null) {
            if (selector.HasValue == false) {
                try {
                    var preMappedSelector = _typeSelectorMappings[typeof(T).Name];
                    return _selectors[preMappedSelector] as Subject<T>;
                } catch (Exception) {
                    return _selectors[Selector.Store] as Subject<T>;
                }
            }
            return _selectors[selector.Value] as Subject<T>;
        }

        private static void updateStoreState(GameState gameState) {
            State.UpdateGameState(gameState);
        }

        private static void notifyListeners<T>(Selector selector, T payload) {
            var subj = _selectors[selector] as Subject<T>;
            subj.OnNext(payload);
        }
    }

    public class StoreState {
        public GameState GameState { get; private set; }

        public StoreState() {
            GameState = Reducers.InitialState();
        }

        public void UpdateGameState(GameState gameState) {
            GameState = gameState;
        }
    }
#pragma warning restore IDE1006 // Naming Styles
}

using System;
using System.Collections;
using Game.Shared.Constants;
using Game.Shared.Constants.Store;
using Game.Shared.Core.Store;
using Game.Shared.Bus;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Game.Shared.Bus.InputEvents;

namespace Chapter.StartGame {
    public class StartGame : MonoBehaviour {

        public Action OnDependenciesLoaded;

        private SceneDependency[] _scenesToLoad;
        private int _loadedIndex = -1;
        private Action<string> _onSceneLoaded;
        private IEnumerator _loadScene;
        private IEnumerator _startGameScene;

        private struct SceneDependency {
            public string name;
            public Action action;
        }

        //-----------------------------------------------------------------------------

        public void ContinueGame() {
            Debug.Log("ContinueGame()");
            _startGameScene = startGameScene();
            StartCoroutine(_startGameScene);
        }

        public void NewGame() {
            Debug.Log("NewGame()");
        }

        //-----------------------------------------------------------------------------

        private void Awake() {
            _scenesToLoad = new SceneDependency[1] {
                    new SceneDependency() {
                        name = "PLAYER",
                        action = onPlayerLoaded
                    }
                };

            OnDependenciesLoaded += onDependenciesLoaded;
            _onSceneLoaded += onSceneLoaded;

            loadNext();
        }

        private void loadNext() {

            if (_loadedIndex == _scenesToLoad.Length - 1) {
                OnDependenciesLoaded?.Invoke();
                return;
            }

            _loadedIndex++;
            _loadScene = loadScene(_scenesToLoad[_loadedIndex].name);
            StartCoroutine(_loadScene);
        }

        private void onDependenciesLoaded() {
            _onSceneLoaded -= onSceneLoaded;
            OnDependenciesLoaded -= onDependenciesLoaded;
            StopCoroutine(_loadScene);


        }

        private IEnumerator loadScene(string sceneName) {
            var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncLoadLevel.isDone) {
                yield return null;
            }

            _onSceneLoaded?.Invoke(sceneName);
        }

        private void onSceneLoaded(string sceneName) {
            Debug.Log((_loadedIndex + 1) + ". " + sceneName + " has been loaded.");

            _scenesToLoad[_loadedIndex].action?.Invoke();

            loadNext();
        }

        private IEnumerator startGameScene() {

            var loadSceneParameters = new LoadSceneParameters();
            loadSceneParameters.loadSceneMode = LoadSceneMode.Additive;

            // if you wan't to switch to a 2D physics, othersiew don't add
            // loadSceneParameters.localPhysicsMode = LocalPhysicsMode.Physics3D;

            var scene = SceneManager.LoadScene("the-dungeon", loadSceneParameters);

            while (!scene.isLoaded) {
                yield return null;
            }

            // Ensure that the newly loaded scene is set as the active scene
            var loadedScene = SceneManager.GetSceneByName("the-dungeon");
            if (loadedScene.IsValid() && loadedScene.isLoaded) {
                SceneManager.SetActiveScene(loadedScene);
            } else {
                Debug.LogError("Failed to set the active scene.");
            }

            SceneManager.UnloadSceneAsync(0);

            StopCoroutine(_startGameScene);
        }

        //-----------------------------------------------------------------------------

        private void onPlayerLoaded() {
            _scenesToLoad[_loadedIndex].action -= onPlayerLoaded;
            StartCoroutine(waitForPlayerToInitialize());
        }

        private IEnumerator waitForPlayerToInitialize() {
            yield return new WaitForSeconds(1f);

            Debug.Log("Do something on Player loaded");

            Store.Dispatch(StoreAction.SetGamePhase, GamePhase.InMainMenu);
            Store.Dispatch(StoreAction.SetGameplay_UIInteractionScreen, (GameplayState.BrowsingMenus, UIInteractionScreen.MainMenuView));

            //__.InputBus.Emit(InputEvt.MENU_SELECTION_PERFORMED);
        }
    }
}
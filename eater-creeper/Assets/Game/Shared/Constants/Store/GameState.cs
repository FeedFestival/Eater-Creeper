namespace Game.Shared.Constants.Store {

    public enum GamePhase {
        GameStarting,
        InMainMenu,
        InGame
    }

    public enum GameplayState {
        Loading,
        FreePlay,
        // TurnBasedFighting,
        StrategicExploration,
        BrowsingMenus
    }

    public enum PlayerState {
        WatchingCutcene,
        WaitingInteraction,
        Interacting,
        Exploring,
    }

    public enum UIInteractionScreen {
        None,
        InGameView,
        WheelSelectionView,
        MainMenuView,
        QuestView,
        InventoryView,
        CharacterView
    }

    // not used yet
    public enum WorldInteractionType {
        None,
        Traversing
    }
}
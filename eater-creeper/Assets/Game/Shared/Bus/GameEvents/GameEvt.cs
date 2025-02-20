namespace Game.Shared.Bus.GameEvents {
    public enum GameEvt {
        GAME_SCENE_LOADED,
        GAME_SCENE_READY,
        //
        PLAYER_INTERACTED,
        PLAYER_INTERACTED_WITH_UNIT,
        PLAYER_ATTACKED_WITH_UNIT,
        //
        PLAY_SFX,
        PLAY_AMBIENT,

        // DEBUG ___ Events
#if UNITY_EDITOR
        SHOW_MOVE_DIR_ARROW,
        CLOSEST_POINT_INDICATOR,
#endif
    }
}

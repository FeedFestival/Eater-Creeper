namespace Game.Shared.Constants.Layer {
    public static class LayerConstants {
        // Define the layers as readonly so they can be reused
        public static readonly int OBSTACLE = 1 << 6;           // OBSTACLE is layer 6
        public static readonly int INTERACT = 1 << 7;           // INTERACT is layer 7
        public static readonly int MOVEMENT = 1 << 8;           // MOVEMENT is layer 8
        public static readonly int GROUND = 1 << 9;             // GROUND   is layer 9

        //
        //public static readonly int CombinedLayerMask = GROUND | INTERACT;
    }
}

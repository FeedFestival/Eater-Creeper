namespace Game.Shared.Interfaces {
    public interface IPlayer {
        IUnit Unit { get; }
        IPlayerControl PlayerControl { get; }
    }
}
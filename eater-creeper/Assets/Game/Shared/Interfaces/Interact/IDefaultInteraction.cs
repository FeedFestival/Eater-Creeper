namespace Game.Shared.Interfaces {
    public interface IDefaultInteraction {
        void DoDefaultInteraction(IPlayer player);
        void DoDefaultInteraction(IUnit unit);
    }
}
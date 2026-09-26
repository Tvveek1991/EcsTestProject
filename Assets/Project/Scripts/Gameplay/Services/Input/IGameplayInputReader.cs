namespace Project.Scripts.Gameplay.Services.Input
{
    public interface IGameplayInputReader
    {
        GameplayInputSnapshot Snapshot { get; }

        void UpdateSnapshot();
    }
}

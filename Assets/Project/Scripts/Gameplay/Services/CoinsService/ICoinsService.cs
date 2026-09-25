namespace Project.Scripts.Gameplay.Services.CoinsService
{
    public interface ICoinsService
    {
        int TotalCount { get; }
        
        void RefreshTotalCount();
    }
}

namespace Project.Scripts.Gameplay.Ecs.Diagnostics
{
    public enum GameEcsPhase
    {
        None,
        Initialization,
        Input,
        Simulation,
        Physics,
        Presentation,
        Cleanup
    }
}

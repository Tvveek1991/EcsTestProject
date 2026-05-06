namespace Project.Scripts.Gameplay.Components
{
    public struct ReactionComponent
    {
        public ReactionType Type;
    }

    public enum ReactionType
    {
        Default,
        CompleteState
    }
}
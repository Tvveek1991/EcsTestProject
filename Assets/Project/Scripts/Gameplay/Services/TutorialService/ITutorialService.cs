using UnityEngine;

namespace Project.Scripts.Gameplay.Services.TutorialService
{
    public interface ITutorialService
    {
        int Entity { get; }
        GameObject View { get; }

        void Construct(int entity, GameObject view);
    }
}

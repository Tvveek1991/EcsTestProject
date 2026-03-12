using UnityEngine;

namespace Project.Scripts.Gameplay.Services.CanvasService
{
    public interface ICanvasService
    {
        int Entity { get; }
        Canvas Canvas { get; }

        void Construct(int entity, Canvas canvas);
        void Clear();
    }
}

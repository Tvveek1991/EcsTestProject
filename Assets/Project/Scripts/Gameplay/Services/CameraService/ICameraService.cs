using Project.Scripts.Gameplay.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.CameraService
{
    public interface ICameraService
    {
        Camera Camera { get; }
        CameraData CameraData { get; }
    }
}

using Project.Scripts.Gameplay.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Services.CameraService
{
    public class CameraService : ICameraService
    {
        private readonly Camera m_camera;
        private readonly CameraData m_cameraData;

        public Camera Camera => m_camera;
        public CameraData CameraData => m_cameraData;
        
        public CameraService(Camera camera, CameraData cameraData)
        {
            m_camera = camera;
            m_cameraData = cameraData;
        }
    }
}

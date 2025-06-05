using UnityEngine;

namespace Project.Scripts.Gameplay.Views
{
    public class HeroView : MonoBehaviour
    {
        public void SetPosition(Vector3 newPosition) => 
            transform.position = newPosition;
        
        void AE_SlideDust()
        {
            Debug.Log("Dust event");
            Vector3 spawnPosition;

            /*if (m_facingDirection == 1)
                spawnPosition = m_wallSensorR2.transform.position;
            else
                spawnPosition = m_wallSensorL2.transform.position;

            if (m_slideDust != null)
            {
                // Set correct arrow spawn position
                GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
                // Turn arrow in correct direction
                dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
            }*/
        }
    }
}
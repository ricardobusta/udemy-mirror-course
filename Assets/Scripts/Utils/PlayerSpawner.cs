using UnityEngine;

namespace Utils
{
    public class PlayerSpawner : MonoBehaviour
    {
        public GameObject unitPrefab;
        public PlayerUnitSpawner[] unitSpawners;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 2);
        }
    }
}
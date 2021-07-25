using UnityEngine;

namespace Utils
{
    public class PlayerUnitSpawner : MonoBehaviour
    {
        public GameObject unitPrefab;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 1);
        }
    }
}
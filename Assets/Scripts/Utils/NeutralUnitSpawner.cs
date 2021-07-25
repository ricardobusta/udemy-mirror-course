using System;
using UnityEngine;

namespace Utils
{
    public class NeutralUnitSpawner : MonoBehaviour
    {
        public GameObject unitPrefab;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(transform.position, 1);
        }
    }
}
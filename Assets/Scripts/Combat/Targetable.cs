using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targetable : NetworkBehaviour
    {
        [SerializeField] private Transform aimAtPoint;

        public Transform AimAtPoint => aimAtPoint;
    }
}
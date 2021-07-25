using Mirror;
using UnityEngine;

namespace Combat
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private int damageToDeal = 1;
        [SerializeField] private float destroyAfterSeconds;
        [SerializeField] private float launchForce;

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
        }

        [ServerCallback]
        private void Update()
        {
            var tr = transform;
            tr.position += tr.forward * (launchForce * Time.deltaTime);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NetworkIdentity>(out var networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient)
                {
                    return; // Don't hit objects to same connection
                }
            }

            if (other.TryGetComponent<Health>(out var health))
            {
                health.DealDamage(damageToDeal);
                NetworkServer.Destroy(gameObject);
            }
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}

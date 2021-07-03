using Combat;
using Mirror;
using UnityEngine;

namespace Units
{
    public class UnitFiring : NetworkBehaviour
    {
        [SerializeField] private Targeter targeter;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float attackRange;
        [SerializeField] private float attackCooldown;
        [SerializeField] private float rotationSpeed;

        private float _currentAttackCooldown;

        [ServerCallback]
        private void Update()
        {
            if (!CanFireAtTarget())
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(targeter.Target.transform.position - transform.position);

            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (_currentAttackCooldown > 0)
            {
                _currentAttackCooldown -= Time.deltaTime;
            }
            else
            {
                Fire(targeter.Target);
            }
        }

        [Server]
        private bool CanFireAtTarget()
        {
            if (targeter.Target.connectionToClient == connectionToClient)
            {
                return false; // Do not shoot own object
            }
            
            if (!targeter.HasTarget)
            {
                return false;
            }

            var targetTransform = targeter.Target.transform;
            return (targetTransform.position - transform.position).sqrMagnitude < (attackRange * attackRange);
        }

        [Server]
        private void Fire(Targetable target)
        {
            _currentAttackCooldown = attackCooldown;

            var projectilePosition = projectileSpawnPoint.position;
            var projectileRotation = Quaternion.LookRotation(target.AimAtPoint.position - projectilePosition);
            var projectile = Instantiate(projectilePrefab, projectilePosition, projectileRotation);
            
            NetworkServer.Spawn(projectile, connectionToClient);
        }
    }
}
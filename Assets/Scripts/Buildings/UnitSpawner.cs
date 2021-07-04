using Combat;
using Mirror;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private Health health;
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private float spawnDistance;
        
        
        private Vector3? rallyPoint;  
        
        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleOnDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleOnDie;
        }
        
        [Server]
        private void ServerHandleOnDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdSpawnUnit()
        {
            var (spawnPos, rallyPos, rotation) = GetSpawnInfo();

            var unitInstance = Instantiate(unitPrefab, spawnPos, rotation);
            unitInstance.GetComponent<UnitMovement>().SetDestination(rallyPos);
        
            NetworkServer.Spawn(unitInstance, connectionToClient);
        }

        [Server]
        private (Vector3 spawnPosition, Vector3 rallyPosition, Quaternion rotation) GetSpawnInfo()
        {
            var spawnerPosition = transform.position;

            Vector3 rallyPosition;
            if (rallyPoint.HasValue)
            {
                rallyPosition = rallyPoint.Value;
            }
            else
            {
                var spawnAngle = Random.Range(0f, Mathf.PI*2);
                rallyPosition = spawnerPosition + new Vector3(Mathf.Cos(spawnAngle) * spawnDistance * 1.1f, 0,
                    Mathf.Sin(spawnAngle) * spawnDistance * 1.1f);
            }
            
            var spawnDirection = (rallyPosition - spawnerPosition).normalized;
            var spawnPosition = spawnerPosition + (spawnDirection * spawnDistance);
            var rotation = Quaternion.LookRotation(spawnDirection, Vector3.up);
            
            return (spawnPosition, rallyPosition, rotation);
        }

        #endregion Server

        #region Client

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!hasAuthority || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
        
            CmdSpawnUnit();
        }

        #endregion Client
    }
}

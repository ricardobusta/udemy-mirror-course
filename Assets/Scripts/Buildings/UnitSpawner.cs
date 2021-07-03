using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private Health health;
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform unitSpawnPoint;

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
            var unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
        
            NetworkServer.Spawn(unitInstance, connectionToClient);
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

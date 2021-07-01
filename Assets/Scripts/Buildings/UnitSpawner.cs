using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform unitSpawnPoint;

        #region Server

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
            Debug.Log("Clicked");
            if (!hasAuthority || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
        
            CmdSpawnUnit();
        }

        #endregion Client
    }
}

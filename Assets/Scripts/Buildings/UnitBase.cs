using System;
using Combat;
using Mirror;
using UnityEngine;

namespace Buildings
{
    public class UnitBase : NetworkBehaviour
    {
        [SerializeField] private Health health;

        public static event Action<int> ServerOnPlayerDie;
        public static event Action<UnitBase> ServerOnBaseSpawned;
        public static event Action<UnitBase> ServerOnBaseDespawned;
        
        #region Server
        
        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleOnDie;
            ServerOnBaseSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleOnDie;
            ServerOnBaseDespawned?.Invoke(this);
        }
        
        [Server]
        private void ServerHandleOnDie()
        {
            NetworkServer.Destroy(gameObject);
            
            if (connectionToClient == null)
            {
                return;
            }
            
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        }

        #endregion Server
        
        #region Client
        #endregion
    }
}
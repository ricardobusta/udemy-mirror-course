using System;
using Combat;
using Mirror;
using UnityEngine;

namespace Buildings
{
    public class UnitBase : NetworkBehaviour
    {
        [SerializeField] private Health health;

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
        }

        #endregion Server
        
        #region Client
        #endregion
    }
}
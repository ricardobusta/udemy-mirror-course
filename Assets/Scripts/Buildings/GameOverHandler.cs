using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Buildings
{
    public class GameOverHandler : NetworkBehaviour
    {
        public static event Action<string> ClientOnGameOver;
        
        private List<UnitBase> _bases = new List<UnitBase>();
        
        #region Server

        public override void OnStartServer()
        {
            UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
            UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
            UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
        }

        [Server]
        private void ServerHandleBaseSpawned(UnitBase unitbase)
        {
            _bases.Add(unitbase);
        }
        
        [Server]
        private void ServerHandleBaseDespawned(UnitBase unitbase)
        {
            _bases.Remove(unitbase);

            if (_bases.Count != 1)
            {
                return;
            }

            var winnerPlayer = _bases[0].connectionToClient.connectionId.ToString();
            
            RpcGameOver($"Player {winnerPlayer}");
            
            Debug.Log("Game Over");
        }

        #endregion Server

        #region Client

        [ClientRpc]
        private void RpcGameOver(string winner)
        {
            ClientOnGameOver?.Invoke(winner);
        }

        #endregion Client
    }
}
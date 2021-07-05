using System;
using System.Collections.Generic;
using Mirror;

namespace Buildings
{
    public class GameOverHandler : NetworkBehaviour
    {
        public static event Action<string> ClientOnGameOver;
        public static event Action ServerOnGameOver;

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
        private void ServerHandleBaseSpawned(UnitBase unitBase)
        {
            _bases.Add(unitBase);
        }

        [Server]
        private void ServerHandleBaseDespawned(UnitBase unitBase)
        {
            _bases.Remove(unitBase);

            if (_bases.Count != 1)
            {
                return;
            }

            var winnerPlayer = _bases[0].connectionToClient.connectionId.ToString();

            RpcGameOver($"Player {winnerPlayer}");
            ServerOnGameOver?.Invoke();
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
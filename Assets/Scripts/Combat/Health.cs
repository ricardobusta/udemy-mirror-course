using System;
using Buildings;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 3;

        [SyncVar(hook = nameof(OnCurrentHealthChanged))]
        private int _currentHealth;

        public event Action ServerOnDie;

        public event Action<int, int> ClientOnHealthChanged;

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();

            _currentHealth = maxHealth;
            UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }

        [Server]
        public void DealDamage(int amount)
        {
            if (_currentHealth == 0)
            {
                return;
            }

            _currentHealth = Mathf.Max(0, _currentHealth - amount);

            if (_currentHealth != 0)
            {
                return;
            }
            
            ServerOnDie?.Invoke();

            Debug.Log($"Died");
        }

        [Server]
        private void ServerHandlePlayerDie(int playerId)
        {
            if (connectionToClient.connectionId != playerId)
            {
                return;
            }
            
            DealDamage(_currentHealth);
        }

        #endregion Server

        #region Client

        [Client]
        private void OnCurrentHealthChanged(int oldHealth, int newHealth)
        {
            ClientOnHealthChanged?.Invoke(newHealth, maxHealth);
        }

        #endregion Client
    }
}
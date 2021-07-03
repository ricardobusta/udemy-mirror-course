using System;
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

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();

            _currentHealth = maxHealth;
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

        #endregion Server

        #region Client

        private void OnCurrentHealthChanged(int oldHealth, int newHealth)
        {
        }

        #endregion Client
    }
}
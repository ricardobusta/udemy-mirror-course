using System;
using Combat;
using Mirror;
using Networking;
using UnityEngine;

namespace Buildings
{
    public class ResourcesGenerator : NetworkBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private int resourcesGenerated;
        [SerializeField] private float generationCooldown;
        [SerializeField] private ProgressCounter progressCounter;

        private RtsPlayer _player;
        private float _currentCooldown;

        #region Server
        
        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
            _currentCooldown = generationCooldown;
            if (connectionToClient != null && connectionToClient.identity != null)
            {
                _player = connectionToClient.identity.GetComponent<RtsPlayer>();
            }
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            if (_currentCooldown > 0)
            {
                _currentCooldown -= Time.deltaTime;
                progressCounter.SetValue(1 - (_currentCooldown / generationCooldown));
                return;
            }

            _currentCooldown = generationCooldown;
            progressCounter.SetValue(0);
            if (_player)
            {
                _player.AddResources(resourcesGenerated);
            }
        }

        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void ServerHandleGameOver()
        {
            enabled = false;
        }
        
        #endregion Server

        private void Start()
        {
            if (hasAuthority)
            {
                progressCounter.SetCounter(resourcesGenerated);
            }
            else
            {
                progressCounter.gameObject.SetActive(false);
            }
        }
    }
}
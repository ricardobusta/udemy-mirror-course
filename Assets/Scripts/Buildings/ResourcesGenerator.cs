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

        private RtsPlayer _player;
        private float _currentCooldown;

        #region Server
        
        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
            _currentCooldown = generationCooldown;
            _player = connectionToClient.identity.GetComponent<RtsPlayer>();
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
                return;
            }

            _currentCooldown = generationCooldown;
            _player.AddResources(resourcesGenerated);
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
    }
}
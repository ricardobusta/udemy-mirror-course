using System;
using System.Collections.Generic;
using Buildings;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Networking
{
    public class RtsNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitBasePrefab;
        [SerializeField] private GameOverHandler gameOverHandlerPrefab;

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        public static RtsNetworkManager RtsSingleton => singleton as RtsNetworkManager;

        public List<RtsPlayer> Players { get; } = new List<RtsPlayer>();

        private bool _isGameInProgress;

        #region Server

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (_isGameInProgress)
            {
                conn.Disconnect();
                Debug.Log("Disconnecting");
            }
            
            Debug.Log("Connecting");
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn == null || conn.identity == null)
            {
                return;
            }

            var rtsPlayer = conn.identity.GetComponent<RtsPlayer>();

            if (rtsPlayer == null)
            {
                return;
            }

            Players.Remove(rtsPlayer);

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            Players.Clear();

            _isGameInProgress = false;
        }

        [Server]
        public void StartGame()
        {
            if (Players.Count < 2)
            {
                return;
            }

            _isGameInProgress = true;

            ServerChangeScene("Map");
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var rtsPlayer = conn.identity.GetComponent<RtsPlayer>();
            var randomColor = Random.onUnitSphere;

            Players.Add(rtsPlayer);

            Debug.Log("Adding player");
            rtsPlayer.SetTeamColor(new Color(Mathf.Abs(randomColor.x), Mathf.Abs(randomColor.y),
                Mathf.Abs(randomColor.z)));
            
            rtsPlayer.SetPartyOwner(Players.Count == 1);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName.EndsWith("Map.unity"))
            {
                var gameOverHandler = Instantiate(gameOverHandlerPrefab);

                NetworkServer.Spawn(gameOverHandler.gameObject);

                foreach (var player in Players)
                {
                    var startPosition = GetStartPosition();
                    var baseInstance = Instantiate(unitBasePrefab, startPosition.position, startPosition.rotation);

                    NetworkServer.Spawn(baseInstance, player.connectionToClient);
                }
            }
        }

        #endregion Server

        #region Client

        public override void OnClientConnect(NetworkConnection conn)
        {
            ClientOnConnected?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            ClientOnDisconnected?.Invoke();
            Players.Clear();
            _isGameInProgress = false;
        }

        public override void OnStopClient()
        {
        }

        #endregion Client
    }
}
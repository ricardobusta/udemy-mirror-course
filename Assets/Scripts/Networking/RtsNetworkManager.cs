using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Random = UnityEngine.Random;

namespace Networking
{
    public class RtsNetworkManager : NetworkManager
    {
        public static bool USE_STEAM
        {
            get
            {
#if UNITY_EDITOR
                return false;
#else
                return true;
#endif
            }
        }

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
            }

            base.OnServerConnect(conn);
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

            base.OnStopServer();
        }

        [Server]
        public void StartGame()
        {
            if (Players.Count < 1)
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

            rtsPlayer.GetClientDisplayName();

            rtsPlayer.SetTeamColor(Players.Count - 1);

            rtsPlayer.SetPartyOwner(Players.Count == 1);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName == "Map")
            {
                var gameOverHandler = Instantiate(gameOverHandlerPrefab);

                NetworkServer.Spawn(gameOverHandler.gameObject);

                var playerSpawners = FindObjectsOfType<PlayerSpawner>();
                
                var indexes = Enumerable.Range(0, playerSpawners.Length).ToList();
                indexes.Shuffle();
                var i = 0;

                foreach (var player in Players)
                {
                    var playerSpawner = playerSpawners[indexes[i++]];
                    var playerSpawnerTransform = playerSpawner.transform;
                    var startPosition = playerSpawnerTransform.position;
                    player.transform.position = startPosition;
                    var baseInstance = Instantiate(playerSpawner.unitPrefab, startPosition, Quaternion.identity);
                    NetworkServer.Spawn(baseInstance, player.connectionToClient);

                    foreach (var unit in playerSpawner.unitSpawners)
                    {
                        var tr = unit.transform;
                        var unitInstance = Instantiate(unit.unitPrefab, tr.position, Quaternion.identity);
                        NetworkServer.Spawn(unitInstance, player.connectionToClient);
                    }
                }

                var neutralUnitSpawners = FindObjectsOfType<NeutralUnitSpawner>();
                foreach (var spawner in neutralUnitSpawners)
                {
                    var tr = spawner.transform;
                    var unitInstance = Instantiate(spawner.unitPrefab, tr.position, tr.rotation);
                    if (unitInstance)
                    {
                        NetworkServer.Spawn(unitInstance);
                    }
                }
            }
            
            base.OnServerSceneChanged(sceneName);
        }

        #endregion Server

        #region Client

        public override void OnClientConnect(NetworkConnection conn)
        {
            ClientOnConnected?.Invoke();
            base.OnClientConnect(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            ClientOnDisconnected?.Invoke();
            Players.Clear();
            _isGameInProgress = false;
            base.OnClientDisconnect(conn);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            SceneManager.LoadScene("Menu");
        }

        public override void Awake()
        {
            base.Awake();
            if (USE_STEAM)
            {
                GetComponent<KcpTransport>().enabled = false;
                transport = gameObject.AddComponent<FizzySteamworks>();
                gameObject.AddComponent<SteamManager>();
            }
            else
            {
                
            }
        }

        #endregion Client
    }
}
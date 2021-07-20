using System;
using System.Collections.Generic;
using Buildings;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using UnityEngine;
using UnityEngine.SceneManagement;
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

            rtsPlayer.SetTeamColor(new Color(Mathf.Abs(randomColor.x), Mathf.Abs(randomColor.y),
                Mathf.Abs(randomColor.z)));

            rtsPlayer.SetPartyOwner(Players.Count == 1);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName == "Map")
            {
                var gameOverHandler = Instantiate(gameOverHandlerPrefab);

                NetworkServer.Spawn(gameOverHandler.gameObject);

                foreach (var player in Players)
                {
                    var startPositionTransform = GetStartPosition();
                    var startPosition = startPositionTransform.position;
                    player.transform.position = startPosition;
                    Debug.Log($"player {player.netId} starting position is {startPositionTransform}");
                    var baseInstance = Instantiate(unitBasePrefab, startPosition, startPositionTransform.rotation);

                    NetworkServer.Spawn(baseInstance, player.connectionToClient);
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
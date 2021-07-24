using System.Collections.Generic;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using Networking;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Menus
{
    public class MainMenu : MonoBehaviour
    {
        private const string PLAYER_NAME_PLAYER_PREF = "PLAYER_NAME_PLAYER_PREF";
        private const string ADDRESS_PLAYER_PREF = "ADDRESS_PLAYER_PREF";
        
        [Header("Menus")] [SerializeField] private GameObject homeMenu;
        [SerializeField] private GameObject joinMenu;
        [SerializeField] private GameObject lobbyMenu;

        [Header("Home Menu")] [SerializeField] private TMP_InputField playerName;
        [SerializeField] private Button homeMenuHostIpButton;
        [SerializeField] private Button homeMenuHostSteamButton;
        [SerializeField] private Button homeMenuJoinButton;

        [Header("Join Menu")] [SerializeField] private Button joinMenuJoinButton;
        [SerializeField] private Button joinMenuBackButton;
        [SerializeField] private TMP_InputField addressInput;

        [SerializeField] private NetworkManager kcpNetworkManager;
        [SerializeField] private NetworkManager steamNetworkManager;

        [Header("Lobby Menu")] [SerializeField]
        private Button lobbyMenuBackButton;

        [SerializeField] private Button startGameButton;

        private List<GameObject> _menus;

        [SerializeField] private TMP_Text[] playerNameTexts;
        [SerializeField] private Image[] playerColorDisplays;

        #region Steam

        private Callback<LobbyCreated_t> lobbyCreated;
        private Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        private Callback<LobbyEnter_t> lobbyEntered;

        #endregion Steam

        private void Start()
        {
            _menus = new List<GameObject> {homeMenu, joinMenu, lobbyMenu};

            playerName.onEndEdit.AddListener(SetPlayerName);
            SetPlayerName(PlayerPrefs.GetString(PLAYER_NAME_PLAYER_PREF, "Player"));

            addressInput.onEndEdit.AddListener(SetAddress);
            SetAddress(PlayerPrefs.GetString(ADDRESS_PLAYER_PREF, "localhost"));

            SetupHomeMenu();
            SetupJoinMenu();
            SetupLobbyMenu();
            
            ReplaceNetworkManager(steamNetworkManager);

            EnableMenu(homeMenu);
        }

        public void ReplaceNetworkManager(NetworkManager newManagerPrefab)
        {
            if (NetworkManager.singleton != null)
            {
                DestroyImmediate(NetworkManager.singleton.gameObject);
            }
            
            RtsNetworkManager.ClientOnConnected += HandleClientConnected;
            RtsNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
            RtsPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;

            if (RtsNetworkManager.USE_STEAM)
            {
                lobbyEntered = Callback<LobbyEnter_t>.Create(OnSteamLobbyEntered);
                lobbyCreated = Callback<LobbyCreated_t>.Create(OnSteamLobbyCreated);
                gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnSteamGameLobbyJoinRequested);
            }

            Instantiate(newManagerPrefab);
        }

        private void OnDisable()
        {
            RtsNetworkManager.ClientOnConnected -= HandleClientConnected;
            RtsNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
            RtsPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        }

        private void SetupHomeMenu()
        {
            homeMenuHostIpButton.onClick.AddListener(StartIpHost);
            homeMenuHostSteamButton.onClick.AddListener(StartSteamHost);
            homeMenuJoinButton.onClick.AddListener(() => { EnableMenu(joinMenu); });
        }

        private void SetupJoinMenu()
        {
            joinMenuBackButton.onClick.AddListener(() => { EnableMenu(homeMenu); });
            joinMenuJoinButton.onClick.AddListener(() => { NetworkManager.singleton.StartClient(); });
            startGameButton.gameObject.SetActive(false);
        }

        private void SetupLobbyMenu()
        {
            lobbyMenuBackButton.onClick.AddListener(() =>
            {
                EnableMenu(homeMenu);
                if (NetworkServer.active && NetworkClient.isConnected)
                {
                    NetworkManager.singleton.StopHost();
                }
                else
                {
                    NetworkManager.singleton.StopClient();
                }
            });

            startGameButton.onClick.AddListener(() =>
            {
                NetworkClient.connection.identity.GetComponent<RtsPlayer>().CmdStartGame();
            });
        }

        private void EnableMenu(Object menu)
        {
            foreach (var menuGo in _menus)
            {
                menuGo.SetActive(menuGo == menu);
            }
        }

        private static void SetPlayerName(string name)
        {
            PlayerPrefs.SetString(PLAYER_NAME_PLAYER_PREF, name);
            RtsPlayer.localDisplayName = name;
        }

        private static void SetAddress(string address)
        {
            PlayerPrefs.SetString(ADDRESS_PLAYER_PREF, address);
            //NetworkManager.singleton.networkAddress = address;
        }

        private void HandleClientConnected()
        {
            EnableMenu(lobbyMenu);
        }

        private void HandleClientDisconnected()
        {
            EnableMenu(homeMenu);
        }

        private void AuthorityHandlePartyOwnerStateUpdated(bool state)
        {
            startGameButton.gameObject.SetActive(state);
        }

        private void ClientHandleInfoUpdated()
        {
            var players = RtsNetworkManager.RtsSingleton.Players;

            for (var i = 0; i < 4; i++)
            {
                var isPlayer = i < players.Count;
                var player = isPlayer ? players[i] : null;
                playerNameTexts[i].text = isPlayer ? player.DisplayName : "Waiting for player...";
                playerColorDisplays[i].color = isPlayer ?  RtsPlayer.TEAM_COLORS[player.TeamColor % RtsPlayer.TEAM_COLORS.Length] : Color.black;
            }
        }

        private void StartIpHost()
        {
            NetworkManager.singleton.StartHost();
        }

        private void StartSteamHost()
        {
           // ReplaceNetworkManager(steamNetworkManager);
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            EnableMenu(null);
        }

        private void OnSteamLobbyCreated(LobbyCreated_t callback)
        {
            Debug.Log("Lobby Created" + callback.m_ulSteamIDLobby);
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                EnableMenu(homeMenu);
                return;
            }
            
            var steamUserId = SteamUser.GetSteamID();
            var steamName = SteamFriends.GetFriendPersonaName(steamUserId);
            SetPlayerName(steamName);
            NetworkManager.singleton.StartHost();
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress",
                steamUserId.ToString());
        }

        private void OnSteamGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("OnSteamGameLobbyJoinRequested " + callback.m_steamIDFriend);
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnSteamLobbyEntered(LobbyEnter_t callback)
        {
            Debug.Log("OnSteamLobbyEntered " + callback.m_ulSteamIDLobby);
            
            if (NetworkServer.active)
            {
                return;
            }

            var hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");

            NetworkManager.singleton.networkAddress = hostAddress;
            var steamUserId = SteamUser.GetSteamID();
            var steamName = SteamFriends.GetFriendPersonaName(steamUserId);
            SetPlayerName(steamName);
            NetworkManager.singleton.StartClient();
        }
    }
}
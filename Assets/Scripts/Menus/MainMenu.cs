using System;
using System.Collections;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using Networking;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField] private Button homeMenuJoinIpButton;
        [SerializeField] private Button homeMenuJoinSteamButton;
        [SerializeField] private Button quitGameButton;

        [Header("Join Menu")] [SerializeField] private Button joinMenuJoinButton;
        [SerializeField] private Button joinMenuBackButton;
        [SerializeField] private TMP_InputField addressInput;

        [SerializeField] private NetworkManager kcpNetworkManager;
        [SerializeField] private NetworkManager steamNetworkManager;

        [Header("Lobby Menu")] [SerializeField]
        private Button lobbyMenuBackButton;
        [SerializeField] private Button copySteamIdButton;
        
        [SerializeField] private Button startGameButton;

        private List<GameObject> _menus;

        [SerializeField] private TMP_Text[] playerNameTexts;
        [SerializeField] private Image[] playerColorDisplays;

        #region Steam

        private Callback<LobbyCreated_t> lobbyCreated;
        private Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        private Callback<LobbyEnter_t> lobbyEntered;
        private Callback<SteamNetConnectionStatusChangedCallback_t> connectionStatusChanged;

        #endregion Steam

        private void Start()
        {
            _menus = new List<GameObject> {homeMenu, joinMenu, lobbyMenu};
            
            copySteamIdButton.gameObject.SetActive(false);

            playerName.text = PlayerPrefs.GetString(PLAYER_NAME_PLAYER_PREF, "Player"); 
            SetPlayerName(playerName.text);
            playerName.onEndEdit.AddListener(SetPlayerName);

            addressInput.text = PlayerPrefs.GetString(ADDRESS_PLAYER_PREF, "localhost");
            SetAddress(addressInput.text);
            addressInput.onEndEdit.AddListener(SetAddress);
            
            quitGameButton.onClick.AddListener(Application.Quit);

            SetupHomeMenu();
            SetupJoinMenu();
            SetupLobbyMenu();

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
                connectionStatusChanged =
                    Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnectionStatusChanged);
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

        private bool joiningSteam = false;

        private void SetupHomeMenu()
        {
            homeMenuHostIpButton.onClick.AddListener(StartIpHost);
            homeMenuHostSteamButton.onClick.AddListener(StartSteamHost);
            homeMenuJoinIpButton.onClick.AddListener(() =>
            {
                ReplaceNetworkManager(kcpNetworkManager);
                joiningSteam = false;
                EnableMenu(joinMenu);
            });
            homeMenuJoinSteamButton.onClick.AddListener(() =>
            {
                ReplaceNetworkManager(steamNetworkManager);
                joiningSteam = true;

                IEnumerator EnableMenuWithDelay()
                {
                    yield return null;
                    yield return null;
                    EnableMenu(joinMenu);
                }

                StartCoroutine(EnableMenuWithDelay());
            });
        }

        private void SetupJoinMenu()
        {
            joinMenuBackButton.onClick.AddListener(() => { EnableMenu(homeMenu); });
            joinMenuJoinButton.onClick.AddListener(() =>
            {
                if (joiningSteam)
                {
                    NetworkManager.singleton.networkAddress = addressInput.text;
                }
                NetworkManager.singleton.StartClient();
            });
            startGameButton.interactable = false;
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
            startGameButton.interactable = state;
        }

        private void ClientHandleInfoUpdated()
        {
            var players = RtsNetworkManager.RtsSingleton.Players;

            for (var i = 0; i < 4; i++)
            {
                var isPlayer = i < players.Count;
                var player = isPlayer ? players[i] : null;
                playerNameTexts[i].text = isPlayer ? player.DisplayName : "Waiting for player...";
                playerColorDisplays[i].color = isPlayer
                    ? RtsPlayer.TeamColors[player.TeamColor % RtsPlayer.TeamColors.Length]
                    : Color.black;
            }
        }

        private void StartIpHost()
        {
            copySteamIdButton.gameObject.SetActive(false);
            IEnumerator StartHostRoutine()
            {
                ReplaceNetworkManager(kcpNetworkManager);
                yield return null;
                NetworkManager.singleton.StartHost();
            }

            StartCoroutine(StartHostRoutine());
        }

        private void StartSteamHost()
        {
            IEnumerator StartHostRoutine()
            {
                ReplaceNetworkManager(steamNetworkManager);
                EnableMenu(null);
                yield return null;
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            }

            StartCoroutine(StartHostRoutine());
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
            copySteamIdButton.gameObject.SetActive(true);
            copySteamIdButton.onClick.AddListener(() => { GUIUtility.systemCopyBuffer = steamUserId.ToString();});
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress",
                steamUserId.ToString());
            NetworkManager.singleton.StartHost();
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

        private void OnConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t param)
        {
            Debug.Log(param.m_info.m_identityRemote.GetSteamID64());
        }

        private void Update()
        {
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                Screen.SetResolution(1920, 1080, true);
            }
        }
    }
}
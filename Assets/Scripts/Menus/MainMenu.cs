using System;
using System.Collections.Generic;
using Mirror;
using Networking;
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

        [Header("Menus")] //
        [SerializeField]
        private GameObject homeMenu;

        [SerializeField] // 
        private GameObject joinMenu;

        [SerializeField] //
        private GameObject lobbyMenu;

        [Header("Home Menu")] //
        [SerializeField]
        private TMP_InputField playerName;

        [SerializeField] //
        private Button homeMenuHostButton;

        [SerializeField] // 
        private Button homeMenuJoinButton;

        [Header("Join Menu")] //
        [SerializeField]
        private Button joinMenuJoinButton;

        [SerializeField] private Button joinMenuBackButton;

        [SerializeField] // 
        private TMP_InputField addressInput;

        [Header("Lobby Menu")] //
        [SerializeField]
        private Button lobbyMenuBackButton;

        [SerializeField] private Button startGameButton;

        private List<GameObject> _menus;

        [SerializeField] private TMP_Text[] playerNameTexts;
        [SerializeField] private Image[] playerColorDisplays;
        

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
            
            RtsNetworkManager.ClientOnConnected += HandleClientConnected;
            RtsNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
            RtsPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
            RtsPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;

            EnableMenu(homeMenu);
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
            homeMenuHostButton.onClick.AddListener(() => { NetworkManager.singleton.StartHost(); });
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
            NetworkManager.singleton.networkAddress = address;
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
                playerColorDisplays[i].color = isPlayer ? player.TeamColor : Color.black;
            }
        }
    }
}
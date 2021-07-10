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


        private List<GameObject> _menus;

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

            EnableMenu(homeMenu);
        }

        private void OnEnable()
        {
            RtsNetworkManager.ClientOnConnected += HandleClientConnected;
            RtsNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            RtsNetworkManager.ClientOnConnected -= HandleClientConnected;
            RtsNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
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
    }
}
using System;
using Buildings;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class GameOverDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverDisplayParent;
        [SerializeField] private TMP_Text endGameText;
        [SerializeField] private Button leaveGameButton;
        
        private void Start()
        {
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
            leaveGameButton.onClick.AddListener(OnLeaveGamePressed);
            
            gameOverDisplayParent.SetActive(false);
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        private static void OnLeaveGamePressed()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }

        private void ClientHandleGameOver(string winner)
        {
            endGameText.text = $"{winner} has won!";
            gameOverDisplayParent.SetActive(true);
        }
    }
}
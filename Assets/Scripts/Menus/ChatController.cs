using System.Collections;
using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class ChatController : MonoBehaviour
    {
        public TMP_InputField inputField;
        public TMP_Text chatHistory;
        public Scrollbar scrollbar;
        public Button submitButton;

        private void Awake()
        {
            RtsPlayer.OnMessageReceived += OnPlayerMessageReceived;

            submitButton.onClick.AddListener(OnSubmit);
            inputField.onSubmit.AddListener(OnSubmit);

            chatHistory.text = string.Empty;
        }

        private void OnDestroy()
        {
            RtsPlayer.OnMessageReceived -= OnPlayerMessageReceived;
        }

        private void OnSubmit()
        {
            var text = inputField.text.Trim();
            OnSubmit(text);
        }

        private void OnSubmit(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            player.CommandSendMessage(inputField.text.Trim());

            inputField.text = string.Empty;
            
            inputField.ActivateInputField();
        }
        
        private void OnPlayerMessageReceived(RtsPlayer player, string message)
        {
            var completeMessage = $"[<color=#{ColorUtility.ToHtmlStringRGB(RtsPlayer.TeamColors[player.TeamColor])}>{player.DisplayName}</color>]: {message}\n";
            StartCoroutine(AppendAndScroll(completeMessage));
        }

        private IEnumerator AppendAndScroll(string message)
        {
            var scrollAtEnd = scrollbar.value == 0;

            chatHistory.text += message;

            // Wait UI to update. It takes 2 frames.
            yield return null;
            yield return null;

            // Move scroll to the end
            scrollbar.value = 0;
        }
    }
}
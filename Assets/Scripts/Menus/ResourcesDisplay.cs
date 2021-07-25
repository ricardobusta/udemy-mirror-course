using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using Utils;

namespace Menus
{
    public class ResourcesDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text resourcesText;

        private RtsPlayer _player;
        private bool _playerSet;

        private void Start()
        {
            if (NetworkClient.connection == null) return;
            _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            _playerSet = _player!=null;
            _player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
            ClientHandleResourcesUpdated(_player.Resources);
        }

        private void OnDestroy()
        {
            if (_playerSet)
            {
                _player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
            }
        }

        private void ClientHandleResourcesUpdated(int resources)
        {
            resourcesText.text = resources.ToString();
        }
    }
}
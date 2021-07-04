using System;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
            LocalRtsPlayer.GetLocalPlayerAsync(player =>
            {
                _player = player;
                _playerSet = true;
                _player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
                ClientHandleResourcesUpdated(_player.Resources);
            });
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
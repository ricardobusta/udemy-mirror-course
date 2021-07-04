using System;
using System.Collections.Generic;
using Mirror;
using Networking;
using UnityEngine;

namespace Utils
{
    public class LocalRtsPlayer : MonoBehaviour
    {
        private static List<Action<RtsPlayer>> _pending = new List<Action<RtsPlayer>>();

        private RtsPlayer _rtsPlayer;
        private bool _playerSet;

        private static LocalRtsPlayer _instance;

        public static void GetLocalPlayerAsync(Action<RtsPlayer> onPlayerGet)
        {
            if (_instance != null && _instance._playerSet)
            {
                onPlayerGet?.Invoke(_instance._rtsPlayer);
                return;
            }

            if (onPlayerGet != null)
            {
                _pending.Add(onPlayerGet);
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Update()
        {
            if (_playerSet)
            {
                return;
            }

            if (NetworkClient.connection == null || NetworkClient.connection.identity == null)
            {
                return;
            }

            _rtsPlayer = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            if (_rtsPlayer == null)
            {
                return;
            }

            _playerSet = true;
            foreach (var pending in _pending)
            {
                pending?.Invoke(_rtsPlayer);
            }
            _pending.Clear();

            enabled = false;
        }
    }
}
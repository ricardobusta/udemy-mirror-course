using System;
using Mirror;
using Networking;
using UnityEngine;
using Utils;

namespace Cameras
{
    public class MapScaler : MonoBehaviour
    {
        [SerializeField] private MiniMap miniMap;

        [SerializeField] private Camera miniMapCamera;

        [SerializeField] private float scale;


        private void Start()
        {
            miniMap.SetScale(scale);
            miniMapCamera.orthographicSize = scale;

            if (NetworkClient.connection == null) return;
            var player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            player.GetComponent<CameraController>().SetScreenLimits(-scale, -scale, scale, scale);
        }
    }
}
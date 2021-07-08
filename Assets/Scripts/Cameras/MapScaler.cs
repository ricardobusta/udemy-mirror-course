using System;
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

            LocalRtsPlayer.GetLocalPlayerAsync(player =>
            {
                player.GetComponent<CameraController>().SetScreenLimits(-scale, -scale, scale, scale);
            });
        }
    }
}
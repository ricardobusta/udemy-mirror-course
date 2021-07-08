using System;
using UnityEngine;
using Utils;

namespace Cameras
{
    public class MapScaler : MonoBehaviour
    {
        [SerializeField] private MiniMap miniMap;
        [SerializeField] private Transform floorTransform;
        [SerializeField] private Camera miniMapCamera;

        [SerializeField] private float scale;


        private void Start()
        {
            var scale2 = scale * 2;
            miniMap.SetScale(scale);
            floorTransform.localScale = new Vector3(scale2, 1, scale2);
            miniMapCamera.orthographicSize = scale;

            LocalRtsPlayer.GetLocalPlayerAsync(player =>
            {
                player.GetComponent<CameraController>().SetScreenLimits(-scale, -scale, scale, scale);
            });
        }
    }
}
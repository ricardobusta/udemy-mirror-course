using System;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utils;

namespace Cameras
{
    public class MiniMap : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [SerializeField] private RectTransform miniMapRect;
        [SerializeField] private float mapScale;

        private Transform _playerCameraTransform;

        private void Start()
        {
            var player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            _playerCameraTransform = player.CameraTransform;
        }

        public void SetScale(float scale)
        {
            mapScale = scale;
        }

        private void MoveCamera()
        {
            var mousePos = Mouse.current.position.ReadValue();

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRect, mousePos, null, 
                out var localPoint))
            {
                return;
            }

            var rect = miniMapRect.rect;

            var relativePos = new Vector2((localPoint.x - rect.x) / rect.width, (localPoint.y - rect.y) / rect.height);

            var newCameraPos = new Vector3((relativePos.x * 2f - 1f) * mapScale, 0,
                (relativePos.y * 2f - 1f) * mapScale);

            _playerCameraTransform.position = newCameraPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveCamera();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveCamera();
        }
    }
}
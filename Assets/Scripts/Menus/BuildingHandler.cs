using System;
using System.Collections.Generic;
using Buildings;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Menus
{
    public class BuildingHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask floorMask;
        [SerializeField] private Building[] buildings;
        
        private bool PlacingBuilding => _buildingId != -1;
        private int _buildingId;
        private GameObject _buildingPreview;
        private Camera _mainCamera;
        private RtsPlayer _player;
        private bool _playerSet;
        private readonly Dictionary<int, Building> _buildingMap = new Dictionary<int,Building>();

        public event Action StartPlacingBuilding;
        public event Action StopPlacingBuilding;

        public IEnumerable<Building> Buildings => buildings;
        
        private void Awake()
        {
            _mainCamera = Camera.main;

            _buildingId = -1;
            
            foreach (var building in buildings)
            {
                _buildingMap.Add(building.Id, building);
            }
        }

        private void Start()
        {
            LocalRtsPlayer.GetLocalPlayerAsync(player =>
            {
                _player = player;
                _playerSet = true;
            });

            RtsPlayer.buildingMap = _buildingMap;
        }

        public void SetBuilding(int buildingId, GameObject buildingPreviewPrefab)
        {
            _buildingPreview = Instantiate(buildingPreviewPrefab);
            _buildingId = buildingId;
            StartPlacingBuilding?.Invoke();
        }
        
        public void StopBuilding()
        {
            if(_buildingPreview!=null)
            {
                Destroy(_buildingPreview.gameObject);
                _buildingPreview = null;
            }

            _buildingId = -1;
            StopPlacingBuilding?.Invoke();
        }

        private void Update()
        {
            if (!_playerSet)
            {
                return;
            }
            
            if(!PlacingBuilding)
            {
                return;
            }
            
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                // Cancel build
                StopBuilding();
                return;
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                // Cancel build
                StopBuilding();
                return;
            }

            if (!_mainCamera.RayCast(out var hit, Mathf.Infinity, floorMask))
            {
                // Targeting outside of build area
                _buildingPreview.gameObject.SetActive(false);
                return;
            }
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Place building
                _player.CmdTryPlaceBuilding(_buildingId, hit.point);
                return;
            }

            if (!_buildingPreview.activeSelf)
            {
                // Re-enable building preview if get to this point
                _buildingPreview.SetActive(true);
            }
            
            // Move building preview
            _buildingPreview.transform.position = hit.point;
        }
    }
}
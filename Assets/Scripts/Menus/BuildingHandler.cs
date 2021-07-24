using System;
using System.Collections.Generic;
using Buildings;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utils;

namespace Menus
{
    public class BuildingHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask floorMask;
        [SerializeField] private Building[] buildings;
        [SerializeField] private Material validMaterial;
        [SerializeField] private Material invalidMaterial;
        [SerializeField] private GameObject buildAreaCircle;
        

        private bool PlacingBuilding => _buildingId != -1;
        private int _buildingId;
        private GameObject _buildingPreview;
        private List<Renderer> _buildingPreviewRenderers = new List<Renderer>();
        private BoxCollider _buildingCollider;
        private Camera _mainCamera;
        private RtsPlayer _player;
        private bool _playerSet;
        private int _buildingPrice;
        private readonly Dictionary<int, Building> _buildingMap = new Dictionary<int, Building>();
        private EventSystem _eventSystem;

        public event Action StartPlacingBuilding;
        public event Action StopPlacingBuilding;

        public IEnumerable<Building> Buildings => buildings;

        private void Awake()
        {
            _mainCamera = Camera.main;

            _buildingId = -1;

            buildAreaCircle.SetActive(false);
            
            foreach (var building in buildings)
            {
                _buildingMap.Add(building.Id, building);
            }
        }

        private void Start()
        {
            _player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            _playerSet = _player!=null;
            _eventSystem = EventSystem.current;

            RtsPlayer.buildingMap = _buildingMap;
        }

        public void SetBuilding(int buildingId, GameObject buildingPreviewPrefab, int price, BoxCollider collider)
        {
            StopBuilding();
            _buildingPreview = Instantiate(buildingPreviewPrefab);
            _buildingPreview.GetComponent<Renderer>();
            _buildingId = buildingId;
            _buildingPrice = price;
            _buildingCollider = collider;
            _buildingPreviewRenderers.AddRange(_buildingPreview.GetComponentsInChildren<Renderer>());
            StartPlacingBuilding?.Invoke();
        }

        public void StopBuilding()
        {
            if (_buildingPreview != null)
            {
                Destroy(_buildingPreview.gameObject);
                _buildingPreview = null;
                _buildingPreviewRenderers.Clear();
                _buildingCollider = null;
            }
            buildAreaCircle.SetActive(false);
            _buildingId = -1;
            StopPlacingBuilding?.Invoke();
        }

        private void Update()
        {
            if (!_playerSet)
            {
                return;
            }

            if (!PlacingBuilding)
            {
                return;
            }

            var canClick = !_eventSystem.IsPointerOverUIObject();
            
            if (!canClick || !_mainCamera.RayCast(out var hit, Mathf.Infinity, floorMask))
            {
                // Targeting outside of build area
                _buildingPreview.gameObject.SetActive(false);
                buildAreaCircle.SetActive(false);
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
                buildAreaCircle.SetActive(true);
            }

            // Move building preview
            _buildingPreview.transform.position = hit.point;
            buildAreaCircle.transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z) ;
            
            var mat = _player.CanPlaceBuilding(_buildingCollider, hit.point, _buildingPrice)
                ? validMaterial
                : invalidMaterial;
            foreach (var previewRenderer in _buildingPreviewRenderers)
            {
                previewRenderer.material = mat;
            }
        }
    }
}
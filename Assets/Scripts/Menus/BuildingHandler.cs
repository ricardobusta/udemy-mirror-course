using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Menus
{
    public class BuildingHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask floorMask;
        
        private bool _placingBuilding;
        private GameObject _buildingPreview;
        private Camera _mainCamera;

        public event Action StartPlacingBuilding;
        public event Action StopPlacingBuilding;
        
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        public void SetBuilding(GameObject buildingPreviewPrefab)
        {
            Debug.Log("Start building");
            _placingBuilding = true;
            _buildingPreview = Instantiate(buildingPreviewPrefab);
            StartPlacingBuilding?.Invoke();
        }

        public void StopBuilding()
        {
            Debug.Log("End Building");
            _placingBuilding = false;
            if(_buildingPreview!=null)
            {
                Destroy(_buildingPreview.gameObject);
                _buildingPreview = null;
            }
            StopPlacingBuilding?.Invoke();
        }

        private void LateUpdate()
        {
            if(!_placingBuilding)
            {
                return;
            }
            
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                StopBuilding();
                return;
            }
            
            if (!_mainCamera.RayCast(out var hit, Mathf.Infinity, floorMask))
            {
                _buildingPreview.gameObject.SetActive(false);
                return;
            }

            if (!_buildingPreview.activeSelf)
            {
                _buildingPreview.SetActive(true);
            }
            Debug.Log(hit.point);
            _buildingPreview.transform.position = hit.point;
        }
    }
}
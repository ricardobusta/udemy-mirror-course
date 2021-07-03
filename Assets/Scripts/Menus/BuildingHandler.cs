using System;
using UnityEngine;
using Utils;

namespace Menus
{
    public class BuildingHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask floorMask;
        
        public bool placingBuilding;
        public GameObject buildingPreview;

        private Camera _mainCamera;
        
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if(!placingBuilding)
            {
                return;
            }

            if (!_mainCamera.RayCast(out var hit, Mathf.Infinity, floorMask))
            {
                return;
            }

            buildingPreview.transform.position = hit.point;

            // Move the preview if available
        }
    }
}
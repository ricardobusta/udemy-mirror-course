using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rts.Networking
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        private Camera _mainCamera;

        public List<Unit> selectedUnits = new List<Unit>();

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                foreach (var previousSelectedUnit in selectedUnits)
                {
                    previousSelectedUnit.Select(false);
                }
                selectedUnits.Clear();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
        }

        private void ClearSelectionArea()
        {
            if (!_mainCamera.RayCast(out var hit, maxDist: Mathf.Infinity, layerMask))
            {
                return;
            }

            if (!hit.collider.TryGetComponent<Unit>(out var unit))
            {
                return;
            }

            if (!unit.hasAuthority)
            {
                return;
            }

            selectedUnits.Add(unit);

            foreach (var selectedUnit in selectedUnits)
            {
                selectedUnit.Select(true);
            }
        }
    }
}
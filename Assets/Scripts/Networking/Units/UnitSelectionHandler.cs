using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rts.Networking
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        private Camera _mainCamera;

        private List<Unit> _selectedUnits = new List<Unit>();

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                foreach (var previousSelectedUnit in _selectedUnits)
                {
                    previousSelectedUnit.Select(false);
                }
                _selectedUnits.Clear();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
        }

        private void ClearSelectionArea()
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
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

            _selectedUnits.Add(unit);

            foreach (var selectedUnit in _selectedUnits)
            {
                selectedUnit.Select(true);
            }
        }
    }
}
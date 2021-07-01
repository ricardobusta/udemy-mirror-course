using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Units
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelectionHandler;
        [SerializeField] private LayerMask layerMask;

        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame)
            {
                return;
            }

            if (!_mainCamera.RayCast(out var hit, Mathf.Infinity, layerMask))
            {
                return;
            }

            TryMove(hit.point);
        }

        private void TryMove(Vector3 point)
        {
            foreach (var unit in unitSelectionHandler.selectedUnits)
            {
                unit.UnitMovement.CmdMove(point);
            }
        }
    }
}

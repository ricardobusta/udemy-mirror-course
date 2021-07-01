using Combat;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Units
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelectionHandler;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Targeter targeter;

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

            if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
            {
                if (target.hasAuthority)
                {
                    TryMove(hit.point);
                    return;
                }

                TryTarget(target);
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
        
        private void TryTarget(Targetable target)
        {
            foreach (var unit in unitSelectionHandler.selectedUnits)
            {
                unit.Targeter.CmdSetTarget(target.gameObject);
            }
        }
    }
}

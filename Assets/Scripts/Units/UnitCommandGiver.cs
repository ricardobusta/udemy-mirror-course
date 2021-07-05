using Buildings;
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

        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
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

            if (hit.collider.TryGetComponent<Targetable>(out var target))
            {
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
        
        private void ClientHandleGameOver(string winnerName)
        {
            enabled = false; // Disable giving commands when game is over
        }
    }
}

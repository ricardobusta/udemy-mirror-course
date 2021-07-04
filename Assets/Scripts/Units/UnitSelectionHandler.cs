using System.Collections.Generic;
using Buildings;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform unitSelectionArea;
        [SerializeField] private LayerMask layerMask;

        private Vector2 _startPosition;
        private RtsPlayer _player;
        private bool _playerSet;

        private Camera _mainCamera;

        public List<Unit> selectedUnits = new List<Unit>();

        private void Start()
        {
            _mainCamera = Camera.main;
            unitSelectionArea.gameObject.SetActive(false);

            unitSelectionArea.anchorMin = Vector2.zero;
            unitSelectionArea.anchorMax = Vector2.zero;
            unitSelectionArea.pivot = new Vector2(0.5f, 0.5f);

            Unit.OnAuthorityUnitDespawned += AuthorityHandleUnityDespawned;

            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
            
            LocalRtsPlayer.GetLocalPlayerAsync(player =>
            {
                _player = player;
                _playerSet = true;
            });
        }

        private void OnDestroy()
        {
            Unit.OnAuthorityUnitDespawned -= AuthorityHandleUnityDespawned;
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;

        }

        private void Update()
        {
            if (!_playerSet)
            {
                return;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }
        }

        private void StartSelectionArea()
        {
            unitSelectionArea.gameObject.SetActive(true);

            _startPosition = Mouse.current.position.ReadValue();

            UpdateSelectionArea();
        }

        private void ClearSelectionArea()
        {
            unitSelectionArea.gameObject.SetActive(false);

            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                foreach (var previousSelectedUnit in selectedUnits)
                {
                    previousSelectedUnit.Select(false);
                }

                selectedUnits.Clear();
            }

            if (unitSelectionArea.sizeDelta.magnitude == 0) // Select single unit
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

                if (selectedUnits.Contains(unit))
                {
                    return;
                }

                selectedUnits.Add(unit);
            }
            else
            {
                var anchoredPosition = unitSelectionArea.anchoredPosition;
                var halfSize = (unitSelectionArea.sizeDelta / 2);
                var min = anchoredPosition - halfSize;
                var max = anchoredPosition + halfSize;

                foreach (var unit in _player.MyUnits)
                {
                    if (selectedUnits.Contains(unit))
                    {
                        continue;
                    }

                    var unitScreenPos = _mainCamera.WorldToScreenPoint(unit.transform.position);

                    if (unitScreenPos.x > min.x &&
                        unitScreenPos.x < max.x &&
                        unitScreenPos.y > min.y &&
                        unitScreenPos.y < max.y)
                    {
                        selectedUnits.Add(unit);
                    }
                }
            }

            foreach (var selectedUnit in selectedUnits)
            {
                selectedUnit.Select(true);
            }
        }

        private void UpdateSelectionArea()
        {
            var mousePosition = Mouse.current.position.ReadValue();

            var width = mousePosition.x - _startPosition.x;
            var height = mousePosition.y - _startPosition.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            unitSelectionArea.anchoredPosition = _startPosition + new Vector2(width / 2f, height / 2f);
        }

        private void AuthorityHandleUnityDespawned(Unit unit)
        {
            selectedUnits.Remove(unit);
        }

        private void ClientHandleGameOver(string winnerName)
        {
            enabled = false; // Disable selection when game is over
        }
    }
}
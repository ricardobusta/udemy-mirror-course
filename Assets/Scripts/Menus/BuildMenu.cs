using System;
using Buildings;
using Networking;
using UnityEngine;

namespace Menus
{
    public class BuildMenu : MonoBehaviour
    {
        [SerializeField] private BuildingHandler buildingHandler;
        [SerializeField] private BuildButton buildButtonPrefab;
        [SerializeField] private Building[] buildings;
        [SerializeField] private Transform buttonParent;

        private Camera _mainCamera;
        private RtsPlayer _player;
        private GameObject _buildingPreviewInstance;
        private BuildButton _currentBuildButton;

        private void Start()
        {
            buildingHandler.StopPlacingBuilding += OnStopPlacingBuilding;

            foreach (var building in buildings)
            {
                var button = Instantiate(buildButtonPrefab, buttonParent);
                button.Set(building);
                button.OnButtonClicked += OnButtonClicked;
            }
        }

        private void OnButtonClicked(BuildButton button)
        {
            if (_currentBuildButton != button)
            {
                ClearButton();
            }

            _currentBuildButton = button;
            buildingHandler.SetBuilding(button.Building.BuildingPreview);
        }

        private void ClearButton()
        {
            if (_currentBuildButton != null)
            {
                _currentBuildButton.ResetButton();
            }
        }

        private void OnStopPlacingBuilding()
        {
            ClearButton();
        }
    }
}
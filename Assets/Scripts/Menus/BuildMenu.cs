using UnityEngine;

namespace Menus
{
    public class BuildMenu : MonoBehaviour
    {
        [SerializeField] private BuildingHandler buildingHandler;
        [SerializeField] private BuildButton buildButtonPrefab;

        [SerializeField] private Transform buttonParent;

        private BuildButton _currentBuildButton;

        private void Start()
        {
            buildingHandler.StopPlacingBuilding += OnStopPlacingBuilding;

            foreach (var building in buildingHandler.Buildings)
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
            var building = button.Building;
            buildingHandler.SetBuilding(building.Id, building.BuildingPreview, building.Price,
                building.GetComponent<BoxCollider>());
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
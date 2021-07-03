using System;
using Buildings;
using Networking;
using UnityEngine;

namespace Menus
{
    public class BuildMenu : MonoBehaviour
    {
        [SerializeField] private BuildButton buildButtonPrefab;
        [SerializeField] private Building[] buildings;
        [SerializeField] private Transform buttonParent;

        private Camera _mainCamera;
        private RtsPlayer _player;
        private GameObject _buildingPreviewInstance;

        private BuildButton currentBuildButton;

        private void Start()
        {
            foreach (var building in buildings)
            {
                var button = Instantiate(buildButtonPrefab, buttonParent);
                button.Set(building, this);
                button.OnButtonClicked += b =>
                {
                    if (currentBuildButton != null && currentBuildButton != b) currentBuildButton.ResetButton();
                    currentBuildButton = b;
                };
            }
        }
    }
}
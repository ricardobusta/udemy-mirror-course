using System;
using Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class BuildButton : MonoBehaviour
    {
        [SerializeField] private Building building;
        [SerializeField] private TMP_Text buildingName;

        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text price;

        [Header("Regular Color")] [SerializeField]
        private ColorBlock regularColor;

        [Header("SelectedColor")] [SerializeField]
        private ColorBlock selectedColor;

        public event Action<BuildButton> OnButtonClicked;

        private bool _selected;

        public Building Building => building;

        public void Set(Building setBuilding)
        {
            building = setBuilding;
            icon.sprite = setBuilding.Icon;
            price.text = $"$ {setBuilding.Price}";
            ResetButton();

            buildingName.text = building.UnitName;

            _selected = false;

            button.onClick.AddListener(() =>
            {
                if (_selected)
                {
                    return;
                }

                OnButtonClicked?.Invoke(this);
                button.colors = selectedColor;
                _selected = true;
            });
        }

        public void ResetButton()
        {
            button.colors = regularColor;
            _selected = false;
        }
    }
}
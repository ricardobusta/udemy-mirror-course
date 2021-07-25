using System;
using Networking;
using TMPro;
using UnityEngine;

namespace Menus
{
    public class UnitCountDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text textLabel;
        
        private void Start()
        {
            RtsPlayer.ClientOnUnitCountUpdated += UpdateUnitCount;
        }

        private void OnDestroy()
        {
            RtsPlayer.ClientOnUnitCountUpdated -= UpdateUnitCount;
        }

        private void UpdateUnitCount(int unitCount, int maxUnitCount)
        {
            textLabel.text = $"{unitCount}/{maxUnitCount}";
        }
    }
}
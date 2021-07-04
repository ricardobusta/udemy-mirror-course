using UnityEngine;
using UnityEngine.UI;

namespace Combat
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image bar;

        public void SetValue(float value)
        {
            bar.fillAmount = value;
        }
    }
}
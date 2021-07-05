using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Buildings
{
    public class ProgressCounter : MonoBehaviour
    {
        [SerializeField] private Image progressBar;
        [SerializeField] private TMP_Text counter;

        private float _targetValue;
        private float _fillVelocity;
        
        public void SetValue(float value)
        {
            _targetValue = value;
        }

        public void SetCounter(int count)
        {
            counter.text = count.ToString();
        }

        private void Update()
        {
            if (_targetValue < progressBar.fillAmount)
            {
                progressBar.fillAmount = _targetValue;
                return;
            }

            progressBar.fillAmount = Mathf.SmoothDamp(progressBar.fillAmount, _targetValue, ref _fillVelocity, 0.1f);
        }
    }
}
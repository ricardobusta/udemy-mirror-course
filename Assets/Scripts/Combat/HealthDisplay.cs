using UnityEngine;
using UnityEngine.EventSystems;

namespace Combat
{
    public class HealthDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Health health;
        [SerializeField] private HealthBar healthBar;

        private void Awake()
        {
            health.ClientOnHealthChanged += UpdateHealth;
           // healthBar.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            health.ClientOnHealthChanged -= UpdateHealth;
        }

        private void UpdateHealth(int currentHealth, int maxHealth)
        {
            healthBar.SetValue(currentHealth / (float) maxHealth);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // healthBar.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
           // healthBar.gameObject.SetActive(false);
        }
    }
}
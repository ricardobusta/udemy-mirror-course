using UnityEngine;
using UnityEngine.EventSystems;

namespace Combat
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private HealthBar healthBar;

        private void Start()
        {
            health.ClientOnHealthChanged += UpdateHealth;
        }

        private void OnDestroy()
        {
            health.ClientOnHealthChanged -= UpdateHealth;
        }

        private void UpdateHealth(int currentHealth, int maxHealth)
        {
            Debug.Log($"Setting health value {currentHealth}");
            healthBar.SetValue(currentHealth / (float) maxHealth);
        }
    }
}
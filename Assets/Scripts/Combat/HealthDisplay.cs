using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Combat
{
    public class HealthDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Health health;
        [SerializeField] private GameObject healthBarParent;
        [SerializeField] private Image healthBar;

        private Camera _mainCamera;
        
        private void Awake()
        {
            health.ClientOnHealthChanged += UpdateHealth;
            healthBarParent.SetActive(false);
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void OnDestroy()
        {
            health.ClientOnHealthChanged -= UpdateHealth;
        }

        private void UpdateHealth(int currentHealth, int maxHealth)
        {
            healthBar.fillAmount = currentHealth / (float) maxHealth;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            healthBarParent.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            healthBarParent.SetActive(false);
        }

        private void Update()
        {
            healthBarParent.transform.rotation = _mainCamera.transform.rotation;
        }
    }
}
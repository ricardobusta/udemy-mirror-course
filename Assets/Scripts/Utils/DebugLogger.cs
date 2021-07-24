using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Utils
{
    public class DebugLogger : MonoBehaviour
    {
        [SerializeField] private TMP_Text history;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Button clearButton;
        [SerializeField] private GameObject rootObject;

        private static DebugLogger singleton;

        private void Awake()
        {
            if (singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            singleton = this;

            DontDestroyOnLoad(gameObject);

            history.text = string.Empty;
            clearButton.onClick.AddListener(() => history.text = string.Empty);
            Application.logMessageReceived += OnLogMessage;

            rootObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnLogMessage;
        }

        private void OnLogMessage(string logString, string stackTrace, LogType type)
        {
            var logColor = type switch
            {
                LogType.Error => Color.red,
                LogType.Warning => Color.yellow,
                _ => Color.white
            };
            history.text += $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>{logString}</color>\n";
            if (gameObject.activeSelf)
            {
                StartCoroutine(ScrollDown());
            }
        }

        private IEnumerator ScrollDown()
        {
            // Wait UI to update. It takes 2 frames.
            yield return null;
            yield return null;
            scrollbar.value = 0;
        }

        private void Update()
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                rootObject.SetActive(!rootObject.activeSelf);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenuRuntime : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var menu = GameObject.Find("SettingsMenu");
        var settingsButton = GameObject.Find("Settings")?.GetComponent<Button>();
        var backButton = menu?.transform.Find("Back")?.GetComponent<Button>();

        if (menu == null || settingsButton == null || backButton == null)
            return;

        menu.SetActive(false);
        settingsButton.onClick.AddListener(() => menu.SetActive(true));
        backButton.onClick.AddListener(() => menu.SetActive(false));

        var buttons = menu.GetComponentsInChildren<Button>(true);
        if (buttons.Length >= 5)
        {
            buttons[0].onClick.AddListener(() => LocalizationManager.SetLanguage("tr"));
            buttons[1].onClick.AddListener(() => LocalizationManager.SetLanguage("en"));
            buttons[2].onClick.AddListener(() => LocalizationManager.SetLanguage("fr"));
            buttons[3].onClick.AddListener(() => LocalizationManager.SetLanguage("zh"));
        }
    }
}

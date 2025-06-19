using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    public string key;
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        LocalizationManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        if (text != null)
            text.text = LocalizationManager.Get(key);
    }
}

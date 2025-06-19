using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image cardBackground;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text leftValueText;
    public TMP_Text rightValueText;
    public Image typeIcon;

    private CardEntry data;

    private static readonly Dictionary<string, Sprite> iconCache = new();

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateLocalization;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateLocalization;
    }

    public void Initialize(CardEntry entry)
    {
        data = entry;

        UpdateLocalization();

        leftValueText.text = entry.leftValue.ToString();
        rightValueText.text = entry.rightValue.ToString();

        if (!iconCache.TryGetValue(entry.type, out Sprite icon))
        {
            icon = Resources.Load<Sprite>($"CardIcons/{entry.type}");
            if (icon != null)
                iconCache[entry.type] = icon;
        }

        if (icon != null)
        {
            typeIcon.sprite = icon;
            typeIcon.enabled = true;
        }
        else
        {
            Debug.LogWarning($"[CardDisplay] Icon not found for type: {entry.type}");
            typeIcon.enabled = false;
        }
    }

    private void UpdateLocalization()
    {
        if (data == null)
            return;

        nameText.text = LocalizationManager.Get(data.nameKey);
        descriptionText.text = LocalizationManager.Get(data.descriptionKey);
    }

    public CardEntry GetData() => data;
}

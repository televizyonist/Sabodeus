using TMPro;
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

    public void Initialize(CardEntry entry)
    {
        data = entry;

        // Localization
        nameText.text = LocalizationManager.Get(entry.nameKey);
        descriptionText.text = LocalizationManager.Get(entry.descriptionKey);

        leftValueText.text = entry.leftValue.ToString();
        rightValueText.text = entry.rightValue.ToString();

        Sprite icon = Resources.Load<Sprite>($"CardIcons/{entry.type}");
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

    public CardEntry GetData() => data;
}

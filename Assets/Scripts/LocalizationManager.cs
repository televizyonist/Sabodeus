using System.Collections.Generic;
using UnityEngine;

public static class LocalizationManager
{
    public static event System.Action OnLanguageChanged;

    private static string currentLanguage = "en";

    private static readonly Dictionary<string, Dictionary<string, string>> localization = new()
    {
        ["en"] = new Dictionary<string, string>
        {
            { "card.bridge.name", "Bridge" },
            { "card.bridge.desc", "Allows flexible placement." },
            { "card.nuclear.name", "Nuclear Plant" },
            { "card.nuclear.desc", "Destroy any completed pair." },
            { "card.factory.name", "Factory" },
            { "card.factory.desc", "Grants a skill card when completed." },
            { "card.castle.name", "Castle" },
            { "card.castle.desc", "Doubles score of structures between it and city center." },
            { "card.port.name", "Port" },
            { "card.port.desc", "Can be sabotaged without being completed." }
        },
        ["tr"] = new Dictionary<string, string>
        {
            { "card.bridge.name", "Köprü" },
            { "card.bridge.desc", "Esnek yerleşime izin verir." },
            { "card.nuclear.name", "Nükleer Santral" },
            { "card.nuclear.desc", "Tamamlanan herhangi bir çifti yok eder." },
            { "card.factory.name", "Fabrika" },
            { "card.factory.desc", "Tamamlandığında bir yetenek kartı verir." },
            { "card.castle.name", "Kale" },
            { "card.castle.desc", "Şehir merkezine kadar olan yapıları iki katı puanlar." },
            { "card.port.name", "Liman" },
            { "card.port.desc", "Tamamlanmadan sabotaj yapılabilir." }
        },
        ["fr"] = new Dictionary<string, string>
        {
            { "card.bridge.name", "Pont" },
            { "card.bridge.desc", "Permet une pose flexible." },
            { "card.nuclear.name", "Centrale Nucléaire" },
            { "card.nuclear.desc", "Détruit n'importe quelle paire complétée." },
            { "card.factory.name", "Usine" },
            { "card.factory.desc", "Donne une carte compétence une fois terminée." },
            { "card.castle.name", "Château" },
            { "card.castle.desc", "Double le score des structures jusqu'au centre-ville." },
            { "card.port.name", "Port" },
            { "card.port.desc", "Peut être saboté sans être complété." }
        },
        ["zh"] = new Dictionary<string, string>
        {
            { "card.bridge.name", "桥梁" },
            { "card.bridge.desc", "允许灵活放置。" },
            { "card.nuclear.name", "核电站" },
            { "card.nuclear.desc", "摧毁任何已完成的组合。" },
            { "card.factory.name", "工厂" },
            { "card.factory.desc", "完成时获得技能牌。" },
            { "card.castle.name", "城堡" },
            { "card.castle.desc", "使其与城市中心之间建筑得分翻倍。" },
            { "card.port.name", "港口" },
            { "card.port.desc", "未完成也可被破坏。" }
        }
    };

    public static void SetLanguage(string code)
    {
        if (localization.ContainsKey(code))
        {
            currentLanguage = code;
            OnLanguageChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Unsupported language: {code}");
        }
    }

    public static string Get(string key)
    {
        if (localization.TryGetValue(currentLanguage, out var dict) && dict.TryGetValue(key, out var value))
            return value;

        Debug.LogWarning("Localization key not found: " + key);
        return key; // fallback
    }
}

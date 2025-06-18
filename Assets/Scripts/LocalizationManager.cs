using System.Collections.Generic;
using UnityEngine;

public static class LocalizationManager
{
    // Test amaçlý yerelleþtirme sözlüðü
    private static Dictionary<string, string> dummyLocalization = new Dictionary<string, string>()
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
    };

    public static string Get(string key)
    {
        if (dummyLocalization.ContainsKey(key))
            return dummyLocalization[key];

        Debug.LogWarning("Localization key not found: " + key);
        return key; // fallback
    }
}

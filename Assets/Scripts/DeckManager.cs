using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform spawnPoint;
    public Transform handAreaTransform;

    public List<CardEntry> fullDeck = new();
    private Stack<CardEntry> drawPile = new();

    public void LoadDeckFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("sabodeus_card_data");
        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found in Resources folder!");
            return;
        }

        CardList loaded = JsonUtility.FromJson<CardList>(jsonFile.text);
        fullDeck = loaded.cards;
        Debug.Log("Loaded " + fullDeck.Count + " cards from JSON.");
    }

    public void InitializeDrawPile()
    {
        if (fullDeck == null || fullDeck.Count == 0)
        {
            Debug.LogError("Deck not loaded. Call LoadDeckFromJson() first.");
            return;
        }

        List<CardEntry> shuffled = fullDeck.OrderBy(x => Random.value).ToList();
        drawPile = new Stack<CardEntry>(shuffled);
        Debug.Log("Draw pile initialized with " + drawPile.Count + " cards.");
    }

    public CardEntry DrawCard()
    {
        if (drawPile.Count == 0)
        {
            Debug.LogWarning("Draw pile is empty!");
            return null;
        }
        return drawPile.Pop();
    }

    public int RemainingCards => drawPile.Count;

    public CardEntry SpawnCardToHand()
    {
        if (handAreaTransform.childCount >= 4)
        {
            Debug.Log("Hand is full. Max 4 cards allowed.");
            return null;
        }

        var cardData = DrawCard();
        if (cardData == null)
        {
            Debug.LogWarning("No card to draw.");
            return null;
        }

        GameObject cardObj = Instantiate(cardPrefab, handAreaTransform);
        cardObj.transform.localScale = Vector3.one;
        cardObj.transform.localPosition = Vector3.zero;

        var display = cardObj.GetComponent<CardDisplay>();
        if (display != null)
            display.Initialize(cardData);
        else
            Debug.LogWarning("CardDisplay script not found on prefab.");

        // 🔄 Fan stili dizilim için doğru scripti çağır
        var layout = handAreaTransform.GetComponent<HandLayoutFanStyle>();
        if (layout != null)
            layout.UpdateLayout();

        return cardData;
    }

    private void Start()
    {
        LoadDeckFromJson();

        TextAsset jsonFile = Resources.Load<TextAsset>("sabodeus_card_data");
        Debug.Log("Yüklenen JSON içeriği:\n" + jsonFile.text);

        InitializeDrawPile();

        var cardData = SpawnCardToHand();
        if (cardData != null)
        {
            Debug.Log("Kart Eline Geldi: " + cardData.id + " → " + cardData.leftValue + "-" + cardData.rightValue);
        }
    }
}

[System.Serializable]
public class CardList
{
    public List<CardEntry> cards;
}

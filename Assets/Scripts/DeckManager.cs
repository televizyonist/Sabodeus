using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform spawnPoint;
    public Transform handAreaTransform;

    [SerializeField] private TextAsset cardDataAsset;

    public List<CardEntry> fullDeck = new();
    private Stack<CardEntry> drawPile = new();
    private HandLayoutFanStyle handLayout;

    public void LoadDeckFromJson()
    {
        TextAsset jsonFile = cardDataAsset;
        if (jsonFile == null)
            jsonFile = Resources.Load<TextAsset>("sabodeus_card_data");

        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found!");
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

        List<CardEntry> shuffled = new(fullDeck);
        Shuffle(shuffled);
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
        // Reduce card size to one third of the previous scale
        cardObj.transform.localScale = Vector3.one * (0.8f / 3f);
        cardObj.transform.localPosition = Vector3.zero;

        var display = cardObj.GetComponent<CardDisplay>();
        if (display != null)
            display.Initialize(cardData);
        else
            Debug.LogWarning("CardDisplay script not found on prefab.");

        // 🔄 Fan stili dizilim için doğru scripti çağır
        if (handLayout != null)
            handLayout.UpdateLayout();

        return cardData;
    }

    private void Start()
    {
        handLayout = handAreaTransform.GetComponent<HandLayoutFanStyle>();
        LoadDeckFromJson();
        InitializeDrawPile();

        var cardData = SpawnCardToHand();
        if (cardData != null)
        {
            Debug.Log("Kart Eline Geldi: " + cardData.id + " → " + cardData.leftValue + "-" + cardData.rightValue);
        }
    }

    private static readonly System.Random rng = new();

    private static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public void ReplenishDeck(IEnumerable<CardEntry> cards)
    {
        if (cards == null)
            return;
        List<CardEntry> list = new(cards);
        if (list.Count == 0)
            return;
        Shuffle(list);
        drawPile = new Stack<CardEntry>(list);
        Debug.Log($"Draw pile replenished with {drawPile.Count} cards from graveyard.");
    }
}

[System.Serializable]
public class CardList
{
    public List<CardEntry> cards;
}

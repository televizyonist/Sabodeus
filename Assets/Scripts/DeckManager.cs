using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform spawnPoint;
    public Transform handAreaTransform;
    public Sprite cardBackSprite;
    public float dealSpeed = 8f;

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

    private IEnumerator SpawnCardToHandAnimatedRoutine(CardEntry cardData, Transform startPoint)
    {
        if (handAreaTransform.childCount >= 4 || cardData == null)
            yield break;

        if (startPoint == null)
            startPoint = spawnPoint;

        GameObject cardObj = Instantiate(cardPrefab, startPoint.position, Quaternion.identity, startPoint.parent);
        cardObj.transform.localScale = Vector3.one * (0.8f / 3f);

        var display = cardObj.GetComponent<CardDisplay>();
        if (display != null)
        {
            if (cardBackSprite != null)
                display.cardBackground.sprite = cardBackSprite;
            display.nameText.enabled = false;
            display.descriptionText.enabled = false;
            display.leftValueText.enabled = false;
            display.rightValueText.enabled = false;
            display.typeIcon.enabled = false;
        }

        Vector3 startPos = startPoint.position;
        Vector3 endPos = handAreaTransform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * dealSpeed;
            cardObj.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        cardObj.transform.SetParent(handAreaTransform, true);
        cardObj.transform.localPosition = Vector3.zero;

        if (display != null)
        {
            display.Initialize(cardData);
            display.nameText.enabled = true;
            display.descriptionText.enabled = true;
            display.leftValueText.enabled = true;
            display.rightValueText.enabled = true;
            display.typeIcon.enabled = true;
        }

        if (handLayout != null)
            handLayout.UpdateLayout();
    }

    public CardEntry SpawnCardToHandAnimated(Transform startPoint)
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

        StartCoroutine(SpawnCardToHandAnimatedRoutine(cardData, startPoint));
        return cardData;
    }

    private IEnumerator SpawnCardToHandAnimated(CardEntry cardData)
    {
        return SpawnCardToHandAnimatedRoutine(cardData, spawnPoint);
    }

    private IEnumerator DealStartingHand(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var cardData = DrawCard();
            if (cardData == null)
                yield break;

            yield return SpawnCardToHandAnimated(cardData);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Start()
    {
        handLayout = handAreaTransform.GetComponent<HandLayoutFanStyle>();
        LoadDeckFromJson();
        InitializeDrawPile();

        StartCoroutine(DealStartingHand(4));
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

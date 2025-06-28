using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraveyardVisual : MonoBehaviour
{
    public TMP_Text countText;
    private readonly List<CardEntry> _cards = new();

    public void AddCard(GameObject card)
    {
        if (card == null) return;
        var disp = card.GetComponent<CardDisplay>();
        if (disp != null)
            _cards.Add(disp.GetData());
        card.transform.SetParent(null); // remove from hand before destroying
        Destroy(card);
        UpdateCountText();
        Debug.Log($"Added card to graveyard: {_cards.Count} total");
    }

    public List<CardEntry> ClearCards()
    {
        List<CardEntry> entries = new(_cards);
        _cards.Clear();
        UpdateCountText();
        return entries;
    }

    private void Start()
    {
        UpdateCountText();
    }

    private void UpdateCountText()
    {
        if (countText != null)
            countText.text = _cards.Count.ToString();
    }
}

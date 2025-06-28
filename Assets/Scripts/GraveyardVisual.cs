using System.Collections.Generic;
using UnityEngine;

public class GraveyardVisual : MonoBehaviour
{
    public float stackOffset = 0.01f;
    private readonly List<GameObject> _cards = new();

    public void AddCard(GameObject card)
    {
        if (card == null) return;
        _cards.Add(card);
        card.transform.SetParent(transform, true);
        card.transform.localRotation = Quaternion.identity;
        card.transform.localPosition = new Vector3(0, 0, -stackOffset * _cards.Count);
        var drag = card.GetComponent<CardDraggable>();
        if (drag != null)
            Destroy(drag);
    }

    public List<CardEntry> ClearCards()
    {
        List<CardEntry> entries = new();
        foreach (var c in _cards)
        {
            var disp = c.GetComponent<CardDisplay>();
            if (disp != null)
                entries.Add(disp.GetData());
            Destroy(c);
        }
        _cards.Clear();
        return entries;
    }
}

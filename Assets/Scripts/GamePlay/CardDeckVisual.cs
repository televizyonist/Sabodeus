using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDeckVisual : NetworkBehaviour, IPointerClickHandler
{
    public DeckManager deckManager;
    public GraveyardVisual graveyard;
    public TMP_Text countText;

    private void Start()
    {
        UpdateCountText();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (deckManager == null)
            return;

        if (deckManager.RemainingCards == 0 && graveyard != null)
        {
            var entries = graveyard.ClearCards();
            if (entries.Count > 0)
                deckManager.ReplenishDeck(entries);
        }

        var card = deckManager.SpawnCardToHandAnimated(transform);
        Debug.Log("Kart ekildi: " + card?.id);
        UpdateCountText();
    }

    private void UpdateCountText()
    {
        if (countText != null && deckManager != null)
            countText.text = $"{deckManager.RemainingCards}/{deckManager.fullDeck.Count}";
    }
}

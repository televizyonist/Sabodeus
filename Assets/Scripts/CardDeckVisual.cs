using UnityEngine;
using UnityEngine.EventSystems;

public class CardDeckVisual : MonoBehaviour, IPointerClickHandler
{
    public DeckManager deckManager;
    public GraveyardVisual graveyard;

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

        var card = deckManager.SpawnCardToHand();
        Debug.Log("Kart ekildi: " + card?.id);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class CardDeckVisual : MonoBehaviour, IPointerClickHandler
{
    public DeckManager deckManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (deckManager != null)
        {
            var card = deckManager.SpawnCardToHand();
            Debug.Log("Kart çekildi: " + card?.id);
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class SlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsRightSide { get; private set; }
    public int Index { get; private set; }
    public SpriteRenderer spriteRenderer;

    private CityAreaManager manager;
    private Color originalColor;
    public void Initialize(CityAreaManager mgr, bool rightSide, int index, SpriteRenderer img)
        spriteRenderer = img;
        originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
        if (isOccupied || spriteRenderer == null) return;
        spriteRenderer.color = highlightColor;
        if (spriteRenderer == null) return;
        spriteRenderer.color = originalColor;
        originalColor = image != null ? image.color : Color.white;
    }

    public void AssignCard(GameObject card)
    {
        if (card == null) return;

        isOccupied = true;
        CurrentCard = card;
        if (manager != null)
        {
            manager.OnSlotFilled(this);
        }
    }

    public void ClearSlot()
    {
        isOccupied = false;
        CurrentCard = null;
        if (image != null)
            image.color = originalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isOccupied || image == null) return;
        image.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image == null) return;
        image.color = originalColor;
    }
}

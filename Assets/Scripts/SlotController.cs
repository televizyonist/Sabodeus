using UnityEngine;
using UnityEngine.EventSystems;

public class SlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsRightSide { get; private set; }
    public int Index { get; private set; }

    public bool isOccupied = false;
    public GameObject CurrentCard;

    private CityAreaManager manager;

    [Header("Border Colors")]
    public Color allowedColor = Color.green;
    public Color forbiddenColor = Color.red;
    private Renderer slotRenderer;

    public string AllowedType { get; private set; } = null;

    public void SetAllowedType(string type)
    {
        AllowedType = type;
    }

    public void Initialize(CityAreaManager mgr, bool rightSide, int index, Renderer rend)
    {
        manager = mgr;
        IsRightSide = rightSide;
        Index = index;

        slotRenderer = rend;

        if (slotRenderer != null)
        {
            slotRenderer.enabled = false;
        }
    }

    public bool CanPlaceCard(GameObject card)
    {
        if (isOccupied) return false;
        return manager == null || manager.CanPlaceCard(this, card);
    }

    public void AssignCard(GameObject card)
    {
        if (card == null) return;

        isOccupied = true;
        CurrentCard = card;

        // Snap to slot position
        card.transform.position = transform.position;
        card.transform.rotation = transform.rotation;

        manager?.OnSlotFilled(this);
    }

    public void ClearSlot()
    {
        isOccupied = false;
        CurrentCard = null;

        if (slotRenderer != null)
        {
            slotRenderer.enabled = false;
        }

        manager?.UpdateScoreDisplay();
    }

    public void OnPointerEnter(PointerEventData eventData) {}

    public void OnPointerExit(PointerEventData eventData) {}

    public void ShowBorder(GameObject card)
    {
        if (slotRenderer == null)
            return;

        if (isOccupied)
        {
            slotRenderer.enabled = false;
            return;
        }

        if (card == null)
        {
            slotRenderer.enabled = false;
            return;
        }

        slotRenderer.enabled = true;
        slotRenderer.material.color = CanPlaceCard(card) ? allowedColor : forbiddenColor;
    }
}

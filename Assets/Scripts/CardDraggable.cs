using UnityEngine;
using UnityEngine.EventSystems;

public class CardDraggable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static GameObject HoveredCard { get; private set; }
    public static GameObject DraggedCard { get; private set; }
    private Vector3 offset;
    private float dragDistance;
    private Camera cam;
    private bool isDragging = false;
    private Vector3 originalPos;
    private Transform originalParent;
    private HandLayoutFanStyle layout;
    private Vector3 previousPos;
    private CanvasGroup canvasGroup;
    private Canvas cardCanvas;
    private int originalSortingOrder;
    private Collider cardCollider;
    public float rotationMultiplier = 30f;
    public float hoverHeight = 0.5f;
    public float hoverForward = 2f;
    private bool isHovered = false;

    void Start()
    {
        cam = Camera.main;
        layout = transform.parent?.GetComponent<HandLayoutFanStyle>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        cardCollider = GetComponent<Collider>();
        cardCanvas = GetComponentInChildren<Canvas>();
        if (cardCanvas != null)
        {
            originalSortingOrder = cardCanvas.sortingOrder;
            if (CityAreaManager.Instance != null)
                cardCanvas.sortingOrder = CityAreaManager.Instance.baseSortingOrder + 2;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging || isHovered) return;
        isHovered = true;
        layout?.SpreadOut();
        CardPreviewManager.Instance?.ShowPreview(gameObject);
        HoveredCard = gameObject;
        CityAreaManager.Instance?.ShowSlotBorders(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered || isDragging) return;
        isHovered = false;
        layout?.Restore();
        CardPreviewManager.Instance?.HidePreview();
        HoveredCard = null;
        CityAreaManager.Instance?.ShowSlotBorders(null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        isHovered = false;
        CardPreviewManager.Instance?.HidePreview();
        originalParent = transform.parent;

        // If this card was sitting in a slot, clear that slot so it can accept
        // a new card while we drag this one around.
        SlotController parentSlot = originalParent != null
            ? originalParent.GetComponent<SlotController>()
            : null;
        if (parentSlot != null)
        {
            parentSlot.ClearSlot();
            transform.SetParent(null, true);
        }
        else if (layout != null && originalParent == layout.transform)
        {
            transform.SetParent(null, true);
            layout.UpdateLayout();
        }

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        if (cardCollider != null)
            cardCollider.enabled = false;

        dragDistance = cam.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDistance));
        offset = transform.position - mouseWorld;
        previousPos = transform.position;

        DraggedCard = gameObject;
        CityAreaManager.Instance?.ShowSlotBorders(gameObject);

        if (cardCanvas != null && CityAreaManager.Instance != null)
            cardCanvas.sortingOrder = CityAreaManager.Instance.baseSortingOrder + 2;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDistance));
        Vector3 target = mouseWorld + offset;
        Vector3 delta = target - previousPos;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg * rotationMultiplier;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 10f);
        transform.position = target;
        previousPos = target;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        isHovered = false;
        transform.rotation = Quaternion.identity;
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
        if (cardCollider != null)
            cardCollider.enabled = true;

        // 💡 Daha sağlam yöntem: pointerEnter üzerinden slotı bul
        if (eventData.pointerEnter != null)
        {
            SlotController slot = eventData.pointerEnter.GetComponent<SlotController>();
            if (slot != null && slot.CanPlaceCard(gameObject))
            {
                Debug.Log($"Card dropped on slot {slot.name}");
                transform.SetParent(slot.transform, true);
                transform.localPosition = Vector3.zero;
                slot.AssignCard(gameObject);
                layout?.UpdateLayout();
                CardPreviewManager.Instance?.HidePreview();
                CityAreaManager.Instance?.ShowSlotBorders(null);
                DraggedCard = null;
                if (cardCanvas != null && CityAreaManager.Instance != null)
                    cardCanvas.sortingOrder = CityAreaManager.Instance.baseSortingOrder - slot.Index;
                return;
            }

            GraveyardVisual gy = eventData.pointerEnter.GetComponent<GraveyardVisual>();
            if (gy == null)
                gy = eventData.pointerEnter.GetComponentInParent<GraveyardVisual>();
            if (gy != null)
            {
                Debug.Log("Card dropped on graveyard");
                gy.AddCard(gameObject);
                layout?.UpdateLayout();
                CardPreviewManager.Instance?.HidePreview();
                CityAreaManager.Instance?.ShowSlotBorders(null);
                DraggedCard = null;
                return;
            }
            else
            {
                Debug.Log($"No valid drop target for {gameObject.name} - pointer over {eventData.pointerEnter.name}");
            }
        }
        else
        {
            Debug.Log($"Pointer up but pointerEnter was null for {gameObject.name}");
        }

        // Uygun slot yok → eski konuma dön
        transform.SetParent(originalParent, true);
        transform.localPosition = Vector3.zero;
        layout?.UpdateLayout();
        CardPreviewManager.Instance?.HidePreview();
        CityAreaManager.Instance?.ShowSlotBorders(null);
        DraggedCard = null;
        if (cardCanvas != null)
            cardCanvas.sortingOrder = originalSortingOrder;
    }
}

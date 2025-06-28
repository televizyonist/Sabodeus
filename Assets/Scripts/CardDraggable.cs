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
    private Vector3 originalLocalPos;
    private int originalSiblingIndex;
    private Transform originalParent;
    private bool hasDragged = false;
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

    private void SetSiblingOpacity(float alpha, bool blockRaycast)
    {
        Transform parent = transform.parent;
        if (parent == null) return;

        foreach (Transform child in parent)
        {
            if (child == transform) continue;
            CanvasGroup cg = child.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = child.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = alpha;
            cg.blocksRaycasts = blockRaycast;
        }
    }

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
        SetSiblingOpacity(0.5f, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered || isDragging) return;
        isHovered = false;
        layout?.Restore();
        CardPreviewManager.Instance?.HidePreview();
        HoveredCard = null;
        CityAreaManager.Instance?.ShowSlotBorders(null);
        SetSiblingOpacity(1f, true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        isHovered = false;
        CardPreviewManager.Instance?.HidePreview();
        SetSiblingOpacity(1f, true);

        originalParent = transform.parent;
        originalLocalPos = transform.localPosition;
        originalSiblingIndex = transform.GetSiblingIndex();
        hasDragged = false;

        dragDistance = cam.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDistance));
        offset = transform.position - mouseWorld;
        previousPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        if (!hasDragged)
        {
            hasDragged = true;

            // If this card was sitting in a slot, clear it first
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

            DraggedCard = gameObject;
            CityAreaManager.Instance?.ShowSlotBorders(gameObject);

            if (cardCanvas != null && CityAreaManager.Instance != null)
                cardCanvas.sortingOrder = CityAreaManager.Instance.baseSortingOrder + 2;
        }

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

        SetSiblingOpacity(1f, true);

        if (!hasDragged)
        {
            CardPreviewManager.Instance?.HidePreview();
            CityAreaManager.Instance?.ShowSlotBorders(null);
            DraggedCard = null;
            if (cardCanvas != null)
                cardCanvas.sortingOrder = originalSortingOrder;
            return;
        }

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
                hasDragged = false;
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
                hasDragged = false;
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
        transform.SetSiblingIndex(originalSiblingIndex);
        transform.localPosition = originalLocalPos;
        layout?.UpdateLayout();
        CardPreviewManager.Instance?.HidePreview();
        CityAreaManager.Instance?.ShowSlotBorders(null);
        DraggedCard = null;
        if (cardCanvas != null)
            cardCanvas.sortingOrder = originalSortingOrder;
        hasDragged = false;
    }
}

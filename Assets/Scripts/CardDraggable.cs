using UnityEngine;
using UnityEngine.EventSystems;

public class CardDraggable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 offset;
    private float dragDistance;
    private Camera cam;
    private bool isDragging = false;
    private Vector3 originalPos;
    private Transform originalParent;
    private HandLayoutFanStyle layout;
    private Vector3 previousPos;
    private CanvasGroup canvasGroup;
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
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging || isHovered) return;
        isHovered = true;
        layout?.SpreadOut();
        CardPreviewManager.Instance?.ShowPreview(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered || isDragging) return;
        isHovered = false;
        layout?.Restore();
        CardPreviewManager.Instance?.HidePreview();
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

        dragDistance = cam.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDistance));
        offset = transform.position - mouseWorld;
        previousPos = transform.position;
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

        // 💡 Daha sağlam yöntem: pointerEnter üzerinden slotı bul
        if (eventData.pointerEnter != null)
        {
            SlotController slot = eventData.pointerEnter.GetComponent<SlotController>();
            if (slot != null && !slot.isOccupied)
            {
                transform.SetParent(slot.transform, true);
                transform.localPosition = Vector3.zero;
                slot.AssignCard(gameObject);
                layout?.UpdateLayout();
                CardPreviewManager.Instance?.HidePreview();
                return;
            }
        }

        // Uygun slot yok → eski konuma dön
        transform.SetParent(originalParent, true);
        transform.localPosition = Vector3.zero;
        layout?.UpdateLayout();
        CardPreviewManager.Instance?.HidePreview();
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class CardDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private SlotController previousSlot;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;

        if (previousSlot != null)
        {
            previousSlot.ClearSlot();
            previousSlot = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Raycast ile slot bul
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            SlotController slot = hit.collider.GetComponent<SlotController>();
            if (slot != null && !slot.isOccupied)
            {
                transform.position = slot.transform.position;
                transform.SetParent(slot.transform);
                slot.AssignCard(gameObject);
                previousSlot = slot;
                return;
            }
        }

        // Snap başarısızsa geri dön
        transform.position = originalPosition;
        transform.SetParent(originalParent);
    }
}

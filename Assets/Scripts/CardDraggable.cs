using UnityEngine;
using UnityEngine.EventSystems;

public class CardDraggable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 offset;
    private Camera cam;
    private bool isDragging = false;
    private Vector3 originalPos;
    private HandLayoutFanStyle layout;
    private Vector3 previousPos;
    public float rotationMultiplier = 30f;
    public float hoverHeight = 0.5f;
    public float hoverForward = 2f;
    private bool isHovered = false;

    void Start()
    {
        cam = Camera.main;
        layout = transform.parent?.GetComponent<HandLayoutFanStyle>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;
        originalPos = transform.localPosition;
        Vector3 pos = originalPos;
        pos.y += 0.5f;
        pos.z = 0.5f;
        transform.localPosition = pos;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;
        transform.localPosition = originalPos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);
        previousPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 target = new Vector3(mouseWorld.x + offset.x, mouseWorld.y + offset.y, transform.position.z);
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
        layout?.UpdateLayout();
    }
}

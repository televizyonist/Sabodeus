using UnityEngine;

public class CardDraggable : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private bool isDragging = false;
    private float originalZ;
    private HandLayoutFanStyle layout;
    private Vector3 previousPos;
    public float rotationMultiplier = 30f;

    void Start()
    {
        cam = Camera.main;
        layout = transform.parent?.GetComponent<HandLayoutFanStyle>();
    }

    void OnMouseEnter()
    {
        if (isDragging) return;
        originalZ = transform.localPosition.z;
        Vector3 pos = transform.localPosition;
        pos.z = 0.1f;
        transform.localPosition = pos;
    }

    void OnMouseExit()
    {
        if (isDragging) return;
        Vector3 pos = transform.localPosition;
        pos.z = originalZ;
        transform.localPosition = pos;
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);
        previousPos = transform.position;
    }

    void OnMouseDrag()
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

    void OnMouseUp()
    {
        isDragging = false;
        transform.rotation = Quaternion.identity;
        layout?.UpdateLayout();
    }
}
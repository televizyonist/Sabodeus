using UnityEngine;

public class CardDraggable : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 originalPosition;
    private Camera cam;

    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;
        originalPosition = transform.position;
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouseWorld.x + offset.x, mouseWorld.y + offset.y, transform.position.z);
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Ýleride: Slot kontrolü buraya gelecek
        transform.position = originalPosition;
    }
}

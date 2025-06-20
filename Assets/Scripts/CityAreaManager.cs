using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityAreaManager : MonoBehaviour
{
    public Sprite cityCenterSprite;
    public Sprite slotSprite;
    public float horizontalSpacing = 0.7f;
    public float verticalOffset = -0.15f;
    public Vector2 elementSize = new Vector2(0.6f, 0.9f); // Hedef boyut (2:3 oran)
    public int baseSortingOrder = 100;

    public Transform slotParent;

    private readonly List<SlotController> rightSlots = new();
    private readonly List<SlotController> leftSlots = new();

    private void Awake()
    {
        EnsureRaycaster();
        BuildLayout();
    }

    private void EnsureRaycaster()
    {
        Camera cam = Camera.main;
        if (cam != null && cam.GetComponent<PhysicsRaycaster>() == null)
        {
            cam.gameObject.AddComponent<PhysicsRaycaster>();
        }
    }

    private void BuildLayout()
    {
        if (slotParent == null)
        {
            Debug.LogError("CityAreaManager: slotParent atanmamış!");
            return;
        }

        GameObject center = CreateSpriteElement("CityCenter", cityCenterSprite, baseSortingOrder + 1, out _);
        center.transform.SetParent(slotParent, false);
        center.transform.localPosition = Vector3.zero;

        for (int i = 0; i < 5; i++)
        {
            SlotController right = CreateSlot(true, i);
            rightSlots.Add(right);
            SlotController left = CreateSlot(false, i);
            leftSlots.Add(left);

            if (i > 0)
            {
                right.gameObject.SetActive(false);
                left.gameObject.SetActive(false);
            }
        }

        UpdateSlotPositions();
    }

    private GameObject CreateSpriteElement(string name, Sprite sprite, int order, out Vector2 worldSize)
    {
        GameObject obj = new GameObject(name);
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = order;

        worldSize = Vector2.one;

        if (sprite != null)
        {
            float pixelWidth = sprite.rect.width / sprite.pixelsPerUnit;
            float pixelHeight = sprite.rect.height / sprite.pixelsPerUnit;

            float scaleX = elementSize.x / pixelWidth;
            float scaleY = elementSize.y / pixelHeight;
            float uniformScale = Mathf.Min(scaleX, scaleY);

            obj.transform.localScale = new Vector3(uniformScale, uniformScale, 1f);

            // Gerçek dünya boyutunu hesapla
            worldSize = new Vector2(pixelWidth * uniformScale, pixelHeight * uniformScale);
        }

        return obj;
    }

    private SlotController CreateSlot(bool rightSide, int index)
    {
        string n = (rightSide ? "RightSlot_" : "LeftSlot_") + index;

        Vector2 worldSize;
        GameObject obj = CreateSpriteElement(n, slotSprite, baseSortingOrder - index, out worldSize);
        obj.transform.SetParent(slotParent, false);

        BoxCollider col = obj.AddComponent<BoxCollider>();
        col.size = new Vector3(worldSize.x, worldSize.y, 0.1f);
        col.center = Vector3.zero;

        SlotController ctrl = obj.AddComponent<SlotController>();
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        ctrl.Initialize(this, rightSide, index, sr);
        return ctrl;
    }

    private void UpdateSlotPositions()
    {
        for (int i = 0; i < leftSlots.Count; i++)
        {
            float x = -(i + 1) * horizontalSpacing;
            float y = (i + 1) * verticalOffset;
            leftSlots[i].transform.localPosition = new Vector3(x, y, 0);
        }

        for (int i = 0; i < rightSlots.Count; i++)
        {
            float x = (i + 1) * horizontalSpacing;
            float y = (i + 1) * verticalOffset;
            rightSlots[i].transform.localPosition = new Vector3(x, y, 0);
        }
    }

    public void OnSlotFilled(SlotController slot)
    {
        List<SlotController> list = slot.IsRightSide ? rightSlots : leftSlots;
        int next = slot.Index + 1;
        if (next < list.Count)
        {
            list[next].gameObject.SetActive(true);
        }
    }
}

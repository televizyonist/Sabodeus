using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CityAreaManager : MonoBehaviour
{
    public Sprite cityCenterSprite;
    public Sprite slotSprite;
    public float horizontalSpacing = 0.7f;
    public float verticalOffset = -0.15f;
    public Vector2 elementSize = new Vector2(0.6f, 0.9f); // 2:3 oran
    public int baseSortingOrder = 100;

    public Transform slotParent; // Parent transform for slots

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
        if (cam != null && cam.GetComponent<UnityEngine.EventSystems.PhysicsRaycaster>() == null)
        {
            cam.gameObject.AddComponent<UnityEngine.EventSystems.PhysicsRaycaster>();
        }
    }

    private void BuildLayout()
    {
        if (slotParent == null)
        {
            Debug.LogError("CityAreaManager: slotParent atanmamış!");
            return;
        }

        // Şehir merkezi oluştur
        GameObject center = CreateSpriteElement("CityCenter", cityCenterSprite, baseSortingOrder + 1);
        center.transform.SetParent(slotParent, false);
        center.transform.localPosition = Vector3.zero;
        center.transform.localScale = new Vector3(elementSize.x, elementSize.y, 1f);

        // Slotları oluştur
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

    private GameObject CreateSpriteElement(string name, Sprite sprite, int order)
    {
        GameObject obj = new GameObject(name);
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = order;
        return obj;
    }

    private SlotController CreateSlot(bool rightSide, int index)
    {
        string n = (rightSide ? "RightSlot_" : "LeftSlot_") + index;
        GameObject obj = CreateSpriteElement(n, slotSprite, baseSortingOrder - index);
        obj.transform.SetParent(slotParent, false);
        obj.transform.localScale = new Vector3(elementSize.x, elementSize.y, 1f);

        BoxCollider col = obj.AddComponent<BoxCollider>();
        col.size = new Vector3(1f, 1f, 0.1f);

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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityAreaManager : MonoBehaviour
{
    public Sprite cityCenterSprite;
    public Sprite slotSprite;
    public float horizontalSpacing = 0.7f;
    public float verticalOffset = -0.15f;
    public Vector2 elementSize = new Vector2(0.6f, 0.9f); // 2:3 oran
    public int baseSortingOrder = 100;

    public Transform slotParent; // World Space Canvas altına atanacak parent

    private readonly List<SlotController> rightSlots = new();
    private readonly List<SlotController> leftSlots = new();

    private void Awake()
    {
        BuildLayout();
    }

    private void BuildLayout()
    {
        if (slotParent == null)
        {
            Debug.LogError("CityAreaManager: slotParent atanmamış!");
            return;
        }

        // Şehir merkezi oluştur
        GameObject center = CreateUIElement("CityCenter", cityCenterSprite, baseSortingOrder + 1);
        center.transform.SetParent(slotParent, false);
        center.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        center.GetComponent<RectTransform>().sizeDelta = elementSize;

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

    private GameObject CreateUIElement(string name, Sprite sprite, int order)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = elementSize;
        Image img = obj.GetComponent<Image>();
        img.sprite = sprite;
        img.raycastTarget = true;
        img.preserveAspect = true;

        Canvas objCanvas = obj.AddComponent<Canvas>();
        objCanvas.overrideSorting = true;
        objCanvas.sortingOrder = order;
        objCanvas.renderMode = RenderMode.WorldSpace;

        return obj;
    }

    private SlotController CreateSlot(bool rightSide, int index)
    {
        string n = (rightSide ? "RightSlot_" : "LeftSlot_") + index;
        GameObject obj = CreateUIElement(n, slotSprite, baseSortingOrder - index);
        obj.transform.SetParent(slotParent, false);

        SlotController ctrl = obj.AddComponent<SlotController>();
        Image img = obj.GetComponent<Image>();
        ctrl.Initialize(this, rightSide, index, img);
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

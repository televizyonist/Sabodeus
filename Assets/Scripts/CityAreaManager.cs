using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityAreaManager : MonoBehaviour
{
    public Sprite cityCenterSprite;
    public Sprite slotSprite;
    public float horizontalSpacing = 1.2f;
    public float verticalOffset = -0.2f;
    public int baseSortingOrder = 10;

    private readonly List<SlotController> rightSlots = new();
    private readonly List<SlotController> leftSlots = new();

    private void Awake()
    {
        BuildLayout();
    }

    private void BuildLayout()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
        }

        if (GetComponent<CanvasScaler>() == null)
            gameObject.AddComponent<CanvasScaler>();
        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        GameObject center = CreateUIElement("CityCenter", cityCenterSprite, baseSortingOrder + 1);
        center.transform.SetParent(transform, false);

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
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasRenderer), typeof(Image));
        Canvas c = obj.GetComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;
        c.overrideSorting = true;
        c.sortingOrder = order;
        Image img = obj.GetComponent<Image>();
        img.sprite = sprite;
        return obj;
    }

    private SlotController CreateSlot(bool rightSide, int index)
    {
        string n = (rightSide ? "RightSlot_" : "LeftSlot_") + index;
        GameObject obj = CreateUIElement(n, slotSprite, baseSortingOrder - index);
        obj.transform.SetParent(transform, false);

        SlotController ctrl = obj.AddComponent<SlotController>();
        ctrl.Initialize(this, rightSide, index);
        ctrl.image = obj.GetComponent<Image>();
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

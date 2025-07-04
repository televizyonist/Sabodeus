using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityAreaManager : NetworkBehaviour
{
    public static List<CityAreaManager> Managers { get; } = new();
    public static CityAreaManager Instance => Managers.Count > 0 ? Managers[0] : null;

    public static CityAreaManager GetManager(int id)
    {
        return Managers.Find(m => m.playerId == id);
    }

    [Tooltip("Owner player id for this city area")]
    public int playerId = 0;

    [Header("Görsel ve Slotlar")]
    public Sprite cityCenterSprite;
    public Sprite slotSprite;
    public Transform slotParent;
    public TMP_Text scoreText;

    [Header("Yerleşim Ayarları")]
    public float horizontalSpacing = 0.7f;
    public float verticalOffset = -0.15f;
    public Vector2 elementSize = new Vector2(0.6f, 0.9f);
    public int baseSortingOrder = 100;

    private readonly List<SlotController> rightSlots = new();
    private readonly List<SlotController> leftSlots = new();
    private readonly HashSet<string> bannedTypes = new();

    private void Awake()
    {
        if (!Managers.Contains(this))
            Managers.Add(this);

        EnsureRaycaster();
        BuildLayout();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Managers.Remove(this);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        UpdateScoreDisplay();
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
            slotParent = transform;
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

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        BoxCollider col = obj.AddComponent<BoxCollider>();
        if (sr != null && sr.sprite != null)
        {
            Vector3 size = sr.sprite.bounds.size;
            Vector3 center = sr.sprite.bounds.center;
            col.size = new Vector3(size.x, size.y, 0.1f);
            col.center = center;
        }
        else
        {
            col.size = new Vector3(worldSize.x, worldSize.y, 0.1f);
            col.center = Vector3.zero;
        }

        SlotController ctrl = obj.AddComponent<SlotController>();
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

    public void ShowSlotBorders(GameObject card)
    {
        foreach (var slot in leftSlots)
            slot.ShowBorder(card);
        foreach (var slot in rightSlots)
            slot.ShowBorder(card);
    }

    private static string GetCardType(GameObject card)
    {
        CardDisplay disp = card != null ? card.GetComponent<CardDisplay>() : null;
        return disp != null ? disp.GetData()?.type : null;
    }

    public bool CanPlaceCard(SlotController slot, GameObject card)
    {
        var disp = card != null ? card.GetComponent<CardDisplay>() : null;
        if (disp != null && disp.ownerId != playerId)
            return false;

        string type = GetCardType(card);
        if (string.IsNullOrEmpty(type))
            return false;

        if (bannedTypes.Contains(type))
            return false;

        if (!string.IsNullOrEmpty(slot.AllowedType) && slot.AllowedType != type)
            return false;

        return true;
    }

    public void OnSlotFilled(SlotController slot)
    {
        int row = slot.Index;
        string type = GetCardType(slot.CurrentCard);
        if (string.IsNullOrEmpty(type))
            return;

        SlotController opposite = slot.IsRightSide ? leftSlots[row] : rightSlots[row];

        if (!opposite.isOccupied)
            opposite.SetAllowedType(type);

        if (leftSlots[row].isOccupied && rightSlots[row].isOccupied)
        {
            leftSlots[row].SetAllowedType(null);
            rightSlots[row].SetAllowedType(null);

            string leftType = GetCardType(leftSlots[row].CurrentCard);
            string rightType = GetCardType(rightSlots[row].CurrentCard);
            if (!string.IsNullOrEmpty(leftType) && leftType == rightType)
                bannedTypes.Add(leftType);

            int next = row + 1;
            if (next < leftSlots.Count)
            {
                leftSlots[next].gameObject.SetActive(true);
                rightSlots[next].gameObject.SetActive(true);
            }
        }

        UpdateScoreDisplay();
        TurnManager.Instance?.CardPlayed();
    }

    public int CalculateScore()
    {
        int total = 0;
        int rows = Mathf.Min(leftSlots.Count, rightSlots.Count);
        for (int i = 0; i < rows; i++)
        {
            int leftVal = 0;
            if (leftSlots[i].isOccupied)
            {
                var disp = leftSlots[i].CurrentCard?.GetComponent<CardDisplay>();
                if (disp != null)
                    leftVal = disp.GetData().leftValue;
            }

            int rightVal = 0;
            if (rightSlots[i].isOccupied)
            {
                var disp = rightSlots[i].CurrentCard?.GetComponent<CardDisplay>();
                if (disp != null)
                    rightVal = disp.GetData().rightValue;
            }

            total += leftVal * rightVal;
        }

        return total;
    }

    public void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = CalculateScore().ToString();
    }
}

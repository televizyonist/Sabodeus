using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsRightSide { get; private set; }
    public int Index { get; private set; }
    public Image image;

    private CityAreaManager manager;
    private Color originalColor;
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0.5f, 1f); // Açýk sarý
    public bool isOccupied { get; private set; } = false;
    public GameObject CurrentCard { get; private set; }

    public void Initialize(CityAreaManager mgr, bool rightSide, int index, Image img)
    {
        manager = mgr;
        IsRightSide = rightSide;
        Index = index;
        image = img;
        originalColor = image != null ? image.color : Color.white;
    }

    public void AssignCard(GameObject card)
    {
        if (card == null) return;

        isOccupied = true;
        CurrentCard = card;
        if (manager != null)
        {
            manager.OnSlotFilled(this);
        }
    }

    public void ClearSlot()
    {
        isOccupied = false;
        CurrentCard = null;
        if (image != null)
            image.color = originalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isOccupied || image == null) return;
        image.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image == null) return;
        image.color = originalColor;
    }
}

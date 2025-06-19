using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    public bool IsRightSide;
    public int Index;
    public Image image;
    public GameObject currentCard;
    private CityAreaManager manager;

    public void Initialize(CityAreaManager areaManager, bool rightSide, int index)
    {
        manager = areaManager;
        IsRightSide = rightSide;
        Index = index;
    }

    public void SetCard(GameObject card)
    {
        currentCard = card;
        if (card != null && manager != null)
        {
            manager.OnSlotFilled(this);
        }
    }
}

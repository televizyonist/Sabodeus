using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(HandLayoutFanStyle))]
public class HandAreaHoverSpread : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private HandLayoutFanStyle layout;

    private void Awake()
    {
        layout = GetComponent<HandLayoutFanStyle>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        layout?.SpreadOut();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        layout?.Restore();
    }
}


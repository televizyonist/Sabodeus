using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(HandLayoutFanStyle))]
public class HandAreaHoverSpread : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
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


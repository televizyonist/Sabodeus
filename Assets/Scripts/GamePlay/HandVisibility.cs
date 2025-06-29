using Unity.Netcode;
using UnityEngine;

public class HandVisibility : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false);
        }
    }
}

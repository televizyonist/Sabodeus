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
        else
        {
            // Sahibi olan oyuncunun eli ekran�n alt�na yerle�tirilir
            transform.position = new Vector3(0, -4.5f, 0);
        }
    }
}

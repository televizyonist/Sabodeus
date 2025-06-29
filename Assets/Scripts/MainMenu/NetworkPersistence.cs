using UnityEngine;
using Unity.Netcode;

public class NetworkPersistence : MonoBehaviour
{
    private void Awake()
    {
        // Eðer sahnede baþka bir NetworkManager varsa bu kopyayý yok et
        if (FindObjectsOfType<NetworkManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Bu objeyi sahneler arasýnda yok etme
        DontDestroyOnLoad(gameObject);
    }
}

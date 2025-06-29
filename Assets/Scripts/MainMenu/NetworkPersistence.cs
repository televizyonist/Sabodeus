using UnityEngine;
using Unity.Netcode;

public class NetworkPersistence : MonoBehaviour
{
    private void Awake()
    {
        // E�er sahnede ba�ka bir NetworkManager varsa bu kopyay� yok et
        if (FindObjectsOfType<NetworkManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Bu objeyi sahneler aras�nda yok etme
        DontDestroyOnLoad(gameObject);
    }
}

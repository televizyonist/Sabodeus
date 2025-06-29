using Unity.Netcode;
using UnityEngine;

public class MultiplayerBoardInitializer : NetworkBehaviour
{
    [Header("Prefabs")]
    public GameObject cityAreaPrefab;
    public GameObject cardHandAreaPrefab;

    [Header("Pozisyonlar")]
    public Vector3 localPlayerCityPos = new Vector3(0, -1.5f, 0);
    public Vector3 remotePlayerCityPos = new Vector3(0, 1.5f, 0);
    public Vector3 localPlayerHandPos = new Vector3(0, -3f, 0);
    public Vector3 remotePlayerHandPos = new Vector3(0, 3f, 0);

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            ulong clientId = client.ClientId;

            // Pozisyonlarý belirle
            bool isLocal = clientId == NetworkManager.Singleton.LocalClientId;
            Vector3 cityPos = isLocal ? localPlayerCityPos : remotePlayerCityPos;
            Vector3 handPos = isLocal ? localPlayerHandPos : remotePlayerHandPos;

            // Þehir oluþtur
            GameObject city = Instantiate(cityAreaPrefab, cityPos, Quaternion.identity);
            city.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            CityAreaManager mgr = city.GetComponent<CityAreaManager>();
            if (mgr != null)
                mgr.playerId = (int)clientId;

            // El alaný oluþtur
            GameObject hand = Instantiate(cardHandAreaPrefab, handPos, Quaternion.identity);
            hand.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }
}

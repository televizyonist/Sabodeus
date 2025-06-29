using Unity.Netcode;
using UnityEngine;

public class MultiplayerBoardInitializer : NetworkBehaviour
{
    [SerializeField] private GameObject cityAreaPrefab;
    [SerializeField] private Vector3 localPlayerCityPos = new Vector3(0, -1.5f, 0);
    [SerializeField] private Vector3 localPlayerHandPos = new Vector3(0, -3f, 0);
    [SerializeField] private Vector3 remotePlayerCityPos = new Vector3(0, 1.5f, 0);
    [SerializeField] private Vector3 remotePlayerHandPos = new Vector3(0, 3f, 0);

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        var clients = NetworkManager.Singleton.ConnectedClientsList;
        int count = clients.Count;

        for (int i = 0; i < count; i++)
        {
            var client = clients[i];
            ulong clientId = client.ClientId;

            // Eðer bu oyuncu LocalPlayer ise alt konumda, deðilse üstte
            bool isLocal = clientId == NetworkManager.Singleton.LocalClientId;

            Vector3 spawnPos = isLocal ? localPlayerCityPos : remotePlayerCityPos;

            GameObject city = Instantiate(cityAreaPrefab, spawnPos, Quaternion.identity);
            city.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            CityAreaManager mgr = city.GetComponent<CityAreaManager>();
            if (mgr != null)
            {
                mgr.playerId = (int)clientId;
            }
        }
    }
}

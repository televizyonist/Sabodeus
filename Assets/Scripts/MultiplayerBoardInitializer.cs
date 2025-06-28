using Unity.Netcode;
using UnityEngine;

public class MultiplayerBoardInitializer : NetworkBehaviour
{
    public GameObject cityAreaPrefab;
    public Vector3 playerOnePosition = new Vector3(-3f, 0f, 0f);
    public Vector3 playerTwoPosition = new Vector3(3f, 0f, 0f);

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            ulong clientId = client.ClientId;
            Vector3 spawnPos = clientId == 0 ? playerOnePosition : playerTwoPosition;

            GameObject city = Instantiate(cityAreaPrefab, spawnPos, Quaternion.identity);
            city.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            CityAreaManager mgr = city.GetComponent<CityAreaManager>();
            if (mgr != null)
                mgr.playerId = (int)clientId;
        }
    }
}

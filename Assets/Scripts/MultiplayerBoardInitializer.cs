using Unity.Netcode;
using UnityEngine;

public class MultiplayerBoardInitializer : NetworkBehaviour
{
    public GameObject cityAreaPrefab;
    public Vector3 playerOnePosition = new Vector3(-3f, 0f, 0f);
    public Vector3 playerTwoPosition = new Vector3(3f, 0f, 0f);

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        var clients = NetworkManager.Singleton.ConnectedClientsList;
        int count = clients.Count;

        float spacing = Mathf.Abs(playerTwoPosition.x - playerOnePosition.x);
        float offset = (count - 1) / 2f;

        for (int i = 0; i < count; i++)
        {
            var client = clients[i];
            ulong clientId = client.ClientId;
            Vector3 spawnPos = new Vector3((i - offset) * spacing, playerOnePosition.y, playerOnePosition.z);

            GameObject city = Instantiate(cityAreaPrefab, spawnPos, Quaternion.identity);
            city.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            CityAreaManager mgr = city.GetComponent<CityAreaManager>();
            if (mgr != null)
                mgr.playerId = (int)clientId;
        }
    }
}

using Unity.Netcode;
using UnityEngine;

public class LobbyPlayerListManager : NetworkBehaviour
{
    public static LobbyPlayerListManager Instance;

    private NetworkList<ulong> connectedClientIds = new NetworkList<ulong>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            connectedClientIds.Clear();

            foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!connectedClientIds.Contains(id))
                    connectedClientIds.Add(id);
            }

            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
        }

        connectedClientIds.OnListChanged += OnListChangedLog;
    }

    private void OnDisable()
    {
        if (connectedClientIds != null)
        {
            connectedClientIds.OnListChanged -= OnListChangedLog;
        }
    }


    private void OnClientConnected(ulong clientId)
    {
        if (!connectedClientIds.Contains(clientId))
            connectedClientIds.Add(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (connectedClientIds.Contains(clientId))
        {
            connectedClientIds.Remove(clientId);
            Debug.Log($"[Lobby] Oyuncu ayrıldı: {clientId}");
        }
    }

    private void OnListChangedLog(NetworkListEvent<ulong> changeEvent)
    {
        Debug.Log("[Lobby] Güncel oyuncular: " + string.Join(", ", connectedClientIds));
    }

    public NetworkList<ulong> GetConnectedClientIds()
    {
        return connectedClientIds;
    }

    public void RegisterUIRefresh(NetworkList<ulong>.OnListChangedDelegate callback)
    {
        connectedClientIds.OnListChanged += callback;
    }

    public void UnregisterUIRefresh(NetworkList<ulong>.OnListChangedDelegate callback)
    {
        connectedClientIds.OnListChanged -= callback;
    }
}

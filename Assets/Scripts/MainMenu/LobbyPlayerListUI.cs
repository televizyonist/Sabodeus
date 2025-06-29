using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class LobbyPlayerListUI : MonoBehaviour
{
    public Transform playerListParent;
    public GameObject playerEntryPrefab;

    private Dictionary<ulong, GameObject> playerEntries = new();

    private void OnEnable()
    {
        RefreshPlayerList();

        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerChange;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerChange;

        if (LobbyPlayerListManager.Instance != null)
        {
            LobbyPlayerListManager.Instance.RegisterUIRefresh(OnListChanged);
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerChange;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerChange;
        }

        if (LobbyPlayerListManager.Instance != null)
        {
            LobbyPlayerListManager.Instance.UnregisterUIRefresh(OnListChanged);
        }
    }

    private void OnPlayerChange(ulong _)
    {
        RefreshPlayerList();
    }

    private void OnListChanged(NetworkListEvent<ulong> _)
    {
        RefreshPlayerList();
    }

    private void RefreshPlayerList()
    {
        foreach (Transform child in playerListParent)
            Destroy(child.gameObject);

        var manager = LobbyPlayerListManager.Instance;
        if (manager == null) return;

        foreach (ulong id in manager.GetConnectedClientIds())
            AddPlayer(id);
    }

    private void AddPlayer(ulong clientId)
    {
        GameObject entry = Instantiate(playerEntryPrefab, playerListParent);
        var text = entry.GetComponentInChildren<TMP_Text>();
        text.text = $"Player {clientId}";
    }
}

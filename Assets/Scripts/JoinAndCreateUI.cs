using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class JoinAndCreateUI : MonoBehaviour
{
    public GameObject startGameButton;
    public GameObject maxPlayerPanel;
    public GameObject fullRoomWarning;
    public TMP_Dropdown maxPlayerDropdown;
    public RelayManager relayManager;
    public TMP_InputField joinCodeInput;
    public TMP_Text roomCodeText;

    public GameObject mainMenuPanel;
    public GameObject lobbyPanel;

    private bool hasFullyJoined = false;
    private float connectedAtTime = 0f;
    private float minStableTime = 2f;

    private void Start()
    {
        if (relayManager != null)
        {
            relayManager.OnRelayCreated.AddListener(ShowLobbyPanel);
        }
        else
        {
            Debug.LogWarning("RelayManager atanmamış!");
        }
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

        if (relayManager != null)
            relayManager.OnRelayCreated.RemoveListener(ShowLobbyPanel);
    }

    private void Update()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsHost)
            return;

        // Bağlantı kurulduktan sonra en az birkaç saniye bekle
        if (!hasFullyJoined || Time.time - connectedAtTime < minStableTime)
            return;

        if ((!NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient)
            && lobbyPanel.activeInHierarchy)
        {
            Debug.Log("[Client] Bağlantı koptu (korumalı), ana menüye dönülüyor.");
            lobbyPanel?.SetActive(false);
            mainMenuPanel?.SetActive(true);
            hasFullyJoined = false;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // Buradaki kontrol artık yedek, esas kontrol Update içinde yapılır
        if (hasFullyJoined && !NetworkManager.Singleton.IsHost && clientId == NetworkManager.ServerClientId)
        {
            Debug.Log("[Client] Host bağlantısı kesildi (event).");
            lobbyPanel?.SetActive(false);
            mainMenuPanel?.SetActive(true);
            hasFullyJoined = false;
        }
    }

    public void OnCreateGameClicked()
    {
        relayManager?.CreateRelay();
    }

    public void OnJoinGameClicked()
    {
        string code = joinCodeInput?.text?.Trim().ToUpper();

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("Join kodu boş!");
            return;
        }

        relayManager?.JoinRelayWithCallback(code, () =>
        {
            ShowLobbyPanel(code);
        });
    }

    public void ShowLobbyPanel(string code)
    {
        mainMenuPanel?.SetActive(false);
        lobbyPanel?.SetActive(true);

        if (roomCodeText != null)
            roomCodeText.text = "Room Code: " + code;

        bool isHost = NetworkManager.Singleton.IsHost;

        if (startGameButton != null)
            startGameButton.SetActive(isHost);

        if (maxPlayerPanel != null)
            maxPlayerPanel.SetActive(isHost);

        hasFullyJoined = true;
        connectedAtTime = Time.time;
    }

    public void OnBackToMenuClicked()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            StartCoroutine(KickAndCloseRoom());
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
            lobbyPanel?.SetActive(false);
            mainMenuPanel?.SetActive(true);
            hasFullyJoined = false;
        }
    }

    private System.Collections.IEnumerator KickAndCloseRoom()
    {
        var ids = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);

        foreach (var clientId in ids)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log($"[Host] Oyuncu atılıyor: {clientId}");
                NetworkManager.Singleton.DisconnectClient(clientId);
                yield return new WaitForSeconds(0.25f);
            }
        }

        while (NetworkManager.Singleton.ConnectedClientsIds.Count > 1)
        {
            yield return null;
        }

        Debug.Log("[Host] Oda kapatılıyor.");
        NetworkManager.Singleton.Shutdown();

        lobbyPanel?.SetActive(false);
        mainMenuPanel?.SetActive(true);
        hasFullyJoined = false;
    }
}

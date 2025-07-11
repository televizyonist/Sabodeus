using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;

public class RelayManager : MonoBehaviour
{
    public UnityEvent<string> OnRelayCreated;
    public int maxPlayers = 4;

    [SerializeField] private JoinAndCreateUI cachedUI;

    private NetworkManager netManager;
    private UnityTransport transport;
    private bool initialized = false;

    public bool IsInitialized => initialized;

    private async void Awake()
    {
        try
        {
            await Initialize();
            netManager = NetworkManager.Singleton;

            // 🔧 UnityTransport'ı güvenli şekilde bul:
            transport = netManager?.GetComponent<UnityTransport>();
            if (transport == null)
                transport = GameObject.FindObjectOfType<UnityTransport>();

            if (cachedUI == null)
                cachedUI = FindObjectOfType<JoinAndCreateUI>();

            initialized = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[RelayManager] Awake failed: " + e);
        }
    }

    private async Task Initialize()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        return;
    }

    public async Task CreateRelay()
    {
        if (!initialized)
        {
            Debug.LogWarning("[RelayManager] CreateRelay called before initialization finished.");
            return;
        }
        if (!CheckTransport()) return;

        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            transport.SetRelayServerData(new RelayServerData(allocation, "dtls"));

            if (netManager.StartHost())
            {
                Debug.Log("[RelayManager] Host başlatıldı. Kod: " + joinCode);
                OnRelayCreated?.Invoke(joinCode);
            }
            else
            {
                Debug.LogError("[RelayManager] Host başlatılamadı.");
            }
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("[RelayManager] Relay Host Hatası: " + e);
        }
    }

    public async Task JoinRelayWithCallback(string joinCode, System.Action onSuccess)
    {
        if (!initialized)
        {
            Debug.LogWarning("[RelayManager] JoinRelay called before initialization finished.");
            return;
        }
        if (!CheckTransport()) return;

        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            if (netManager.ConnectedClientsIds.Count >= maxPlayers)
            {
                ShowFullRoomWarning();
                return;
            }

            transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            if (netManager.StartClient())
            {
                Debug.Log("[RelayManager] Client bağlandı. Kod: " + joinCode);
                onSuccess?.Invoke();
            }
            else
            {
                Debug.LogError("[RelayManager] Client başlatılamadı.");
            }
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("[RelayManager] Relay Join Hatası: " + e);
        }
    }

    private bool CheckTransport()
    {
        if (netManager == null)
        {
            Debug.LogError("[RelayManager] NetworkManager Singleton eksik!");
            return false;
        }

        if (transport == null)
        {
            Debug.LogError("[RelayManager] UnityTransport bulunamadı!");
            return false;
        }

        return true;
    }

    private void ShowFullRoomWarning()
    {
        if (cachedUI == null)
            cachedUI = FindObjectOfType<JoinAndCreateUI>();

        if (cachedUI?.fullRoomWarning != null)
            cachedUI.fullRoomWarning.SetActive(true);
        else
            Debug.LogWarning("Uyarı kutusu bulunamadı.");
    }

    public void UpdateMaxPlayersFromDropdown(int dropdownIndex)
    {
        maxPlayers = dropdownIndex + 2;
        Debug.Log("[RelayManager] Max player set to: " + maxPlayers);
    }
}

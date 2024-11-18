using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button connectButton;
    private bool connectionAttemptFailed = false;
    private int retryCount = 0;
    private const int maxRetries = 3;
    private bool isHostInitialized = false;
    private bool isClientFullyConnected = false;

    private void Awake()
    {
        connectButton.onClick.AddListener(OnConnectButtonPressed);
    }

    private void Start(){
        LogNetworkConfig();
    }

    private void OnConnectButtonPressed()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Already connected as Host or Client.");
            return;
        }

        StartClientWithFallbackToHost();
    }

    private void LogNetworkConfig()
    {
        var config = NetworkManager.Singleton.NetworkConfig;
        /*Debug.Log("NetworkConfig Settings:");
        
        Debug.Log("Connection Approval: " + config.ConnectionApproval);
        Debug.Log("Protocol Version: " + config.ProtocolVersion);


        // Log any other common settings
        Debug.Log("Enable Scene Management: " + config.EnableSceneManagement);
        Debug.Log("Force Same Prefabs: " + config.ForceSamePrefabs);*/
    }

    private async void StartClientWithFallbackToHost()
    {
        retryCount = 0;
        connectionAttemptFailed = false;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        while (retryCount < maxRetries && !NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsConnectedClient)
        {
            retryCount++;
            Debug.Log($"Attempting to start as Client (Attempt {retryCount})...");
            NetworkManager.Singleton.StartClient();

            // Wait for connection attempt to complete
            await Task.Delay(3000);

            if (connectionAttemptFailed || !NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient || !isClientFullyConnected)
            {
                connectionAttemptFailed = true;
                Debug.LogWarning("Client connection attempt failed.");
                NetworkManager.Singleton.Shutdown();
                await Task.Delay(500);
            }
            else
            {
                Debug.Log("Successfully connected as Client.");
                break;
            }
        }

        // If all client connection attempts fail, start as host
        if (connectionAttemptFailed && retryCount >= maxRetries)
        {
            Debug.Log("All client connection attempts failed. Starting as Host...");
            NetworkManager.Singleton.StartHost();

            await Task.Delay(5000); // Wait for host connection initialization

            isHostInitialized = true;
            Debug.Log("Host initialization complete.");
            
            // Force registration as a connected client
            OnClientConnected(0); 
        }

        // Unregister callbacks to avoid memory leaks
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Ignore the host's own client connection (client ID 0) when running as host until fully initialized
        if (clientId == 0 && NetworkManager.Singleton.IsHost && !isHostInitialized)
        {
            Debug.Log("Ignoring host's own client connection (ID 0) until fully initialized.");
            return;
        }

        Debug.Log("Client successfully connected with ID: " + clientId);
        
        // Mark the client as fully connected
        isClientFullyConnected = true;
        connectionAttemptFailed = false;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // Ignore the host's own client disconnection (client ID 0) when running as host
        if (clientId == 0 && NetworkManager.Singleton.IsHost && !isHostInitialized)
        {
            Debug.Log("Host's own client disconnection ignored.");
            return;
        }

        Debug.LogWarning("Client disconnected with ID: " + clientId);
        connectionAttemptFailed = true;
        isClientFullyConnected = false;
    }
}
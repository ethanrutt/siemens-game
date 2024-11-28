using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button connectButton;
    [SerializeField] private TMP_InputField ipAddressField; // Input for the host IP address
    [SerializeField] private GameObject playerOne; // Reference to the PlayerOne GameObject
    [SerializeField] private GameObject deckOverlay; // Reference to the PlayerOne GameObject
    [SerializeField] private GameObject discardOverlay; // Reference to the PlayerOne GameObject
    [SerializeField] private CardSharingManager cardShare; // Reference to the PlayerOne GameObject
    [SerializeField] private GameManager2 gm; // Reference to the PlayerOne GameObject
    [SerializeField] private ushort port = 7777; // Default port for LAN communication

    private bool connectionAttemptFailed = false;
    private int retryCount = 0;
    private const int maxRetries = 3;
    private static bool isShuttingDown = false;

    // Network variables
    private NetworkVariable<bool> hostInitialized = new NetworkVariable<bool>(false); // Tracks if the host is initialized
    private NetworkVariable<int> synchronizedClientsCount = new NetworkVariable<int>(0); // Tracks the number of connected clients (excluding the host)

    private void Awake()
    {
        connectButton.onClick.AddListener(OnConnectButtonPressed);
        playerOne.SetActive(false); // Ensure PlayerOne is inactive by default
        deckOverlay.SetActive(false);
        discardOverlay.SetActive(false);

        // Set the IP address field to the device's LAN IP
        string localIP = GetLocalIPAddress();
        if (!string.IsNullOrEmpty(localIP))
        {
            ipAddressField.text = localIP;
        }
        else
        {
            Debug.LogWarning("Unable to determine the local LAN IP address.");
        }
    }

    private void Start()
    {
        LogNetworkConfig();

        // Listen for changes to the network variables
        hostInitialized.OnValueChanged += OnHostInitializedChanged;
        synchronizedClientsCount.OnValueChanged += OnClientsCountChanged;
    }

    private void OnDestroy()
    {
        isShuttingDown = true; // Mark the shutdown state for the host
        Debug.Log("Host is shutting down. Skipping further updates to NetworkVariables.");

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        hostInitialized.OnValueChanged -= OnHostInitializedChanged;
        synchronizedClientsCount.OnValueChanged -= OnClientsCountChanged;
    }

    private void OnConnectButtonPressed()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Already connected as Host or Client.");
            connectButton.gameObject.SetActive(false);
            ipAddressField.gameObject.SetActive(false);
            return;
        }

        string ipAddress = ipAddressField.text;
        if (string.IsNullOrEmpty(ipAddress))
        {
            Debug.LogError("Please enter a valid IP address.");
            return;
        }

        connectButton.gameObject.SetActive(false);
        ipAddressField.gameObject.SetActive(false);

        StartClientWithFallbackToHost(ipAddress);
    }

    private void LogNetworkConfig()
    {
        var config = NetworkManager.Singleton.NetworkConfig;
        Debug.Log("Network Configuration loaded.");
    }

    private async void StartClientWithFallbackToHost(string ipAddress)
    {
        retryCount = 0;
        connectionAttemptFailed = false;

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(ipAddress, port);

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        while (retryCount < maxRetries && !NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsConnectedClient)
        {
            retryCount++;
            Debug.Log($"Attempting to connect to {ipAddress}:{port} (Attempt {retryCount})...");
            NetworkManager.Singleton.StartClient();

            // Wait for connection attempt to complete
            await Task.Delay(3000);

            if (connectionAttemptFailed || !NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient)
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

        if (connectionAttemptFailed && retryCount >= maxRetries)
        {
            Debug.LogWarning("All client connection attempts failed. Starting as Host...");
            StartHost();
        }
        
        if(hostInitialized.Value == false && connectionAttemptFailed){
            connectButton.gameObject.SetActive(true);
            ipAddressField.gameObject.SetActive(true);
        }
    }

    private async void StartHost()
    {
        NetworkManager.Singleton.StartHost();

        if (IsServer)
        {
            hostInitialized.Value = true;
            synchronizedClientsCount.Value = 0;
            await Task.Delay(7500);
            Debug.Log("Host initialized successfully.");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost && clientId == 0)
        {
            Debug.Log("Host's own client connection ignored.");
            return;
        }

        Debug.Log($"Client connected with ID: {clientId}");

        if (IsHost)
        {
            synchronizedClientsCount.Value++;
            Debug.Log($"Updated synchronizedClientsCount: {synchronizedClientsCount.Value}");
        }

        CheckAndActivatePlayerOne();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (isShuttingDown || !NetworkManager.Singleton || !NetworkManager.Singleton.IsHost)
        {
            Debug.LogWarning("NetworkManager is not active or host is shutting down. Skipping disconnection handling.");
            
            gm.Reset();
            playerOne.SetActive(false);
            deckOverlay.SetActive(false);
            discardOverlay.SetActive(false);
            connectButton.gameObject.SetActive(true);
            ipAddressField.gameObject.SetActive(true);

            cardShare.DestroyAllSharedCards();
            return;
        }

        // Ignore the host's own client disconnection (ID 0)
        if (NetworkManager.Singleton.IsHost && clientId == 0)
        {
            Debug.Log("Host's own client disconnection ignored.");
            return;
        }

        Debug.LogWarning($"Client disconnected with ID: {clientId}");

        if (IsHost)
        {
            // Increment the synchronizedClientsCount
            synchronizedClientsCount.Value--;
            Debug.Log($"Updated synchronizedClientsCount: {synchronizedClientsCount.Value}");
            
        }

        // Check conditions to activate PlayerOne
        CheckAndActivatePlayerOne();
    }

    private void OnHostInitializedChanged(bool previousValue, bool newValue)
    {
        Debug.Log($"HostInitialized changed to: {newValue}");
        CheckAndActivatePlayerOne();
    }

    private void OnClientsCountChanged(int previousCount, int newCount)
    {
        Debug.Log($"SynchronizedClientsCount changed to: {newCount}");
        CheckAndActivatePlayerOne();
    }

    private void CheckAndActivatePlayerOne()
    {
        if (hostInitialized.Value && synchronizedClientsCount.Value > 0)
        {
            if (!playerOne.activeSelf)
            {
                playerOne.SetActive(true);
                deckOverlay.SetActive(true);
                discardOverlay.SetActive(true);
                Debug.Log("PlayerOne activated: Host initialized and at least one client connected.");
            }
        }
        else
        {
            if (playerOne.activeSelf)
            {
                gm.Reset();
                playerOne.SetActive(false);
                deckOverlay.SetActive(false);
                discardOverlay.SetActive(false);
                Debug.Log("PlayerOne deactivated: Host not initialized or no clients connected.");
            }
        }
    }


    // Utility to get the local LAN IP address
    private string GetLocalIPAddress()
    {
        try
        {
            foreach (var networkInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                    (networkInterface.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211 ||
                     networkInterface.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet))
                {
                    foreach (var address in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return address.Address.ToString(); // Return the first valid LAN IP
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error getting LAN IP Address: " + ex.Message);
        }
        return null; // Return null if no IP is found
    }
}
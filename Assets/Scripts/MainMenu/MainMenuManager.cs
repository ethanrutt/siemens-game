using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UserData
{
    public int user_id;
    public string user_name;
    public string employee_id;
    public int current_coins;
    public int total_coins;
    public List<int> items_owned;
    public List<int> items_equipped;
}

[System.Serializable]
public class LoginResponse
{
    public string message;
    public UserData user;
}

// Rishi Santhanam
// Main menu manager script will be responsible for the functions that the
// buttons on the main menu are assigned to. It will cause certain modals to
// appear and disappear, and will also load the game scene when the play button
// is clicked.
public class MainMenuManager : MonoBehaviour
{
    // Serialize all panels in the game needed
    // in GameObject form (for simple act/deact)
    [SerializeField] private GameObject loginPanel; // Base login panel
    [SerializeField] private GameObject registerPanel; // Base register panel
    [SerializeField] private GameObject loginregAbstractPanel; // The panel responsible for abstracting the login and register panels

    // Serialize the modal blackOut
    [SerializeField] private GameObject blackOutModal;

    // Serialize the settings and credits panels
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [SerializeField] private TMP_InputField createAccountEmployeeId;
    [SerializeField] private TMP_InputField createAccountUsername;
    [SerializeField] private TMP_InputField createAccountPassword;

    [SerializeField] private TMP_InputField loginUsername;
    [SerializeField] private TMP_InputField loginPassword;

    [SerializeField] private Button createAccountButton;
    [SerializeField] private Button loginButton;

    // For beta testing, without logging in you will go to starting cutscene
    public void PlayGame()
    {
        // Load the game scene starting cutscene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Starting-Cutscene");
    }

    // Close button functions for login,register,abstract,settings,credits
    public void CloseLoginPanel()
    {
        loginPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseRegisterPanel()
    {
        registerPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseLoginRegAbstractPanel()
    {
        loginregAbstractPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseCreditsPanel()
    {
        creditsPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    // Open button functions for login,register,abstract,settings,credits
    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        blackOutModal.SetActive(true);

        // Make the abstract panel disappear
        loginregAbstractPanel.SetActive(false);
    }

    public void OpenRegisterPanel()
    {
        registerPanel.SetActive(true);
        blackOutModal.SetActive(true);

        // Make the abstract panel disappear
        loginregAbstractPanel.SetActive(false);
    }

    public void OpenLoginRegAbstractPanel()
    {
        loginregAbstractPanel.SetActive(true);
        blackOutModal.SetActive(true);
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        blackOutModal.SetActive(true);
    }

    public void OpenCreditsPanel()
    {
        creditsPanel.SetActive(true);
        blackOutModal.SetActive(true);
    }

    public void CollectLoginInput()
    {
        string username = loginUsername.text;
        string password = loginPassword.text;

        SendLoginRequest(username, password);
    }

    public void SetupErrorScreen(string errorMessage)
    {
        errorScreen.SetActive(true);
        errorMessage.text = errorMessage;
    }


    public void CollectCreateAccountInput()
    {
        string employeeId = createAccountEmployeeId.text;
        string username = createAccountUsername.text;
        string password = createAccountPassword.text;

        SendCreateAccountRequest(employeeId, username, password);
    }

    public void SendLoginRequest(string username, string password)
    {
        Debug.Log($"sending request to login for {username} {password}");

        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/Login";
        string jsonData = System.String.Format(@"{{
            ""user_name"": ""{0}"",
            ""user_password"": ""{1}""
        }}", username, password);

        WebRequestUtility.SendWebRequest(this, url, jsonData, (string response) =>
        {
            LoginResponse data = JsonUtility.FromJson<LoginResponse>(response);
            playerData.username = data.user.user_name;
            playerData.userId = data.user.user_id;
            playerData.coins = data.user.current_coins;
            playerData.unlocked_items = data.user.items_owned;
            playerData.equipped_items = data.user.items_equipped;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Starting-Cutscene");
        });
    }

    public void SendCreateAccountRequest(string employeeId, string username, string password)
    {
        Debug.Log($"sending request to create account for {employeeId} {username} {password}");

        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/Login";
        string jsonData = System.String.Format(@"{{
            ""user_name"": ""{0}"",
            ""user_password"": ""{1}"",
            ""employee_id"": {2}
        }}", username, password, employeeId);

        WebRequestUtility.SendWebRequest(this, url, jsonData, (string response) =>
        {
            LoginResponse data = JsonUtility.FromJson<LoginResponse>(response);
            playerData.username = data.user.user_name;
            playerData.userId = data.user.user_id;
            playerData.coins = data.user.current_coins;
            playerData.unlocked_items = data.user.items_owned;
            playerData.equipped_items = data.user.items_equipped;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Starting-Cutscene");
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        isLoginSuccessful = false;
        createAccountButton.onClick.AddListener(CollectCreateAccountInput);
        loginButton.onClick.AddListener(CollectLoginInput);
    }
}

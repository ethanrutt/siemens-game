using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
using UnityEngine.UI;

/**
 * @class UserData
 * @brief This is a dataclass used to work with the json utility in Unity.
 * @details The fields in this class are synced up with the fields in the json
 * response from the API. It is important to not change these names (i.e. to
 * camelcase) because they are serialized directly from the json response.
 */
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

/**
 * @class LoginResponse
 * @brief This is a dataclass used to work with the json utility in Unity.
 * @details The fields in this class are synced up with the fields in the json
 * response from the API. It is important to not change these names (i.e. to
 * camelcase) because they are serialized directly from the json response.
 * Becuase of the way that the login response is formed, this class mainly uses
 * the UserData class to hold the relevant information. This is effectively a
 * wrapper around the UserData dataclass.
 *
 * @see UserData
 */
[System.Serializable]
public class LoginResponse
{
    public string message;
    public UserData user;
}
=======
>>>>>>> 878ff7f2413801b48682745b6faf2f5a490799a2

// Rishi Santhanam
// Main menu manager script will be responsible for the functions that the
// buttons on the main menu are assigned to. It will cause certain modals to
// appear and disappear, and will also load the game scene when the play button
// is clicked.
/**
 * @class MainMenuManager
 * @brief This handles the login and entering into the introduction cutscene at
 * the start of the game
 * @details This class handles the display of the login modals as well as
 * sending the login api request.
 * - On new user creation, the username is checked against a bad word list seen
 *   below.
 * - Once the player is successfully logged in, we populate PlayerData with the
 *   values we get back from the API
 * - If the login is unsuccessful, error screens are shown
 * - If login is successful, then the player data is populated and the starting
 *   cutscene is loaded
 *
 * @see UserData
 * @see PlayerData
 */
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

<<<<<<< HEAD
    public void CollectLoginInput()
    {
        string username = loginUsername.text;
        string password = loginPassword.text;

        SendLoginRequest(username, password);
    }

    public void SetupErrorScreen(string errorMessage)
    {
        errorScreen.SetActive(true);
        // Close other
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        loginregAbstractPanel.SetActive(false);
        settingsPanel.SetActive(false);
        errorMessageText.text = errorMessage;
    }

    public void CollectCreateAccountInput()
    {
        string employeeId = createAccountEmployeeId.text;
        string username = createAccountUsername.text;
        string password = createAccountPassword.text;

        for (int i = 0; i < badwords.Length; i++)
        {
            if (username.Contains(badwords[i]))
            {
                SetupErrorScreen($"Error: Your username contains {badwords[i]}, an inappropriate word. Please change it");
                return;
            }
        }

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

        WebRequestUtility.SendWebRequest(
                this,
                url,
                jsonData,
                (string response) => {
                    LoginResponse data = JsonUtility.FromJson<LoginResponse>(response);
                    playerData.username = data.user.user_name;
                    playerData.userId = data.user.user_id;
                    playerData.coins = data.user.current_coins;
                    playerData.unlocked_items = data.user.items_owned;
                    playerData.equipped_items = data.user.items_equipped;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Starting-Cutscene");
                },
                (string response) => {
                    SetupErrorScreen("Error: The request was invalid. Are you sure you have the right username and password?");
                }
        );
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

        WebRequestUtility.SendWebRequest(
                this,
                url,
                jsonData,
                (string response) => {
                    LoginResponse data = JsonUtility.FromJson<LoginResponse>(response);
                    playerData.username = data.user.user_name;
                    playerData.userId = data.user.user_id;
                    playerData.coins = data.user.current_coins;
                    playerData.unlocked_items = data.user.items_owned;
                    playerData.equipped_items = data.user.items_equipped;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Starting-Cutscene");
                },
                (string response) => {
                    SetupErrorScreen("Error: The request was invalid. Are you sure you have the right employee ID? If so, please contact Sidra Maryam");
                }

        );
    }

=======
>>>>>>> 878ff7f2413801b48682745b6faf2f5a490799a2
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

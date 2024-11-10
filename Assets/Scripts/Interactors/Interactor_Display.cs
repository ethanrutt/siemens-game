using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

[System.Serializable]
public class UserScoreList
{
    public List<UserScore> users;
}

[System.Serializable]
public class UserScore
{
    // DON'T CHANGE THIS TO CAMEL CASE
    // since we are parsing JSON, the names need to match up with what is in
    // the JSON for this to serialize properly
    public string user_name;
    public double score;
}


public class Interactor_Display : MonoBehaviour
{
    // Call the GameObject for Leaderboards
    [SerializeField] private GameObject leaderboards;

    // Call the GameObject for the Base Game UI
    [SerializeField] private GameObject baseGameUI;

    // Call the GameObject for Player
    [SerializeField] private GameObject player;

    // Call the Inventory Panel
    [SerializeField] private GameObject inventoryPanel;

    // Menu Panel
    [SerializeField] private GameObject menuPanel;

    // Modal
    [SerializeField] private GameObject modal;

    [SerializeField] private TMP_Text top5;
    [SerializeField] private TMP_Text top10;

    // Grab the dialogueManager script object
    // the script is DialogueManager_TS on the Town_Square scene
    // and is on DialogueManager object
    [SerializeField] private DialogueManager_TS dialogueManager;

    // GameManager
    public GameManager gameManager => GameManager.Instance;

    // Defining open Menu
    public void OpenMenu()
    {
        menuPanel.SetActive(true);
        baseGameUI.SetActive(false);

        // Stop the player
        player.GetComponent<Character_Movement>().StopPlayer();
    }

    // Defining leave Menu
    public void LeaveMenu()
    {
        menuPanel.SetActive(false);
        baseGameUI.SetActive(true);

        // Unstop the player
        player.GetComponent<Character_Movement>().UnstopPlayer();
    }

    // Defining the Exit Out for the Game
    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Defining the Exit Out for Leaderboards
    public void ExitLeaderboards()
    {
        leaderboards.SetActive(false);
        baseGameUI.SetActive(true);

        // Unstop the player
        player.GetComponent<Character_Movement>().UnstopPlayer();
    }

    // Defining the Exit Out for Inventory
    public void ExitInventory()
    {
        inventoryPanel.SetActive(false);
        baseGameUI.SetActive(true);
        modal.SetActive(false);

        // Unstop the player
        player.GetComponent<Character_Movement>().UnstopPlayer();
    }

    // Opening the Inventory
    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        baseGameUI.SetActive(false);
        modal.SetActive(true);

        // Stop the player
        player.GetComponent<Character_Movement>().StopPlayer();
    }

    // Grab the playerData's interactable string
    // EX: If the playerData's interactable string is "leaderboard"
    // then we will show the leaderboards
    public string interactable
    {
        get { return PlayerData.Instance.interactable; }
    }

    public void DisplayPanel()
    {
        // Debug.Log("Display Panel" + interactable);
        // If the player walks into the collider for the leaderboard,
        // we will change the player's interactable to "leaderboard"
        // and show the interact button.
        if (interactable == "leaderboard")
        {
            // Debug.Log("Leaderboard Interact");
            leaderboards.SetActive(true);
            baseGameUI.SetActive(false);

            // Stop the player
            player.GetComponent<Character_Movement>().StopPlayer();
        } else if (interactable == "enterlaboratory")
        {
            // Debug.Log("Enter Laboratory"); -33, 10.25
            gameManager.ChangePlayerSpawnPosition(new Vector2(-33f, 10.25f));
            SceneManager.LoadScene("Laboratory_Main");
        } else if (interactable == "exitlaboratory")
        {
            // Debug.Log("Exit Laboratory");
            gameManager.ChangePlayerSpawnPosition(new Vector2(13.97f, -4.36f));
            SceneManager.LoadScene("Town_Square");
        } else if (interactable == "drunkard")
        {
            // Debug.Log("Drunkard Interact");
            dialogueManager.TalkToDrunkGuy();
        }
        else if (interactable == "shopowner")
        {
            // Debug.Log("Shop Owner Interact");
            dialogueManager.TalkToShopOwner();
        }
        // exit caisno and enter asino
        else if (interactable == "exitcasino")
        {
            // Debug.Log("Exit Casino");
            gameManager.ChangePlayerSpawnPosition(new Vector2(-22f, -1.5f));
            SceneManager.LoadScene("Town_Square");
        } else if (interactable == "entercasino")
        {
            // Debug.Log("Enter Casino"); //0.36,-9.37
            gameManager.ChangePlayerSpawnPosition(new Vector2(0.36f, -9.37f));
            SceneManager.LoadScene("Casino_Main");
        } else if (interactable == "enterwackywires")
        {
            //none
        }
         else {
            // Debug.Log("No Interactable");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find dialogueManager_TS if current scene is Town_Square
        if (SceneManager.GetActiveScene().name == "Town_Square")
        {
            if (dialogueManager == null)
            {
                dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager_TS>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetPeculiarPipesLeaderboard()
    {
        Debug.Log("getting pipe leaderboard");
        getLeaderboard(5);
    }
    public void GetWackyWiresLeaderboard()
    {
        Debug.Log("getting wire leaderboard");
        getLeaderboard(7);
    }

    private void getLeaderboard(int gameId)
    {
        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/Leaderboard";
        string jsonData = System.String.Format(@"{{
            ""game_id"": {0}
        }}", gameId);

        WebRequestUtility.SendWebRequest(this, url, jsonData, setLeaderboardText);
    }

    void setLeaderboardText(string responseText)
    {
        if (responseText != null)
        {
            string json = System.String.Format(@"{{
                ""users"" : {0}
            }}", responseText);

            UserScoreList scores = JsonUtility.FromJson<UserScoreList>(json);

            int i = 1;
            string top5Text = "";
            string top10Text = "";

            foreach (UserScore user in scores.users)
            {
                string currText = System.String.Format("{0}. {1} : {2:00.00}\n", i, user.user_name, user.score);
                if (i < 6)
                {
                    top5Text += currText;
                }
                else if (i <= 10)
                {
                    top10Text += currText;
                }
                else
                {
                    break;
                }
                i++;
            }

            top5.text = top5Text;
            top10.text = top10Text;
        }
    }
}

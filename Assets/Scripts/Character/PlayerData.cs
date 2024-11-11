using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rishi Santhanam
// Player data storage. All player data is stored here.
public class PlayerData : MonoBehaviour
{
    // Storing information like the player's username, coins
    public string username;
    public int coins;
    public int userId;

    public static PlayerData Instance;
    
    // Storing public information like the current interactable
    public string interactable = "";

    // Now storing information used for achievements
    // Hint (to Ethan & Rohan & possibly Naveed) -> you will need to call the PlayerData instance
    // and affect these variables upon any sort of win
    public int wire_puzzle_wins = 0;
    public int wire_puzzle_fullround = 0; // Make it to the last round
    public int card_game_wins = 0; // Wins from the card game
    public int card_game_fullround = 0; // Make it to the last round
    public int casino_winnings = 0; // Total winnings from the casino (coins)
    public int casino_losses = 0; // Total losses from the casino　（coins)
    public int pipe_puzzle_wins = 0; // Wins from the pipe puzzle

    // Movement speed
    public float movement_speed = 5.0f;

    // Storing the player's chosen horse
    public string chosen_horse = ""; // can be "blackhoof", "chromeblitz", "robotrotter", "nanomane", "thunderbyte"
    public int bet_amount = 0; // The amount the player is betting on
    
    // Storing neuroflux meter
    public int neuroflux_meter = 0; // Can go max 100

    // Where the UI for coins is stored
    // Look for Currency Image, which has a Text (TMP) that is a child object
    private TMPro.TMP_Text currencyText;

    // Item Ids
    [SerializeField] private ItemIDs item_ids = new ItemIDs();
    // Grab the item_database
    public Dictionary<int, ItemIDs.Item> item_database => item_ids.item_database;

    // NOTE:
    // Will eventually store all obtained items here PUBLICLY
    // because we will need to access this later for inventory
    // and other setup.

    // Grab the button for the DanceEmote m_Colors.m_DisabledColor
    [SerializeField] private UnityEngine.UI.Button danceEmoteButton;

    // Add player data such as interactions with certain NPC's
    // Default Npc's: 'deckmaster', 'casino_owner', 'shopkeeper', 'drunk_robot'
    // Storing the interaction_times with each NPC (starting at 0)
    [SerializeField] public Dictionary<string, int> npc_interactions = new Dictionary<string, int>()
    {
        {"deckmaster", 0},
        {"casino_owner", 0},
        {"shopkeeper", 0},
        {"drunkard", 0},
        {"sensei", 0}, // just for debug
        {"casinoenter", 0}
    };

    // Storing the current items the player has unlocked (list of item id's)
    [SerializeField] public List<int> unlocked_items = new List<int>();

    // Storing the current items the player is equipping right now
    // List of item id's
    [SerializeField] public List<int> equipped_items = new List<int>();

    // The original load items should be the items that the player has
    // when they first start the game. This is used to reset the player's
    // items to the original state.
    [SerializeField] public List<int> original_load_items = new List<int>();

    // A list of unlocked achievements
    [SerializeField] public List<int> unlocked_achievements = new List<int>(); // by the ids, 0 , 1, 2, etc...

    // AchievementFunction
    // This function will be called when the player completes an achievement
    public void UnlockAchievement(int achievement_id)
    {
        // If the achievement is not already unlocked, unlock it
        if (!unlocked_achievements.Contains(achievement_id))
        {
            unlocked_achievements.Add(achievement_id);
        }

        // Also call PopulatePanel on AchievementHandler
        // to update the achievements panel
        FindObjectOfType<AchievementsHandler>().PopulatePanel();

        // Call AchievementHandler's ShowAchievementUnlockedScreen
        // to show the achievement unlocked screen
        FindObjectOfType<AchievementsHandler>().ShowAchievementUnlockedScreen(achievement_id);
    }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // If there is no instance of PlayerData, set it to this
        if (Instance == null)
        {
            Instance = this;
        }
        // If there is an instance of PlayerData, destroy this
        else
        {
            Destroy(gameObject);
        }

        // Don't destroy this object when loading a new scene
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // If there is no equipped items within 500's range, grey out dance button (non-selectable)
        if (danceEmoteButton != null && !equipped_items.Exists(x => x >= 500 && x < 600))
        {
            danceEmoteButton.interactable = false;
        }

        // DEBUG:
        // Give the player all unlocked achievements from 0->11
        // for testing purposes
        // StartCoroutine(WaitAndUnlockAchievement(3.0f, 2));
        //     unlocked_achievements.Add(i);
        // }
    }

    // Wait and unlock achievement DEBUG
    // private IEnumerator WaitAndUnlockAchievement(float waitTime, int achievement_id)
    // {
    //     yield return new WaitForSeconds(waitTime);
    //     UnlockAchievement(achievement_id);
    // }

    // Update is called once per frame
    void Update()
    {
        // If ItemIDs is null, look for any ItemIDs in the scene
        if (item_ids == null)
        {
            item_ids = FindObjectOfType<ItemIDs>();
        }

        // If DanceEmoteButton is null, look for any DanceEmoteButton in the scene
        if (danceEmoteButton == null)
        {
            // The name must be "UI_Button_Dance"
            // If can find it (it's not on all scenes)
            if (GameObject.Find("UI_Button_Dance") != null)
            {
                danceEmoteButton = GameObject.Find("UI_Button_Dance").GetComponent<UnityEngine.UI.Button>();
            }
        }

        // For just showing how everything works, add all items to unlocked_items
        // if someone pressed Y key
        // DEBUG:::
        // if (Input.GetKeyDown(KeyCode.Y))
        // {
        //     Debug.Log("Adding all items to unlocked items");
        //     foreach (KeyValuePair<int, ItemIDs.Item> item in item_database)
        //     {
        //         if (!unlocked_items.Contains(item.Key))
        //         {
        //             unlocked_items.Add(item.Key);
        //         }
        //     }

        //     // Now call ItemIds FillAllInventoryButtons()
        //     item_ids.FillInventoryButtons();

        //     // Now, we want to make sure to unlock all achievements that aren't already unlocked
        //     for (int i = 0; i < 19; i++)
        //     {
        //         if (!unlocked_achievements.Contains(i))
        //         {
        //             unlocked_achievements.Add(i);
        //         }
        //     }
        // }


        // If any 0's in equipped_items, remove them
        if (equipped_items.Contains(0))
        {
            equipped_items.RemoveAll(x => x == 0);
        }
        // If dances are equipped, make the dance button interactable
        if (danceEmoteButton != null && equipped_items.Exists(x => x >= 500 && x < 600))
        {
            danceEmoteButton.interactable = true;
        } else if (danceEmoteButton != null && !equipped_items.Exists(x => x >= 500 && x < 600))
        {
            danceEmoteButton.interactable = false;
        }

        // Set the coins
        if (currencyText == null)
        {
            // currencyText = GameObject.Find("Currency-Text").GetComponent<TMPro.TMP_Text>();
            // If can find it (it's not on all scenes)
            if (GameObject.Find("Currency-Text") != null)
            {
                currencyText = GameObject.Find("Currency-Text").GetComponent<TMPro.TMP_Text>();
            }
        }

        if (currencyText != null)
        {
            currencyText.text = coins.ToString();
        }
    }
}

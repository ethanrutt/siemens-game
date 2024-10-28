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

    public static PlayerData Instance;
    
    // Storing public information like the current interactable
    public string interactable = "nig";

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
        {"drunk_robot", 0}
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
    }

    // Update is called once per frame
    void Update()
    {
        // If any 0's in equipped_items, remove them
        if (equipped_items.Contains(0))
        {
            equipped_items.RemoveAll(x => x == 0);
        }
        // If dances are equipped, make the dance button interactable
        if (danceEmoteButton != null && equipped_items.Exists(x => x >= 500 && x < 600))
        {
            danceEmoteButton.interactable = true;
        }
    }
}

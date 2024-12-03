using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Netcode
using Unity.Netcode;

// Rishi Santhanam
// Player data storage. All player data is stored here.
//
/**
 * @brief Rishi Santhanam - player data storage. All player data is stored here
 *
 * This includes the players username (used for api calls), coins, userid, etc.
 * This also tracks what npcs the player has talked to previously. as well as what areas they've previously visited
 *
 * These values are populated on login
 *
 * This is a singleton pattern.
 *
 * To use this in another script, grab the singleton instance by adding this
 * line as a member in your class
 * ```
 * private PlayerData playerData => PlayerData.Instance;
 * ```
 */
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
        {"deckmaster", 1},
        {"casino_owner", 1},
        {"shopkeeper", 0},
        {"drunkard", 0},
        {"sensei", 2}, // just for debug
        {"casinoenter", 1},
        {"labenter", 1}
    };

    // We are going to store the cards from the card game using a specific struct
    // This struct contains the card id, cost, power, and clientId
    private struct Card : INetworkSerializable, IEquatable<Card>
    {
        public int id;
        public int rarity;
        public int power;
        public enum Element { Electrical, Pressure, Heat };
        public string name;
        public UnityEngine.UI.Image image;
        public ulong clientId; // Add clientId field to track the source of the card data

        public Card(int id, int rarity, int power, Element element, string name, UnityEngine.UI.Image image, ulong clientId)
        {
            this.id = id;
            this.rarity = rarity;
            this.power = power;
            this.element = element;
            this.name = name;
            this.image = image;
            this.clientId = clientId;
        }

        // Implement IEquatable<Card>
        public bool Equals(Card other)
        {
            return id == other.id && cost == other.cost && power == other.power && clientId == other.clientId;
        }

        public override bool Equals(object obj)
        {
            return obj is Card other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + id.GetHashCode();
                hash = hash * 23 + cost.GetHashCode();
                hash = hash * 23 + power.GetHashCode();
                hash = hash * 23 + clientId.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Card left, Card right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Card left, Card right)
        {
            return !(left == right);
        }

        // Implement INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref cost);
            serializer.SerializeValue(ref power);
            serializer.SerializeValue(ref clientId); // Serialize clientId
        }
    };

    // Store the card images
    // and the dictionary, where we will populate the dictionary
    // and then we will populate the images on Awake
    [SerializeField] private List<UnityEngine.UI.Image> card_images = new List<UnityEngine.UI.Image>();
    private Dictionary<int, Card> cards = new Dictionary<int, Card> = {
        {0, new Card(0, 1, 3, Card.Element.Electrical, "Circuit Breaker", card_images[0], 0)},
        {1, new Card(1, 2, 6, Card.Element.Pressure, "Compression Burst", card_images[1], 0)},
        {2, new Card(2, 3, 8, Card.Element.Pressure, "Cyclone Force", card_images[2], 0)},
        {3, new Card(3, 4, 13, Card.Element.Electrical, "Electro Shield", card_images[3], 0)},
        {4, new Card(4, 1, 3, Card.Element.Pressure, "Gear Shift", card_images[4], 0)},
        {5, new Card(5, 4, 13, Card.Element.Heat, "Heat Eyes", card_images[5], 0)},
        {6, new Card(6, 2, 5, Card.Element.Heat, "Heatwave Radiator", card_images[6], 0)},
        {7, new Card(7, 3, 10, Card.Element.Pressure, "Hydraulic Crash", card_images[7], 0)},
        {8, new Card(8, 3, 8, Card.Element.Electrical, "Lights Off", card_images[8], 0)},
        {9, new Card(9, 1, 3, Card.Element.Heat, "Meltdown", card_images[9], 0)},
        {10, new Card(10, 3, 10, Card.Element.Heat, "Overheat", card_images[10], 0)},
        {11, new Card(11, 2, 6, Card.Element.Electrical, "Power Grid", card_images[11], 0)},
        {12, new Card(12, 4, 12, Card.Element.Pressure, "Pressure Disruptor", card_images[12], 0)},
        {13, new Card(13, 1, 4, Card.Element.Pressure, "Pressure Valve", card_images[13], 0)},
        {14, new Card(14, 2, 6, Card.Element.Heat, "Radiant Blaze", card_images[14],
        {15, new Card(15, 3, 11, Card.Element.Electrical, "Surge Strike", card_images[15], 0)},
        {16, new Card(16, 2, 5, Card.Element.Electrical, "Flux Grip", card_images[16], 0)},
        {17, new Card(17, 3, 8, Card.Element.Heat, "Welding Fury", card_images[17], 0)}
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
    }

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
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Adding all items to unlocked items");
            foreach (KeyValuePair<int, ItemIDs.Item> item in item_database)
            {
                if (!unlocked_items.Contains(item.Key))
                {
                    unlocked_items.Add(item.Key);
                }
            }

            // Now call ItemIds FillAllInventoryButtons()
            item_ids.FillInventoryButtons();

            // Now, we want to make sure to unlock all achievements that aren't already unlocked
            for (int i = 0; i < 19; i++)
            {
                if (!unlocked_achievements.Contains(i))
                {
                    unlocked_achievements.Add(i);
                }
            }
        }


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




// [Serializable]
// public struct CardData : INetworkSerializable, IEquatable<CardData>
// {
//     public int id;
//     public int cost;
//     public int power;
//     public ulong clientId; // Add clientId field to track the source of the card data

//     public CardData(int id, int cost, int power, ulong clientId)
//     {
//         this.id = id;
//         this.cost = cost;
//         this.power = power;
//         this.clientId = clientId;
//     }

//     // Implement IEquatable<CardData>
//     public bool Equals(CardData other)
//     {
//         return id == other.id && cost == other.cost && power == other.power && clientId == other.clientId;
//     }

//     public override bool Equals(object obj)
//     {
//         return obj is CardData other && Equals(other);
//     }

//     public override int GetHashCode()
//     {
//         unchecked
//         {
//             int hash = 17;
//             hash = hash * 23 + id.GetHashCode();
//             hash = hash * 23 + cost.GetHashCode();
//             hash = hash * 23 + power.GetHashCode();
//             hash = hash * 23 + clientId.GetHashCode();
//             return hash;
//         }
//     }

//     public static bool operator ==(CardData left, CardData right)
//     {
//         return left.Equals(right);
//     }

//     public static bool operator !=(CardData left, CardData right)
//     {
//         return !(left == right);
//     }

//     // Implement INetworkSerializable
//     public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//     {
//         serializer.SerializeValue(ref id);
//         serializer.SerializeValue(ref cost);
//         serializer.SerializeValue(ref power);
//         serializer.SerializeValue(ref clientId); // Serialize clientId
//     }
// }
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

// Rishi Santhanam
// ItemIDs.cs

// Defines all ItemIDs for the game
// This includes all items, dances, and unlockables


public class ItemIDs : MonoBehaviour
{
    // Each item will have an ID, a name, and a type
    // Types: "hat", "chest", "leggings", "shoes", "dance", "unlockable"

    // Will be defined in an item database/dictionary that will have an integer id linked to
    // a object struct that contains name, type, and other information
    
    // Create the object struct
    public struct Item
    {
        public string name; // The name of the item
        public string type; // "hat", "chest", "leggings", "shoes", "dance", "unlockable"
        public int id; // The main kxwey
        public int cost; // The cost of the item
    }

    // Create the item database
    // Hats start at 100, Chests start at 200, Leggings start at 300, Shoes start at 400, Dances start at 500, Unlockables start at 1000
    public Dictionary<int, Item> item_database = new Dictionary<int, Item>()
    {
        // Hats
        {100, new Item{name = "Orange Hard Hat", type = "hat", id = 100, cost = 125}},
        {101, new Item{name = "White Hard Hat", type = "hat", id = 101, cost = 125}},
        {102, new Item{name = "Yellow Hard Hat", type = "hat", id = 102, cost = 125}},
        {103, new Item{name = "Orange Party Hat", type = "hat", id = 103, cost = 250}},
        {104, new Item{name = "Purple Party Hat", type = "hat", id = 104, cost = 250}},
        {105, new Item{name = "Blue Party Hat", type = "hat", id = 105, cost = 250}},
        {106, new Item{name = "Cone", type = "hat", id = 106, cost = 500}},
        {107, new Item{name = "Medieval Helmet", type = "hat", id = 107, cost = 1000}},
        {108, new Item{name = "A&M Football Helmet", type = "hat", id = 108, cost = 0}},
        // Chest
        {200, new Item{name = "White Hoodie", type = "chest", id = 200, cost = 150}},
        {201, new Item{name = "Grey Hoodie", type = "chest", id = 201, cost = 150}},
        {202, new Item{name = "Black Hoodie", type = "chest", id = 202, cost = 150}},
        {203, new Item{name = "Blue Hoodie", type = "chest", id = 203, cost = 150}},
        {204, new Item{name = "Green Hoodie", type = "chest", id = 204, cost = 150}},
        {205, new Item{name = "Black Safety Vest", type = "chest", id = 205, cost = 450}},
        {206, new Item{name = "Grey Safety Vest", type = "chest", id = 206, cost = 450}},
        {207, new Item{name = "Orange Safety Vest", type = "chest", id = 207, cost = 450}},
        {208, new Item{name = "Medieval Armor", type = "chest", id = 208, cost = 1250}},
        // Leggings
        {300, new Item{name = "Black Utility Pants", type = "leggings", id = 300, cost = 300}},
        {301, new Item{name = "Blue Utility Pants", type = "leggings", id = 301, cost = 300}},
        {302, new Item{name = "Grey Utility Pants", type = "leggings", id = 302, cost = 300}},
        {303, new Item{name = "White Nano-Fiber Leggings", type = "leggings", id = 303, cost = 500}},
        {304, new Item{name = "Grey Nano-Fiber Leggings", type = "leggings", id = 304, cost = 500}},
        {305, new Item{name = "Black Nano-Fiber Leggings", type = "leggings", id = 305, cost = 500}},
        {306, new Item{name = "Medieval Leggings", type = "leggings", id = 306, cost = 1000}},
        // Footwear
        {400, new Item{name = "Brown Utility Boots", type = "shoes", id = 400, cost = 100}},
        {401, new Item{name = "Grey Utility Boots", type = "shoes", id = 401, cost = 100}},
        {402, new Item{name = "Black Utility Boots", type = "shoes", id = 402, cost = 100}},
        {403, new Item{name = "BW Jordan 1", type = "shoes", id = 403, cost = 500}},
        {404, new Item{name = "Retro Jordan 1", type = "shoes", id = 404, cost = 500}},
        {405, new Item{name = "UNC Jordan 1", type = "shoes", id = 405, cost = 500}},
        {406, new Item{name = "Medieval Boots", type = "shoes", id = 406, cost = 750}},
        // Dances
        {500, new Item{name = "Head-Ripper", type = "dance", id = 500, cost = 500}},
        {501, new Item{name = "Robot Dance", type = "dance", id = 501, cost = 500}},
        {502, new Item{name = "Zen Flip", type = "dance", id = 502, cost = 1200}},
    };

    // Type of item that is selected in inventory (either "all", "hat" ...)
    public string selectedType = "all";
    public string newselectedType = "all";

    // Set the selected type of item
    public void setHat()
    {
        selectedType = "hat";
    }

    public void setChest()
    {
        selectedType = "chest";
    }

    public void setLeggings()
    {
        selectedType = "leggings";
    }

    public void setShoes()
    {
        selectedType = "shoes";
    }

    public void setUnlockable() // includes dance
    {
        selectedType = "unlockable";
    }


    // Put all the item IDs and the sprites in separate Serializables
    // This will allow us to easily assign the sprites to the item IDs
    // This will be used in the inventory system
    [Serializable]
    public struct ItemSprite
    {
        public int id;
        public Sprite sprite;
    }

    // Create the item sprites
    [SerializeField] private ItemSprite[] itemSprites;

    // The UI buttons in the inventory
    [SerializeField] private GameObject[] inventoryButtons;

    // Grab the player's owned items and equippeditems from the playerData (is a singleton)
    private PlayerData playerData => PlayerData.Instance;

    // Function to get the sprite of an item
    public Sprite GetItemSprite(int itemID)
    {
        // Loop through all the item sprites
        for (int i = 0; i < itemSprites.Length; i++)
        {
            // If the item ID matches the item sprite ID
            if (itemID == itemSprites[i].id)
            {
                // Return the sprite
                return itemSprites[i].sprite;
            }
        }

        // If the item ID is not found, return null
        return null;
    }

    // Function to fill in the inventory buttons with the correct sprites
    // Only fill between 100-200 for hats
    // ... etc etc
    public void FillInventoryButtons()
    {
        List<int> ownedItems = playerData.unlocked_items;

        if (selectedType == "all")
        {
            for (int i = 0; i < inventoryButtons.Length; i++)
            {
                if (i < ownedItems.Count)
                {
                    int itemID = ownedItems[i];
                    Sprite itemSprite = GetItemSprite(itemID);
                    inventoryButtons[i].SetActive(true);
                    inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                    inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;
                    
                    // Capture the current value of i in a new local variable
                    int buttonIndex = i;

                    // Dynamically change the button's onClick event
                    inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                    // Add the new listener
                    inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() => 
                        inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                    );
                }
                else
                {
                    inventoryButtons[i].SetActive(false);
                }
            }
        } else if (selectedType == "hat") {

            List<int> displayItems = new List<int>();

            for (int i = 0; i < ownedItems.Count; i++)
            {
                if (ownedItems[i] >= 100 && ownedItems[i] < 200)
                {
                    displayItems.Add(ownedItems[i]);
                }
            }

            for (int i = 0; i < inventoryButtons.Length; i++)
            {
                if (i < displayItems.Count)
                {
                    int itemID = displayItems[i];
                    Sprite itemSprite = GetItemSprite(itemID);
                    inventoryButtons[i].SetActive(true);
                    inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                    inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;

                    // Capture the current value of i in a new local variable
                    int buttonIndex = i;

                    // Dynamically change the button's onClick event
                    inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                    // Add the new listener
                    inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() => 
                        inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                    );

                    // Debug
                    // Debug.Log("Button Index: " + buttonIndex);
                }
                else
                {
                    inventoryButtons[i].SetActive(false);
                }
            }

        } else if (selectedType == "chest") {
                
                List<int> displayItems = new List<int>();
    
                for (int i = 0; i < ownedItems.Count; i++)
                {
                    if (ownedItems[i] >= 200 && ownedItems[i] < 300)
                    {
                        displayItems.Add(ownedItems[i]);
                    }
                }
    
                for (int i = 0; i < inventoryButtons.Length; i++)
                {
                    if (i < displayItems.Count)
                    {
                        int itemID = displayItems[i];
                        Sprite itemSprite = GetItemSprite(itemID);
                        inventoryButtons[i].SetActive(true);
                        inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                        inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;

                        // Capture the current value of i in a new local variable
                        int buttonIndex = i;

                        // Dynamically change the button's onClick event
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                        // Add the new listener
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() => 
                            inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                        );
                    }
                    else
                    {
                        inventoryButtons[i].SetActive(false);
                    }
                }
    
            } else if (selectedType == "leggings") {
    
                List<int> displayItems = new List<int>();
    
                for (int i = 0; i < ownedItems.Count; i++)
                {
                    if (ownedItems[i] >= 300 && ownedItems[i] < 400)
                    {
                        displayItems.Add(ownedItems[i]);
                    }
                }
    
                for (int i = 0; i < inventoryButtons.Length; i++)
                {
                    if (i < displayItems.Count)
                    {
                        int itemID = displayItems[i];
                        Sprite itemSprite = GetItemSprite(itemID);
                        inventoryButtons[i].SetActive(true);
                        inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                        inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;

                        // Capture the current value of i in a new local variable
                        int buttonIndex = i;

                        // Dynamically change the button's onClick event
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                        // Add the new listener
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() => 
                            inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                        );
                    }
                    else
                    {
                        inventoryButtons[i].SetActive(false);
                    }
                }
    
            } else if (selectedType == "shoes") {
    
                List<int> displayItems = new List<int>();
    
                for (int i = 0; i < ownedItems.Count; i++)
                {
                    if (ownedItems[i] >= 400 && ownedItems[i] < 500)
                    {
                        displayItems.Add(ownedItems[i]);
                    }
                }
    
                for (int i = 0; i < inventoryButtons.Length; i++)
                {
                    if (i < displayItems.Count)
                    {
                        int itemID = displayItems[i];
                        Sprite itemSprite = GetItemSprite(itemID);
                        inventoryButtons[i].SetActive(true);
                        inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                        inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;

                        // Capture the current value of i in a new local variable
                        int buttonIndex = i;

                        // Dynamically change the button's onClick event
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                        // Add the new listener
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() => 
                            inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                        );
                    }
                    else
                    {
                        inventoryButtons[i].SetActive(false);
                    }
                }
        }
        else if (selectedType == "unlockable") {
    
            List<int> displayItems = new List<int>();
    
            for (int i = 0; i < ownedItems.Count; i++)
            {
                if (ownedItems[i] >= 500)
                {
                    displayItems.Add(ownedItems[i]);
                }
            }
    
            for (int i = 0; i < inventoryButtons.Length; i++)
            {
                if (i < displayItems.Count)
                {
                    int itemID = displayItems[i];
                    Sprite itemSprite = GetItemSprite(itemID);
                    inventoryButtons[i].SetActive(true);
                    inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                    inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;
                }
                else
                {
                    inventoryButtons[i].SetActive(false);
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // selectedType = all
        selectedType = "all";

        // DEBUG: Equip a dance
        // playerData.equipped_items.Add(500);

        if (!isSorted(playerData.unlocked_items))
        {
            playerData.unlocked_items.Sort();
        }

        // Fill in the inventoryButtons images with the
        // correct sprites
        FillInventoryButtons();

    }

    // Check sorted list
    private bool isSorted(List<int> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] < list[i - 1])
            {
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        // Make unlocked Items organized if not sorted
        if (!isSorted(playerData.unlocked_items))
        {
            playerData.unlocked_items.Sort();
        }

        // DEBUG LOG for selectedType
        // Debug.Log("Selected Type: " + selectedType);
        if (selectedType != newselectedType)
        {
            FillInventoryButtons();
            newselectedType = selectedType;
        }

        // DEBUG LOG
        // Debug.Log("Selected Type: " + selectedType);
    }
}

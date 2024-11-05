using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Rishi Santhanam
// Main Script for Managing the Shop
public class ShopManager : MonoBehaviour
{
    // Since this will only be on the Town Square
    // We will serialize all fields that need to be serialized
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private UnityEngine.UI.Button leftButton;
    [SerializeField] private UnityEngine.UI.Button rightButton;

    // PlayerData
    private PlayerData playerData => PlayerData.Instance;

    // Now, serialize the buttons for the Obj_Holders
    // These will be dynamically populated based on the items in the shop
    // and the items that the player does not have
    [SerializeField] private UnityEngine.UI.Button[] objHolders;
    [SerializeField] private GameObject itemTitle; // The title of the item
    [SerializeField] private GameObject itemCost; // The cost of the item
    [SerializeField] private GameObject itemRarity; // The rarity of the item

    // Purchase Button
    [SerializeField] private UnityEngine.UI.Button purchaseButton;

    // The DialoguePanel
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject blackoutPanel;

    // Serialize ItemIds Script
    [SerializeField] private ItemIDs itemIDs;

    // We need to keep a system that loads every item that the player has not
    // yet bought, and display it in the shop
    // if an item is bought, it will be removed from the shop and not be
    // displayed again. Edge cases (if there are most items bought in the shop)
    // then we will not show some of the objHolders.

    // The objHolders will be changed to the icons of the itemId, which is assigned
    // in the itemIds script as SerializeFields. 

    // The PlayerData has the unlocked_items which will be used to determine
    // which items to display or not to display
    private List<int> unlockedItems;

    // There are 5 object Holders, and clicking on the right button will
    // change pages. The pages will be determined by the number of items
    // that the player has not bought yet.

    // We will hold the pages in a list of lists
    private List<List<int>> pages = new List<List<int>>();

    // The current page
    private int currentPage = 0;

    // Populate Lists based on unlockedItems
    private void PopulateLists()
    {
        // Clear the pages
        pages.Clear();

        // Create a new list to hold items for the current page
        List<int> currentPageList = new List<int>();

        // Loop through all the items in ItemIds, and if that item
        // is not in the unlockedItems, add it to the currentPageList
        foreach (KeyValuePair<int, ItemIDs.Item> item in itemIDs.item_database)
        {
            if (!unlockedItems.Contains(item.Key))
            {
                currentPageList.Add(item.Key);

                // If the current page list reaches 5 items, add it to pages and start a new list
                if (currentPageList.Count == 5)
                {
                    pages.Add(new List<int>(currentPageList));
                    currentPageList.Clear();
                }
            }
        }

        // Add any remaining items to the pages
        if (currentPageList.Count > 0)
        {
            pages.Add(currentPageList);
        }

        // If there are no pages, then the player has bought all the items
        // and there is no need to display the shop
        if (pages.Count == 0)
        {
            // Set the player's interactable to "shopclosed"
            playerData.interactable = "shopclosed";
        }
    }

    private int clicked_item_id = -1;
    private string clicked_item_name = "";
    private int clicked_item_cost = 0;
    private string clicked_item_rarity = "";

    public void SetClickedItem(int item_id)
    {
        // To choose what to display in the shop.
        clicked_item_id = item_id;
        clicked_item_name = itemIDs.item_database[item_id].name;
        clicked_item_cost = itemIDs.item_database[item_id].cost;

        // Get the rarity of the item
        GetRarity(item_id);
    }

    private void GetRarity(int item_id)
    {
        // Get the rarity of the item
        // and display it in the shop
        // essentially, for items < 200, it is common
        // for items in between 200-430, it is rare
        // then 430-700 is epic, and 700+ is legendary
        // essentially search the item_id's cost that it's linked to in itemIds

        int cost = itemIDs.item_database[item_id].cost;

        if (cost < 200)
        {
            clicked_item_rarity = "Common";
        }
        else if (cost >= 200 && cost < 430)
        {
            clicked_item_rarity = "Rare";
        }
        else if (cost >= 430 && cost < 700)
        {
            clicked_item_rarity = "Epic";
        }
        else
        {
            clicked_item_rarity = "Legendary";
        }
    }

    // Purchase Button
    public void Purchase()
    {
        // If the player has enough money
        if (playerData.coins >= clicked_item_cost)
        {
            // Deduct the money
            playerData.coins -= clicked_item_cost;

            // Add the item to the player's inventory
            playerData.unlocked_items.Add(clicked_item_id);

            // Set the clicked item to -1
            clicked_item_id = -1;

            // Deactivate the purchase button
            purchaseButton.interactable = false;

            // Update the pages
            PopulateLists();

            // EdgeCase: If the player has bought the only item on
            // the last page, then we need to decrement the current page
            if (currentPage == pages.Count && currentPage != 0)
            {
                currentPage--;
            }

            // Update the objHolders
            UpdateObjHolders();

            // Call ItemIds function to fillinventorybuttons
            // after sorting the unlocked_items
            playerData.unlocked_items.Sort();
            itemIDs.FillInventoryButtons();
        }
    }

    // Start
    private void Start()
    {
        // Set players items
        unlockedItems = playerData.unlocked_items;

        // Populate the lists
        PopulateLists();

        // DEBUG:!
        // for (int pageIndex = 0; pageIndex < pages.Count; pageIndex++)
        // {
        //     foreach (int id in pages[pageIndex])
        //     {
        //     Debug.Log("Page: " + pageIndex + " ID: " + id);
        //     }
        // }

        // Update the objHolders
        UpdateObjHolders();
    }

    // Update the objHolders
    private void UpdateObjHolders()
    {
    // Check which items to display based on the current page
    List<int> currentPageList = pages[currentPage];

    for (int i = 0; i < objHolders.Length; i++)
    {
        // If the current page list has an item
        if (i < currentPageList.Count)
        {
            // Set the objHolder button to active
            objHolders[i].gameObject.SetActive(true);

            // Get the item id from the current page list
            int itemId = currentPageList[i];

            // Set the sprite for the button image using the GetItemSprite method
            Image buttonImage = objHolders[i].GetComponent<Image>();
            buttonImage.sprite = itemIDs.GetItemSprite(itemId);  // Update sprite

            // Clear previous listeners (important to avoid stacking listeners)
            objHolders[i].onClick.RemoveAllListeners();

            // Dynamically set the onClick listener for this button
            int buttonIndex = i; // Capture the current index (to avoid issues with closures)
            objHolders[i].onClick.AddListener(() => SetClickedItem(currentPageList[buttonIndex]));
        }
        else
        {
            // Set the objHolder button to inactive if there's no item to display
            objHolders[i].gameObject.SetActive(false);
        }
    }
    }


    

    public void NextPage()
    {
        // If the current page is not the last page
        if (currentPage != pages.Count - 1)
        {
            // Increment the current page
            currentPage++;

            // Update the objHolders
            UpdateObjHolders();
        }
    }

    public void PreviousPage()
    {
        // If the current page is not the first page
        if (currentPage != 0)
        {
            // Decrement the current page
            currentPage--;

            // Update the objHolders
            UpdateObjHolders();
        }
    }

    // Update
    private void Update()
    {
        // If the currentPage == 0, disable the left button (it's a UI button so grey)
        // not interactable
        if (currentPage == 0)
        {
            leftButton.interactable = false;
        }
        else
        {
            leftButton.interactable = true;
        }

        // If the currentPage == pages.Count - 1, disable the right button
        if (currentPage == pages.Count - 1)
        {
            rightButton.interactable = false;
        }
        else
        {
            rightButton.interactable = true;
        }

        // Set the item title to the name of the item
        // and the cost to the cost of the item
        if (clicked_item_id != -1)
        {
            itemTitle.GetComponent<TextMeshProUGUI>().text = itemIDs.item_database[clicked_item_id].name;
            itemCost.GetComponent<TextMeshProUGUI>().text = itemIDs.item_database[clicked_item_id].cost.ToString();
            GetRarity(clicked_item_id);

            // Set the rarity of the item
            itemRarity.GetComponent<TextMeshProUGUI>().text = clicked_item_rarity;

            // Purchase button is interactable if the player has enough money
            if (playerData.coins >= clicked_item_cost)
            {
                purchaseButton.interactable = true;
            }
            else
            {
                purchaseButton.interactable = false;
            }
        }

        // If the player is in the shop
        if (playerData.interactable == "shopopen")
        {
            // Set the shop panel to active
            shopPanel.SetActive(true);
            blackoutPanel.SetActive(true);
        }
        else
        {
            // Set the shop panel to inactive
            shopPanel.SetActive(false);
            blackoutPanel.SetActive(false);
        }
    }

    // CloseShop
    public void CloseShop()
    {
        // Set the player's interactable to "none"
        playerData.interactable = "none";

        // Turn off shop
        shopPanel.SetActive(false);
        blackoutPanel.SetActive(false);
    }

}

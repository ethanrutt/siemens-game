using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TMP
using TMPro;

public class AchievementsHandler : MonoBehaviour
{
    // Grab playerData
    private PlayerData playerData => PlayerData.Instance;
    //[SerializeField] public List<string> unlocked_achievements = new List<int>(); // by the ids, 0 , 1, 2, etc... is on PlayerData

    // Grab itemIDS
    [SerializeField] private ItemIDs itemIDS;

    // Grab the achievements panel
    [SerializeField] private GameObject achievementsPanel;

    // Grab the playerMovement
    [SerializeField] private Character_Movement playerMovement;

    // Grab the blackoutModal
    [SerializeField] private GameObject blackoutModal;
    
    // Grab the UI Object (because we're going to) deactivate it
    [SerializeField] private GameObject uiObject;

    // Grab the left and right UI buttons
    [SerializeField] private UnityEngine.UI.Button leftButton;
    [SerializeField] private UnityEngine.UI.Button rightButton; // These will be greyed out depending on the pages

    // Grab the array of UI Buttons for the lore objects
    [SerializeField] private UnityEngine.UI.Button[] loreButtons;

    // Have a page counter
    private int pageCounter = 0;

    // Grab the lore object panel, the lore title, sender, recipients, and body
    // We will display everything from this script.
    [SerializeField] private GameObject loreObjectPanel;
    [SerializeField] private TMPro.TMP_Text loreTitle;
    [SerializeField] private TMPro.TMP_Text loreSender;
    [SerializeField] private TMPro.TMP_Text loreRecipients;
    [SerializeField] private TMPro.TMP_Text loreBody;

    // Grab the texts from the achievements (there are only 3, and 2 each)
    [SerializeField] private TMPro.TMP_Text[] achievementTitles;
    [SerializeField] private TMPro.TMP_Text[] achievementDescriptions;

    // Grab the texts from the lore object
    private string loreTitleText;
    private string loreSenderText;
    private string loreRecipientsText;
    private string loreBodyText;

    // Grab the GameObject[] rectangleHolders
    [SerializeField] private GameObject[] rectangleHolders;

    // Open lore panel
    public void OpenLorePanel(int loreID)
    {
        // Set the lore title, sender, recipients, and body
        loreTitleText = itemIDS.lore_database[loreID].subject;
        loreSenderText = itemIDS.lore_database[loreID].sender;
        loreRecipientsText = string.Join(", ", itemIDS.lore_database[loreID].receivers);
        loreBodyText = itemIDS.lore_database[loreID].body;

        // Set the texts
        loreTitle.text = loreTitleText;
        loreSender.text = "From: " + loreSenderText;
        loreRecipients.text = "To: " + loreRecipientsText;
        loreBody.text = loreBodyText;

        // Close achievements panel
        achievementsPanel.SetActive(false);
        // open lore panel
        loreObjectPanel.SetActive(true);
    }

    // Close lore panel
    public void CloseLorePanel()
    {
        loreObjectPanel.SetActive(false);
        // open achievements panel
        achievementsPanel.SetActive(true);
    }

    // Hold pages in a list of lists (int) for achievement IDS
    private List<List<int>> achievementPages = new List<List<int>>(); // There are only 3 achievements per page

    // OpenPanel
    public void OpenAchievementsPanel()
    {
        achievementsPanel.SetActive(true);
        playerMovement.StopPlayer();
        blackoutModal.SetActive(true);
        uiObject.SetActive(false);
    }

    public void CloseAchievementsPanel()
    {
        achievementsPanel.SetActive(false);
        playerMovement.UnstopPlayer();
        blackoutModal.SetActive(false);
        uiObject.SetActive(true);
    }

    // Populate the lists of achievements
    private void PopulateLists()
    {
        // Loop through all the achievements
        foreach (KeyValuePair<int, ItemIDs.Achievement> achievement in itemIDS.achievement_database)
        {
            // Add the achievement to the list
            achievementPages[achievementPages.Count - 1].Add(achievement.Key);

            // If the list is full, create a new list
            if (achievementPages[achievementPages.Count - 1].Count == 3)
            {
                achievementPages.Add(new List<int>());
            }
        }
    }

    // Populate achievements panel with the pages (similar to ShopManager)
    // and populate the buttons with open(loreID)
    private void PopulatePanel()
    {
        // Basically, will populate the texts 
        // and the buttons with the loreID
        // for each achievement
        for (int i = 0; i < achievementPages[pageCounter].Count; i++)
        {
            // Set the achievement title and description
            achievementTitles[i].text = itemIDS.achievement_database[achievementPages[pageCounter][i]].title;
            achievementDescriptions[i].text = itemIDS.achievement_database[achievementPages[pageCounter][i]].description;

            // in the cases where there is one achievement on that page, the other two achievements below should be blank
            if (i == 0 && achievementPages[pageCounter].Count == 1)
            {
                achievementTitles[1].text = "";
                achievementDescriptions[1].text = "";
                achievementTitles[2].text = "";
                achievementDescriptions[2].text = "";

                // disable rectangle holder 2 and 3
                rectangleHolders[1].SetActive(false);
                rectangleHolders[2].SetActive(false);
            } else if (i == 1 && achievementPages[pageCounter].Count == 2)
            {
                achievementTitles[2].text = "";
                achievementDescriptions[2].text = "";

                // disable rectangle holder 3
                rectangleHolders[2].SetActive(false);
            } else if (i == 2 && achievementPages[pageCounter].Count == 3)
            {
                // set all rectangle holders to active
                rectangleHolders[1].SetActive(true);
                rectangleHolders[2].SetActive(true);
            }

            // Set the button's onClick event
            int loreID = itemIDS.achievement_database[achievementPages[pageCounter][i]].lore_linker;
            loreButtons[i].onClick.RemoveAllListeners();
            loreButtons[i].onClick.AddListener(() => OpenLorePanel(loreID));

            // If the achievement is not unlcoked, interactable=false for
            // the button
            if (!playerData.unlocked_achievements.Contains(achievementPages[pageCounter][i]))
            {
                loreButtons[i].interactable = false;
            } else
            {
                loreButtons[i].interactable = true;
            }
        }

        // If the page counter is 0, disable the left button
        if (pageCounter == 0)
        {
            leftButton.interactable = false;
        } else
        {
            leftButton.interactable = true;
        }

        // If the page counter is the last page, disable the right button
        if (pageCounter == achievementPages.Count - 1)
        {
            rightButton.interactable = false;
        } else
        {
            rightButton.interactable = true;
        }
    }
    
    // PageBack
    public void PageBack()
    {
        // Decrement the page counter
        pageCounter--;

        // Populate the panel
        PopulatePanel();
    }

    // PageForward
    public void PageForward()
    {
        // Increment the page counter
        pageCounter++;

        // Populate the panel
        PopulatePanel();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Populate the lists
        achievementPages.Add(new List<int>());
        PopulateLists();
        PopulatePanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

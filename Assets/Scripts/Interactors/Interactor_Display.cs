using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scene Manager
using UnityEngine.SceneManagement;

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
        } else {
            // Debug.Log("No Interactable");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

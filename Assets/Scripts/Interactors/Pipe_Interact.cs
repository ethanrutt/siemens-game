using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe_Interact : MonoBehaviour
{
    // The player object
    // The player object has its own script that stores player data
    // so we will tap into that public string interactable
    // and change it to "leaderboard"
    [SerializeField] public GameObject player;
    [SerializeField] private GameObject interactButton;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("Leaderboard Interact script loaded");
    }

    // Update is called once per frame
    // If the player walks into the collider for leaderboard,
    // we will change the player's interactable to "leaderboard"
    // and if walks away, the interactable is nothing, and the
    // interact button disappears.
    void Update()
    {
        
    }

    // Grab the PlayerData object (this is a DoNotDestroyOnLoad object)
    // and set the player object to the player object in the scene
    // The PlayerData is a singelton




    // When the player walks into the collider for the leaderboard,
    // we will change the player's interactable to "leaderboard"
    // and show the interact button.
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerData.Instance.interactable = "pipegame";
        interactButton.SetActive(true);
    }

    // When the player walks away from the collider for the leaderboard,
    // we will change the player's interactable to nothing
    // and hide the interact button.
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerData.Instance.interactable = "";
        interactButton.SetActive(false);
    }
}
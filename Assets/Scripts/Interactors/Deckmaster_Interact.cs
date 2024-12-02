using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class Deckmaster_Interact
 * @brief This class handles the interaction between the player and the Deckmaster.
 * @details This class is attached to the Deckmaster GameObject in the scene.
 * When the player walks into the collider for the Deckmaster, the player's
 * interactable is set to "deckmaster" and the interact button is shown. When
 * the player walks away from the collider for the Deckmaster, the player's
 * interactable is set to nothing and the interact button is hidden.
 */
public class Deckmaster_Interact : MonoBehaviour
{
    // The player object
    // The player object has its own script that stores player data
    // so we will tap into that public string interactable
    // and change it to "leaderboard"
    [SerializeField] public GameObject player;
    [SerializeField] private GameObject interactButton;

    // When the player walks into the collider for the leaderboard,
    // we will change the player's interactable to "leaderboard"
    // and show the interact button.
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerData.Instance.interactable = "deckmaster";
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class Leaderboard_Interact
 * @brief This handles the interact button when you get close to the
 * leaderboard area in the laboratory
 * @details Consists of colliders and collider triggers that display the
 * button to pull up the leaderboard modal
 */
public class Leaderboard_Interact : MonoBehaviour
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
        PlayerData.Instance.interactable = "leaderboard";
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

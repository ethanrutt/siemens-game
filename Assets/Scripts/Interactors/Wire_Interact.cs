using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class Wire_Interact
 * @brief This handles display of the interact button for entering the wire
 * game when getting near the wire game area.
 */
public class Wire_Interact : MonoBehaviour
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
        PlayerData.Instance.interactable = "wiregame";
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

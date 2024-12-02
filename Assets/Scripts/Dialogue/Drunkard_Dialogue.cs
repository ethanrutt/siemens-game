using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This handles the business logic for the drunkard on top of the
 * building. The actual dialogue sentences are in DialogueManager_TS
 *
 * @see DialogueManager
 * @see DialogueManager_TS
 */
public class Drunkard_Dialogue : MonoBehaviour
{
    // PlayerData global singleton
    private PlayerData playerData => PlayerData.Instance;

    // Find interact button and set it to active
    [SerializeField] private GameObject interactButton;

    // OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Set interactable to "drunkard"
        playerData.interactable = "drunkard";
        interactButton.SetActive(true);
    }

    // OnTriggerExit2D
    private void OnTriggerExit2D(Collider2D other)
    {
        playerData.interactable = null;
        interactButton.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find UI_Button_Interact
        if (interactButton == null)
        {
            interactButton = GameObject.Find("UI_Button_Interact");
        }
    }
}

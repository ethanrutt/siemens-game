using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTutorial : MonoBehaviour
{

    // Grab playerData
    public PlayerData playerData => PlayerData.Instance;

    // Grab gameManager
    public GameManager gameManager => GameManager.Instance;

    // Add the unlocked_items 108
    // Take to the Laboratory Scene
    public void TakeToTownSquare()
    {
        // Get the unlocked_items
        playerData.unlocked_items.Add(108);

        // Change the player's spawn position -30.1,20.49
        gameManager.ChangePlayerSpawnPosition(new Vector2(-30.1f, 20.49f));

        // Load the laboratory scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Town_Square");
    }
}

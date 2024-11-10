using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rishi Santhanam
// The Game Manager script is responsible for managing the game state.
// This allows for states to persist between scenes.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Vector2 playerSpawnPosition;

    // Now the Awake() method
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Now changing the player's spawn position
    public void ChangePlayerSpawnPosition(Vector2 newPosition)
    {
        playerSpawnPosition = newPosition;
    }
}
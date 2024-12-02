using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Rishi Santhanam
  * @brief The Game Manager script is responsible for managing the game state.
  * This allows for states to persist between scenes.
  * This is a singleton class.
  * The main part of this class is the `playerSpawnPosition` member, which
  * holds the players position. This is mainly used in transitions between
  * scenes so that we can spawn the player back where they were before they
  * entered the scene
  *
  * i.e. This code snippet will change the spawn position so that when we
  * enter into the lab we are in the right position
  * ```
  * gameManager.ChangePlayerSpawnPosition(new Vector2(-33f, 10.25f));
  * SceneManager.LoadScene("Laboratory_Main");
  * ```
  */
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

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class CheckLaboratory
{
    // A Test behaves as an ordinary method
    [UnityTest]
    public IEnumerator CheckLaboratorySimplePasses()
    {
        // Load Main Menu Scene to get Player Data singleton loaded
        yield return SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);

        // Wait for one frame to ensure the scene is fully loaded
        yield return null;

        // Load the Town_Square scene
        yield return SceneManager.LoadSceneAsync("Laboratory_Main", LoadSceneMode.Additive);

        // Wait for one frame to ensure the scene is fully loaded
        yield return null;

        // Find the Player_Object in the scene
        GameObject player = GameObject.Find("Player_Object");

        // Check if the Player_Object is present
        Assert.IsNotNull(player, "player not found in Lab scene");
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Unload the Town_Square scene
        yield return SceneManager.UnloadSceneAsync("Laboratory_Main");

        // Wait for one frame to ensure the scene is fully unloaded
        yield return null;

        // Unload MainMenu scene
        yield return SceneManager.UnloadSceneAsync("MainMenu");

        // Wait for one frame to ensure the scene is fully unloaded
        yield return null;

    }
}

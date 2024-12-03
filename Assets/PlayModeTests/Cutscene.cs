using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class Cutscene
{
    // A Test behaves as an ordinary method
    [UnityTest]
    public IEnumerator CheckCutsceneSimplePasses()
    {
        // Load Main Menu Scene to get Player Data singleton loaded
        yield return SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);

        // Wait for one frame to ensure the scene is fully loaded
        yield return null;

        // Load the Town_Square scene
        yield return SceneManager.LoadSceneAsync("Starting-Cutscene", LoadSceneMode.Additive);

        // Wait for one frame to ensure the scene is fully loaded
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Unload the Town_Square scene
        yield return SceneManager.UnloadSceneAsync("Starting-Cutscene");

        // Wait for one frame to ensure the scene is fully unloaded
        yield return null;

        // Unload MainMenu scene
        yield return SceneManager.UnloadSceneAsync("MainMenu");

        // Wait for one frame to ensure the scene is fully unloaded
        yield return null;

    }
}

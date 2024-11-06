using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

// We're going to test that the TownSquare has the Player_Object
[TestFixture]
public class CheckTownSquare : MonoBehaviour
{
    // We are going to load the Town_Square scene
    // and then we will check if the Player_Object is present
    [UnityTest]
    public IEnumerator CheckTownSquareForPlayerObject()
    {
        // Load the Town_Square scene
        yield return SceneManager.LoadSceneAsync("Town_Square");

        // Wait for one frame to ensure the scene is fully loaded
        yield return null;

        // Find the Player_Object in the scene
        GameObject playerObject = GameObject.Find("Player_Object");

        // Check if the Player_Object is present
        Assert.IsNotNull(playerObject, "Player_Object not found in Town_Square scene");
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Unload the Town_Square scene
        yield return SceneManager.UnloadSceneAsync("Town_Square");

        // Wait for one frame to ensure the scene is fully unloaded
        yield return null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class Laboratory_RoomEnterExit
 * @brief This handles the enter and exit script for the laboratory room which
 * allows players to see what area of the room they are in
 * @details on entering a new area, some text at the bottom will be displayed that tells which section you're in.
 * Room ID Dictionary:
 * - 0 -> Automation Station (Top left)
 * - 1 -> Synthesis Chamber (Top right)
 * - 2 -> Main Hallway (Middle)
 * - 3 -> Data Center (Bottom left)
 * - 4 -> Digital Command Room (Middle bottom)
 * - 5 -> Integration Zone (Bottom right)
 * TOWN SQUARE
 * - 10 -> The Reclamation Zone
 * - 11 -> Casino
 * - 12 -> Scavenger Shop
 * - 13 -> Tipsy Tower
 * - 14 -> Laboratory
 */
public class Laboratory_RoomEnterExit : MonoBehaviour
{

    // Serialize the player and the UI text fields
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject currentRoomObject;
    [SerializeField] private GameObject text_Title;
    [SerializeField] private GameObject text_Description;

    // Serialize the roomID
    [SerializeField] private string roomID;


    // Fading text
    public IEnumerator FadeTextToZeroAlpha(float t, TMPro.TextMeshProUGUI text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / t));
            yield return null;
        }

        // Set the current room object to inactive
        currentRoomObject.SetActive(false);
    }

    // Fading in
    public IEnumerator FadeTextToFullAlpha(float t, TMPro.TextMeshProUGUI text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    // On a 2D colliderEnter (.setText() TMP) NOTE THAT TITLE NEVER CHANGES, ONLY DESCRIPTION
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the player enters the collider
        // Set currentRoomObject (which stores the current room object) to active
        currentRoomObject.SetActive(true);

        // Stop all coroutines
        StopAllCoroutines();

            // Debug.Log("Player entered room: " + roomID);
            switch (roomID)
            {
                case "0":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("LABORATORY");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Automation Station");
                    break;
                case "1":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("LABORATORY");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Synthesis Chamber");
                    break;
                case "2":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("LABORATORY");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Main Hallway");
                    break;
                case "3":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("LABORATORY");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Data Center");
                    break;
                case "4":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("LABORATORY");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Digital Command Room");
                    break;
                case "5":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("LABORATORY");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Integration Zone");
                    break;
                case "10":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("TOWN SQUARE");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("The Reclamation Zone");
                    break;
                case "11":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("TOWN SQUARE");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Casino");
                    break;
                case "12":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("TOWN SQUARE");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Scavenger Shop");
                    break;
                case "13":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("TOWN SQUARE");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Tipsy Tower");
                    break;
                case "14":
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("TOWN SQUARE");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Laboratory");
                    break;
                default:
                    text_Title.GetComponent<TMPro.TextMeshProUGUI>().SetText("Error");
                    text_Description.GetComponent<TMPro.TextMeshProUGUI>().SetText("Error");
                    break;
            }
            StartCoroutine(FadeTextToFullAlpha(1f, text_Title.GetComponent<TMPro.TextMeshProUGUI>()));
            StartCoroutine(FadeTextToFullAlpha(1f, text_Description.GetComponent<TMPro.TextMeshProUGUI>()));
    }

    // On a 2D colliderExit (.setText() TMP)
    private void OnTriggerExit2D(Collider2D other)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTextToZeroAlpha(1f, text_Title.GetComponent<TMPro.TextMeshProUGUI>()));
        StartCoroutine(FadeTextToZeroAlpha(1f, text_Description.GetComponent<TMPro.TextMeshProUGUI>()));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HorseBehavior : MonoBehaviour
{
    // Grab all HorseObjects (they're Unity UI Images)
    [SerializeField] private GameObject[] horseObjects;

    private Vector2[] horseStartPositions;

    // Grab the PlayerData
    private PlayerData playerData => PlayerData.Instance;

    // Grab the flags gameObjects
    [SerializeField] private GameObject[] flags;

    [SerializeField] private TMP_Text horseInLead;

    public float[] slowSpeeds =
    {
        20f, 22.5f, 25f, 27.5f, 30f, 32.5f, 35f, 37.5f, 40f, 42.5f, 45f, 47.5f, 50f
    };

    public float[] mediumSpeeds =
    {
        55f, 60f, 65f, 70f, 75f, 80f, 85f, 90f, 95f, 100f, 105f, 110f, 115f
    };

    public float[] fastSpeeds =
    {
        120f, 130f, 140f, 150f, 160f, 170f, 180f, 190f, 200f, 210f, 220f, 230f, 240f
    };

    // Grab the players neuroflux level. It goes up to 100 (think of percent, but its an integer)
    private int neurofluxLevel;

    // Grab the player's chosen horse
    private string chosen_horse_; // if it's "blackhoof", "chromeblitz", "robotrotter", "nanomane", "thunderbyte"
    private int betAmount;

    private bool horseCrossedFinishLine;

    [SerializeField] Canvas GameScreen;
    [SerializeField] Canvas GameOverScreen;
    [SerializeField] TMP_Text horseWinnerText;
    [SerializeField] TMP_Text coinsLostOrGainedText;
    
    // RandomSpeed function
    private float RandomSpeed()
    {
        // Check if the player is on neuroflux
        bool isFluxed = neurofluxLevel > 0;

        // If the player has 25 neuroflux, they have a 25% chance of getting a speed boost
        // If the player has 50 neuroflux, they have a 50% chance of getting a speed boost etc
        if (isFluxed)
        {
            // Random number between 0 and 100
            int rand = Random.Range(0, 100);

            // Generate a random number that we will test with the neuroflux
            int test = Random.Range(0, 100);

            // If test > neurofluxLevel, then we will return a random speed
            if (test > neurofluxLevel)
            {
                // Return something from small or medium with a 50% chance of each
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    return mediumSpeeds[Random.Range(0, mediumSpeeds.Length)];
                }
                else
                {
                    return slowSpeeds[Random.Range(0, slowSpeeds.Length)];
                }
            }
            else
            {
                // If the player is on neuroflux, they have a 50% chance of getting a speed boost
                // If they do get a speed boost, we will return a random speed
                return fastSpeeds[Random.Range(0, fastSpeeds.Length)];
            }
        }
        else {
            // If the player is not on neuroflux, they have a 33% chance of being slow, 33% chance of being medium, 33% chance of being fast
            int random = Random.Range(0, 3);

            if (random == 0)
            {
                return slowSpeeds[Random.Range(0, slowSpeeds.Length)];
            }
            else if (random == 1)
            {
                return mediumSpeeds[Random.Range(0, mediumSpeeds.Length)];
            }
            else
            {
                return fastSpeeds[Random.Range(0, fastSpeeds.Length)];
            }
        }
    }

    private float NonChosenHorseSpeed()
    {
        // If this a horse the player has not chosen, 33% chance of being slow, 33% chance of being medium, 33% chance of being fast
        int random = Random.Range(0, 3);

        if (random == 0)
        {
            return slowSpeeds[Random.Range(0, slowSpeeds.Length)];
        }
        else if (random == 1)
        {
            return mediumSpeeds[Random.Range(0, mediumSpeeds.Length)];
        }
        else
        {
            return fastSpeeds[Random.Range(0, fastSpeeds.Length)];
        }
    }

    void Start()
    {
        neurofluxLevel = playerData.neuroflux_meter;
        chosen_horse_ = playerData.chosen_horse;
        betAmount = playerData.bet_amount;
        horseCrossedFinishLine = false;

        horseStartPositions = new Vector2[horseObjects.Length];

        for (int i = 0; i < horseObjects.Length; i++)
        {
            horseStartPositions[i] = horseObjects[i].GetComponent<RectTransform>().anchoredPosition;
        }
    }

    // Update to check if the horse has reached the finish line
    void Update()
    {
        if (horseCrossedFinishLine)
        {
            return;
        }

        // Check if the horse has reached the finish line
        for (int i = 0; i < horseObjects.Length; i++)
        {
            if (horseObjects[i].transform.position.x >= flags[i].transform.position.x)
            {
                horseCrossedFinishLine = true;

                string winningHorse = "";
                switch (i)
                {
                    case 0:
                        winningHorse = "Blackhoof";
                        break;
                    case 1:
                        winningHorse = "Chromeblitz";
                        break;
                    case 2:
                        winningHorse = "Robotrotter";
                        break;
                    case 3:
                        winningHorse = "Nanomane";
                        break;
                    case 4:
                        winningHorse = "Thunderbyte";
                        break;
                }

                EndGame(winningHorse);
            }
        }

        if (!horseCrossedFinishLine)
        {
            MoveHorses();
            UpdateLeadHorse();
        }
    }

    public void UpdateLeadHorse()
    {
        int horseInLeadIdx = 0;
        for (int i = 0; i < horseObjects.Length; i++)
        {
            if (horseObjects[i].transform.position.x > horseObjects[horseInLeadIdx].transform.position.x)
            {
                horseInLeadIdx = i;
            }
        }

        switch (horseInLeadIdx)
        {
            case 0:
                horseInLead.text = "Blackhoof is in the lead!";
                break;
            case 1:
                horseInLead.text = "Chromeblitz is in the lead!";
                break;
            case 2:
                horseInLead.text = "Robotrotter is in the lead!";
                break;
            case 3:
                horseInLead.text = "Nanomane is in the lead!";
                break;
            case 4:
                horseInLead.text = "Thunderbyte is in the lead!";
                break;
        }
    }

    public void MoveHorses()
    {
        Debug.Log("moving horses");
        if (chosen_horse_ == "blackhoof")
        {
            horseObjects[0].transform.position += Vector3.right * RandomSpeed() * Time.deltaTime;
            // Make the rest of them NonChosen
            horseObjects[1].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[2].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[3].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[4].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
        }
        else if (chosen_horse_ == "chromeblitz")
        {
            horseObjects[1].transform.position += Vector3.right * RandomSpeed() * Time.deltaTime;
            // Make the rest of them NonChosen
            horseObjects[0].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[2].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[3].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[4].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
        }
        else if (chosen_horse_ == "robotrotter")
        {
            horseObjects[2].transform.position += Vector3.right * RandomSpeed() * Time.deltaTime;
            // Make the rest of them NonChosen
            horseObjects[0].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[1].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[3].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[4].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
        }
        else if (chosen_horse_ == "nanomane")
        {
            horseObjects[3].transform.position += Vector3.right * RandomSpeed() * Time.deltaTime;
            // Make the rest of them NonChosen
            horseObjects[0].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[1].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[2].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[4].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
        }
        else if (chosen_horse_ == "thunderbyte")
        {
            horseObjects[4].transform.position += Vector3.right * RandomSpeed() * Time.deltaTime;
            // Make the rest of them NonChosen
            horseObjects[0].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[1].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[2].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
            horseObjects[3].transform.position += Vector3.right * NonChosenHorseSpeed() * Time.deltaTime;
        }
    }

    public void EndGame(string winningHorse)
    {
        GameScreen.gameObject.SetActive(false);
        GameOverScreen.gameObject.SetActive(true);
        horseWinnerText.text = $"{winningHorse} has won!";
        if (winningHorse == chosen_horse_)
        {
            int coinsGained = 2 * betAmount;
            playerData.coins += coinsGained;
            coinsLostOrGainedText.text = $"You gained {coinsGained} coins!";
        }
        else
        {
            coinsLostOrGainedText.text = $"You lost {betAmount} coins!";
        }

        // reset horse positions
        for (int i = 0; i < horseObjects.Length; i++)
        {
            horseObjects[i].GetComponent<RectTransform>().anchoredPosition = horseStartPositions[i];
        }
    }

    public void ExitButton()
    {
        GameScreen.gameObject.SetActive(false);
        GameOverScreen.gameObject.SetActive(false);
        horseCrossedFinishLine = false;
    }
}

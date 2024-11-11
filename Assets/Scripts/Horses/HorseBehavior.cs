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

    // Grab playerMomvent
    [SerializeField] private Character_Movement playerMovement;
    //Modal back
    [SerializeField] private GameObject modalBack;

    // Grab the flags gameObjects
    [SerializeField] private GameObject[] flags;

    [SerializeField] private TMP_Text horseInLead;

    public float[] slowSpeeds =
    {
        20f, 22.5f, 25f, 27.5f, 30f, 32.5f, 35f, 37.5f, 40f
    };

    public float[] mediumSpeeds =
    {
        55f, 60f, 65f, 70f, 72.5f, 75f, 77.5f, 80f
    };

    public float[] fastSpeeds =
    {
        115f, 120f, 125f, 130f, 135f, 140f, 145f, 150f
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
            int test = Random.Range(15, 110);

            neurofluxLevel = playerData.neuroflux_meter;

            // If test > neurofluxLevel, then we will return a random speed
            if (test > neurofluxLevel)
            {
                // Return something from small or medium with a 75% chance of small
                int random = Random.Range(0, 4);
                if (random == 0)
                {
                    // Debug.Log("Speed Boost MED!");
                    return mediumSpeeds[Random.Range(0, mediumSpeeds.Length)];
                } else
                {
                    // Debug.Log("Speed Boost SLOW!");
                    return slowSpeeds[Random.Range(0, slowSpeeds.Length)];
                }
            }
            else
            {
                // If the player is on neuroflux, they have a 50% chance of getting a speed boost
                // If they do get a speed boost, we will return a random speed
                // fastspeed has a 33% chance of being chosen
                int rand_2 = Random.Range(0, 3);
                if (rand_2 == 0)
                {
                    return fastSpeeds[Random.Range(0, fastSpeeds.Length)];
                }
                else
                {
                    // Return something from small or medium with a 25% chance of small
                    int random = Random.Range(0, 4);
                    if (random == 0)
                    {
                        return slowSpeeds[Random.Range(0, slowSpeeds.Length)];
                    }
                    else
                    {
                        return mediumSpeeds[Random.Range(0, mediumSpeeds.Length)];
                    }
                }
            }
        }
        else {
            // Make it really rigged against the player. Over 80% chance of being slow
            int random = Random.Range(0, 10);

            if (random < 8)
            {
                return slowSpeeds[Random.Range(0, slowSpeeds.Length)];
            }
            else if (random < 9)
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
        int random = Random.Range(0, 10);

        if (random < 5)
        {
            return slowSpeeds[Random.Range(0, slowSpeeds.Length)];
        }
        else if (random >= 5 && random < 9)
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

        // change chosen horse
        if (chosen_horse_ != playerData.chosen_horse)
        {
            chosen_horse_ = playerData.chosen_horse;
        }

        // change bet amount
        if (betAmount != playerData.bet_amount)
        {
            betAmount = playerData.bet_amount;
        }
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
                        winningHorse = "blackhoof";
                        break;
                    case 1:
                        winningHorse = "chromeblitz";
                        break;
                    case 2:
                        winningHorse = "robotrotter";
                        break;
                    case 3:
                        winningHorse = "nanomane";
                        break;
                    case 4:
                        winningHorse = "thunderbyte";
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
                horseInLead.text = "Chrome Blitz is in the lead!";
                break;
            case 2:
                horseInLead.text = "Robotrotter is in the lead!";
                break;
            case 3:
                horseInLead.text = "Nano Mane is in the lead!";
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

    public bool lostGame = false;

    public void EndGame(string winningHorse)
    {
        GameScreen.gameObject.SetActive(false);
        GameOverScreen.gameObject.SetActive(true);
        horseWinnerText.text = $"{winningHorse} has won!";
        if (winningHorse == chosen_horse_)
        {
            int coinsGained = 2 * betAmount;
            lostGame = false;
            playerData.coins += coinsGained;
            coinsLostOrGainedText.text = $"You gained {coinsGained} coins!";//achid=13,14

            // add coinsGained to playerData.casino_wins
            playerData.casino_winnings += coinsGained;

            if (playerData.casino_winnings >= 500 && !playerData.unlocked_achievements.Contains(13))
            {
                // unlock achievement 13
                playerData.UnlockAchievement(13);
            } else if (playerData.casino_winnings >= 1000 && !playerData.unlocked_achievements.Contains(14))
            {
                // unlock achievement 14
                playerData.UnlockAchievement(14);
            }
            
        }
        else
        {
            lostGame = true;
            coinsLostOrGainedText.text = $"You lost {betAmount} coins!";//achid=12

            // add betAmount to playerData.casino_losses
            playerData.casino_losses += betAmount;

            if (playerData.casino_losses >= 100 && !playerData.unlocked_achievements.Contains(12))
            {
                // unlock achievement 12
                playerData.UnlockAchievement(12);
            }
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

        // if the player has won, subtract only 15 from neuroflux
        if (!lostGame)
        {
            if (playerData.neuroflux_meter < 15)
            {
                playerData.neuroflux_meter = 0;
            }
            else
            {
                playerData.neuroflux_meter -= 15;
            }

        }
        else
        {
            // if the player has lost, subtract 25 from neuroflux
            if (playerData.neuroflux_meter < 25)
            {
                playerData.neuroflux_meter = 0;
            }
            else
            {
                playerData.neuroflux_meter -= 25;
            }
        }

        // Unstop   
        playerMovement.UnstopPlayer();
        modalBack.SetActive(false);
    }
}

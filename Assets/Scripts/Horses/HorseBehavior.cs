using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseBehavior : MonoBehaviour
{
    // Grab all HorseObjects (they're Unity UI Images)
    [SerializeField] private GameObject[] horseObjects;

    // Grab the PlayerData
    private PlayerData playerData => PlayerData.Instance;

    // Grab the flags gameObjects
    [SerializeField] private GameObject[] flags;

    // Have a set of speeds for the horses (first, slow speeds)
    public float[] slowSpeeds =
    {
        20f, 22.5f, 25f, 27.5f, 30f, 32.5f, 35f, 37.5f, 40f, 42.5f, 45f, 47.5f, 50f
    };

    // Have a set of speeds for the horses (second, medium speeds)
    public float[] mediumSpeeds =
    {
        55f, 60f, 65f, 70f, 75f, 80f, 85f, 90f, 95f, 100f, 105f, 110f, 115f
    };

    // Have a set of speeds for the horses (third, fast speeds)
    public float[] fastSpeeds =
    {
        120f, 130f, 140f, 150f, 160f, 170f, 180f, 190f, 200f, 210f, 220f, 230f, 240f
    };

    // Reach Finish Line function (stop the horses)
    // The Flag GameObject has it's own position
    // public void ReachFinishLine()
    // {
    //     // Stop all horses
    //     // change this to just stop the horses transform.position in the array
    //     foreach (GameObject horse in horseObjects)
    //     {
    //         horse.GetComponent<HorseBehavior>().enabled = false;
    //     }

    //     // Stop all flags
    //     foreach (GameObject flag in flags)
    //     {
    //         flag.GetComponent<FlagBehavior>().enabled = false;
    //     }
    // }

    // Grab the players neuroflux level. It goes up to 100 (think of percent, but its an integer)
    private int neurofluxLevel;

    // Grab the player's chosen horse
    private string chosen_horse_; // if it's "blackhoof", "chromeblitz", "robotrotter", "nanomane", "thunderbyte"
    // blackhoof is the 0th index, chromeblitz is the 1st index, etc.
    

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

    // Start
    void Start()
    {
        neurofluxLevel = playerData.neuroflux_meter;
        chosen_horse_ = playerData.chosen_horse;
        // Set the speed of the horse
        // Remember, the horse is a Unity UI Image
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

    // Update to check if the horse has reached the finish line
    void Update()
    {
        // Check if the horse has reached the finish line
        // Go through all horses and check if their position matches the flag's position
        // then stop their movement

        // Check if the horse has reached the finish line
        for (int i = 0; i < horseObjects.Length; i++)
        {
            if (horseObjects[i].transform.position.x >= flags[i].transform.position.x)
            {
                // Stop the horse
                // Change the transofrm position +=
                horseObjects[i].GetComponent<HorseBehavior>().enabled = false;
            }
        }
    }

    // Start is called before the first frame update

    // private System.Random rand = new System.Random();

    // public float[] speeds = {55f, 66f, 77f, 88f, 99f, 110f, 121f, 132f, 143f, 154f, 165f, 176f, 187f, 198f};

    // // 

    // public float neurofluxLevel = 0;

    // public Vector3 direction = Vector3.right;

    // // Start is called before the first frame update
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     transform.position += direction.normalized * speeds[rand.Next(speeds.Length)] * (1 + neurofluxLevel * rand.Next(2)) * Time.deltaTime;
    // }
}

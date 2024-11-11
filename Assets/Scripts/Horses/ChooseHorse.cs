// public string chosen_horse = ""; // can be "blackhoof", "chromeblitz", "robotrotter", "nanomane", "thunderbyte"
// on PlayerData

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChosenHorse : MonoBehaviour
{
    public string chosen_horse = ""; // can be "blackhoof", "chromeblitz", "robotrotter", "nanomane", "thunderbyte"
    [SerializeField] private TMPro.TMP_Text chosenHorseText;

    // HorseSelection Screen serialize
    [SerializeField] private GameObject horseSelectionScreen;
    // HorseGame Screen serialize
    [SerializeField] private GameObject horseGameScreen;
    // ModalBackground serialize
    [SerializeField] private GameObject modalBackground;

    // Button serialize
    [SerializeField] private UnityEngine.UI.Button bet10Button;
    [SerializeField] private UnityEngine.UI.Button bet50Button;
    [SerializeField] private UnityEngine.UI.Button bet100Button;

    // PlayerData
    private PlayerData playerData => PlayerData.Instance;

    // SetHorse
    public void SetBlackHoof()
    {
        chosen_horse = "blackhoof";
        playerData.chosen_horse = chosen_horse;
        chosenHorseText.text = "Selected: Black Hoof";
    }

    public void SetChromeBlitz()
    {
        chosen_horse = "chromeblitz";
        playerData.chosen_horse = chosen_horse;
        chosenHorseText.text = "Selected: Chrome Blitz";
    }

    public void SetRoboTrotter()
    {
        chosen_horse = "robotrotter";
        playerData.chosen_horse = chosen_horse;
        chosenHorseText.text = "Selected: RoboTrotter";
    }

    public void SetNanoMane()
    {
        chosen_horse = "nanomane";
        playerData.chosen_horse = chosen_horse;
        chosenHorseText.text = "Selected: Nano Mane";
    }

    public void SetThunderByte()
    {
        chosen_horse = "thunderbyte";
        playerData.chosen_horse = chosen_horse;
        chosenHorseText.text = "Selected: ThunderByte";
    }

    public void Bet10()
    {
        playerData.bet_amount = 10;
        // Go set modalBack active, turn off horseSelectionScreen
        // and go to horseGameScreen
        modalBackground.SetActive(true);
        horseSelectionScreen.SetActive(false);
        horseGameScreen.SetActive(true);

        // Set the player's coins to be 10 less
        playerData.coins -= 10;
    }

    public void Bet50()
    {
        playerData.bet_amount = 50;
        // Go set modalBack active, turn off horseSelectionScreen
        // and go to horseGameScreen
        modalBackground.SetActive(true);
        horseSelectionScreen.SetActive(false);
        horseGameScreen.SetActive(true);

        playerData.coins -= 50;
    }

    public void Bet100()
    {
        playerData.bet_amount = 100;
        // Go set modalBack active, turn off horseSelectionScreen
        // and go to horseGameScreen
        modalBackground.SetActive(true);
        horseSelectionScreen.SetActive(false);
        horseGameScreen.SetActive(true);

        playerData.coins -= 100;
    }

    // Awake
    private void Awake()
    {
        chosenHorseText.text = "Selected: None";
    }

    // On Update, check if the player has chosen a horse or has enough to bet
    // then turn the bet buttons to interactable=false
    private void Update()
    {
        if (playerData.chosen_horse == "")
        {
            bet10Button.interactable = false;
            bet50Button.interactable = false;
            bet100Button.interactable = false;
        } else {
            bet10Button.interactable = true;
            bet50Button.interactable = true;
            bet100Button.interactable = true;
        }

        if (playerData.coins < 10)
        {
            bet10Button.interactable = false;
            bet50Button.interactable = false;
            bet100Button.interactable = false;
        } else if (playerData.coins < 50)
        {
            bet50Button.interactable = false;
            bet100Button.interactable = false;
        } else if (playerData.coins < 100)
        {
            bet100Button.interactable = false;
        }
    }
}
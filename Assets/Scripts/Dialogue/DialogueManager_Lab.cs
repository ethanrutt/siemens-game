using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the DialogueManager for the Lab
public class DialogueManager_Lab : MonoBehaviour
{
    // PlayerData
    private PlayerData playerData => PlayerData.Instance;
    // We are going to set to "shopopen" once the player talks with the shop guy
    // Keep in mind if the npc_interactions with shopkeeper is 0, the shopkeeper
    // will have a short explanation before he opens the shop

    // Variables
    private bool isTyping = true;
    private float typingSpeed = 0.025f;

    // Text
    public TMPro.TextMeshProUGUI dialogueText;
    // Charname
    public TMPro.TextMeshProUGUI charName;
    // image for the character
    public UnityEngine.UI.Image characterImage;
    // TTC text
    public TMPro.TextMeshProUGUI TTC_Text;

    // import Camera_Movement script
    [SerializeField] private CameraFollow cameraFollow;
    // import PlayerMovement script
    [SerializeField] private Character_Movement playerMovement;

    // Modal back
    [SerializeField] public GameObject backModal;

    // Load in the dialoguePanel
    // Look for UI-Panel Dialogue-Panel and assign it
    public GameObject dialoguePanel;

    // Text for the dialoguePanel
    // Text for the name of the NPC

    // public int dialogueindex
    private int dialogueIndex = 0;

    // Card view panel
    [SerializeField] private GameObject cardViewPanel;
    // Add all the casino owner sprites
    [SerializeField] private Sprite[] deckmasterSprites; // 0->serious, 1-> serious with hands, 2->charismatic, 3->happy with hands

    // Add all the casino owner dialogues
    private string[] deckMasterInitial = {
        "",
    };

    // Integer list of inital sprites
    private int[] deckMasterInitialSprites = {
        2, 0, 1, 2, 3, 2
    };


    // Make casino owner speak
    // public void CasinoOwnerSpeak()
    // {
    //     // Stop player
    //     playerMovement.StopPlayer();

    //     // Turn on the dialogue panel
    //     dialoguePanel.SetActive(true);

    //     // First, say the first dialogue, second dialogue, third dialogue, pan(0), fourth dialogue, pan(1), fifth dialogue, pan(2), sixth dialogue
    //     // If the casino owner has never been interacted with, do the initial coroutine
    //     // otherwise just the normal coroutine
    //     if (playerData.npc_interactions["casino_owner"] == 0)
    //     {
    //         StartCoroutine(CasinoOwnerSpeakInitial());
    //     }
    //     else
    //     {
    //         StartCoroutine(CasinoOwnerSpeakCoroutine());
    //     }

    //     // Increment the npc_interactions for casino_owner
    //     playerData.npc_interactions["casino_owner"] += 1;
    // }

    // private IEnumerator CasinoOwnerSpeakInitial()
    // {
    //     // Change TTC_Text to "Do Not Tap."
    //     TTC_Text.text = "Do Not Tap...";

    //     for (int i = 0; i < casinoOwnerInitial.Length; i++)
    //     {
    //         // If the coroutine is not null, stop the coroutine
    //         if (typeSentenceCoroutine != null)
    //         {
    //             StopCoroutine(typeSentenceCoroutine);
    //         }

    //         // Set the dialogueText to an empty string
    //         dialogueText.text = "";

    //         // Set the characterImage to the appropriate sprite
    //         characterImage.sprite = casinoOwnerSprites[casinoOwnerInitialSprites[i]];

    //         // Set the charName to "Casino Owner"
    //         charName.text = "Casino Owner";

    //         // Start typing the sentence
    //         isTyping = true;
    //         typeSentenceCoroutine = StartCoroutine(TypeSentence(casinoOwnerInitial[i]));

    //         // Wait
    //         yield return new WaitForSeconds(casinoOwnerInitial[i].Length * typingSpeed + 1.25f);

    //         // Wait
    //         yield return new WaitForSeconds(2);
    //     }

    //     // Change TTC_Text to "Tap to Continue."
    //     TTC_Text.text = "Tap to Continue...";
        
    //     // Close the dialogue panel
    //     dialoguePanel.SetActive(false);

    //     // Let player move
    //     playerMovement.UnstopPlayer();
    // }
    
    // private IEnumerator CasinoOwnerSpeakCoroutine()
    // {
    //     // Basically, we're going to go to playerData casino_winnings (int) and casino_losses (int) and get the ratio for that.
    //     // This only applies if we have interacted with the casino owner more than once.
    //     float ratio = (float)playerData.casino_winnings / (float)playerData.casino_losses;

    //     // Change TTC_Text to "Do Not Tap."
    //     TTC_Text.text = "Do Not Tap...";

    //     string[] selectedDialogues;
    //     int[] selectedSprites;

    //     // Determine which dialogues and sprites to use based on the ratio
    //     if (ratio < 0.95)
    //     {
    //         selectedDialogues = casinoOwnerOneLinersLosing;
    //         selectedSprites = casinoOwnerOneLinersLosingSprites;
    //     }
    //     else if (ratio >= 0.95 && ratio <= 1)
    //     {
    //         selectedDialogues = casinoOwnerOneLinersNeutral;
    //         selectedSprites = casinoOwnerOneLinersNeutralSprites;
    //     }
    //     else
    //     {
    //         selectedDialogues = casinoOwnerOneLinersWinning;
    //         selectedSprites = casinoOwnerOneLinersWinningSprites;
    //     }

    //     // If winnings and losings are both 0, then just do ratio = 1
    //     if (playerData.casino_winnings == 0 && playerData.casino_losses == 0)
    //     {
    //         selectedDialogues = casinoOwnerOneLinersNeutral;
    //         selectedSprites = casinoOwnerOneLinersNeutralSprites;
    //     }

    //     // Execute only one oneliner
    //     int i = Random.Range(0, selectedDialogues.Length);

    //     // If the coroutine is not null, stop the coroutine
    //     if (typeSentenceCoroutine != null)
    //     {
    //         StopCoroutine(typeSentenceCoroutine);
    //     }

    //     // Set the dialogueText to an empty string
    //     dialogueText.text = "";

    //     // Set the characterImage to the appropriate sprite
    //     characterImage.sprite = casinoOwnerSprites[selectedSprites[i]];

    //     // Set the charName to "Casino Owner"
    //     charName.text = "Casino Owner";

    //     // Start typing the sentence
    //     isTyping = true;
    //     typeSentenceCoroutine = StartCoroutine(TypeSentence(selectedDialogues[i]));

    //     // Wait
    //     yield return new WaitForSeconds(selectedDialogues[i].Length * typingSpeed + 1.25f);

    //     // Wait
    //     yield return new WaitForSeconds(2);

    //     // Show the choice panel
    //     casinoOwnerChoicePanel.SetActive(true);

    //     // modal goes up, and also dialogue is out
    //     dialoguePanel.SetActive(false);
    //     backModal.SetActive(true);
    // }
    


    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            if (!isTyping)
            {
                dialogueText.text = sentence;
                break;
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
    }

    // Coroutine for typing the sentence
    private Coroutine typeSentenceCoroutine;
        

    // Public integer array for each drunkGuySprites[n] to correspond to drunkGuyDialogues[n]

    // We're going to have a set of dialogues on first interact with the shop owner
    // Start is called before the first frame update
    void Start()
    {
        // Debug to test how sensei interactions are stored
        // Debug.Log("sensei interactions: " + playerData.npc_interactions["sensei"]);

        // Assign the dialoguePanel
        if (dialoguePanel == null)
        {
            dialoguePanel = GameObject.Find("Dialogue-Panel");
        }
        if (dialogueText == null)
        {
            dialogueText = GameObject.Find("Current-Text").GetComponent<TMPro.TextMeshProUGUI>();
        }
        if (charName == null)
        {
            charName = GameObject.Find("Char-Name").GetComponent<TMPro.TextMeshProUGUI>();
        }
        if (characterImage == null)
        {
            characterImage = GameObject.Find("Character-Image").GetComponent<UnityEngine.UI.Image>();
        }

        // if casinoenter=0, start the sensei tutorial
        if (playerData.npc_interactions["labenter"] == 0)
        {
            // Sensei Tutorial
            SenseiTutorial();
        }
    }
    
    // GameObjects to pan the Camera to
    [SerializeField] private GameObject cameraPanTarget; // The area where the casino owner is.

    // Sensei Town Square tutorial
    public void SenseiTutorial()
    {
        // Stop player
        playerMovement.StopPlayer();

        // Turn on the dialogue panel
        dialoguePanel.SetActive(true);

        // First, say the first dialogue, second dialogue, third dialogue, pan(0), fourth dialogue, pan(1), fifth dialogue, pan(2), sixth dialogue
        StartCoroutine(SenseiTutorialCoroutine());

        // Increment the npc_interactions for casinoenter
        playerData.npc_interactions["casinoenter"] = 1;
    }

    private IEnumerator SenseiTutorialCoroutine()
    {
        // Change TTC_Text to "Do Not Tap."
        TTC_Text.text = "Do Not Tap...";

        for (int i = 0; i < senseiDialogues.Length; i++)
        {
            // If the coroutine is not null, stop the coroutine
            if (typeSentenceCoroutine != null)
            {
                StopCoroutine(typeSentenceCoroutine);
            }

            // Set the dialogueText to an empty string
            dialogueText.text = "";

            // Set the characterImage to the appropriate sprite
            characterImage.sprite = senseiSprites[senseiSpriteIndices[i]];

            // Set the charName to "Sensei"
            charName.text = "Sensei";

            // Start typing the sentence
            isTyping = true;
            typeSentenceCoroutine = StartCoroutine(TypeSentence(senseiDialogues[i]));

            // Wait
            yield return new WaitForSeconds(senseiDialogues[i].Length * typingSpeed + 1.25f);

            // if i = 2, pan to the casino owner
            if (i == 1)
            {
                cameraFollow.PanCamera(cameraPanTarget.transform);
            }

            // Wait
            yield return new WaitForSeconds(2);
        }

        // Change TTC_Text to "Tap to Continue."
        TTC_Text.text = "Tap to Continue...";
        
        // Close the dialogue panel
        dialoguePanel.SetActive(false);

        // Let player move
        playerMovement.UnstopPlayer();
    }

    // Sensei sprites
    [SerializeField] private Sprite[] senseiSprites; // series no hands, serious with hands, happy with hands, quite happy with hands
    // 0=serious no hands, 1=serious with hands, 2=happy with hands, 3=quite happy with hands

    // Multiple dialogues for the sensei
    private string[] senseiDialogues = {
        "Right... Welcome to the casino. You want money? Possibly? Well, you're in the right place.",
        "There's really only one thing to do here. Bet on robot horses. Yes, it sounds ridiculous, but it's the only way to make money. The table is over there.",
        "The casino owner offers a 1:1 payout on bets. You basically get twice the amount you bet. Sick, right? Well... There is a catch.",
        "There's something in this town called Neuroflux. You've probably already noticed the meter at the top of your screen. You can only see this meter in the casino.",
        "And when you're more \"fluxed up\", you have a higher chance of winning. But hey, flux isn't cheap, so you have to pay to play. Go ask the casino owner for more details.",
        "Winning big sounds fun, though. But it's much harder than it seems. Good luck, and remember, the house always wins."
    };

    private int[] senseiSpriteIndices = {
        2, 1, 0, 1, 1, 3
    };

    void Update()
    {

        // If player is talking to sensei, no matter if they click or tap, Do NOT close the dialogue
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && playerData.interactable == "sensei")
        {
            // Do nothing
            return;
        }


        // If player is talking to drunkard, one-liner dialogue
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && playerData.interactable == "drunkard")
        {
            dialoguePanel.SetActive(false);
        }


    }
}

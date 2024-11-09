using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the DialogueManager for the TownSquare
// It will store the dialogues for the two NPCs, the Shopowner
// and the Drunk guy on the building
public class DialogueManager_Casino : MonoBehaviour
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

    // Load in the dialoguePanel
    // Look for UI-Panel Dialogue-Panel and assign it
    public GameObject dialoguePanel;

    // Text for the dialoguePanel
    // Text for the name of the NPC

    // public int dialogueindex
    private int dialogueIndex = 0;

    // Casino owner Choice Panel
    public GameObject casinoOwnerChoicePanel;

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
        if (playerData.npc_interactions["casinoenter"] == 0)
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

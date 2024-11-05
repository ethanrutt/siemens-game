using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the DialogueManager for the TownSquare
// It will store the dialogues for the two NPCs, the Shopowner
// and the Drunk guy on the building
public class DialogueManager_TS : MonoBehaviour
{
    // Load the sprites in an array for the two NPCs
    [SerializeField] private Sprite[] shopOwnerSprites;
    [SerializeField] private Sprite[] drunkGuySprites; // serious, serious with hand, happy with hand

    // Variables
    private bool isTyping = true;
    private float typingSpeed = 0.025f;

    // Text
    public TMPro.TextMeshProUGUI dialogueText;
    // Charname
    public TMPro.TextMeshProUGUI charName;
    // image for the character
    public UnityEngine.UI.Image characterImage;

    // Load in the dialoguePanel
    // Look for UI-Panel Dialogue-Panel and assign it
    public GameObject dialoguePanel;

    // Text for the dialoguePanel
    // Text for the name of the NPC


    // We're going to have a set of ONLY random dialogues for the drunk guy
    public string[] drunkGuyDialogues = {
        "You know, the view up here, it's so nice.",
        "I know that I'm not supposed to be up here, but I just can't help it.",
        "I'm not drunk, I'm just... happy.",
        "I'm not going to jump, I'm just going to enjoy the view.",
        "Maybe if life was a little bit more like this view, I'd be happier.",
        "You like that tree? It has no leaves. It's like me, I have no leaves.",
        "That guy at the casino, he gave me some Neuroflux. I lost all my money on that horse game. You know that game?",
        "Meh. Life is like a box of chocolates, without the chocolates inside. Just the box. I guess that's life.",
        "You think I'm here to give you wisdom? You're right. I am the wise one. Call me the wise one. Or don't. But if you do, maybe I'll give you some wisdom.",
        "You know, you really should try the Neuroflux. It's like a party in your head. But don't do it. It's bad for you. But it's so good. But don't do it.",
        "I get happy thinking about how my ex-wife got all my money. I don't have to worry about it anymore. I'm free. I'm free.",
        "Maybe there's more to life than just being happy. Maybe there's more to life than just being happy. Maybe there's – you see how much of a deep thinker I am? That's how most people sound to me.",
        "Most people are fools. Me? I'm just a drunk guy. Well, the youngins here say 'fluxed up'. I guess I'm fluxed up. Doesn't that sound cool? Fluxed up. Try saying that five times fast.",
        "Happiness, sadness, angryness, wait... Angryness? Is that a word? I don't know. I'm just a drunk guy. I'm not a dictionary. I'm not a dictionary.",
        "You know, my name used to be Philip back in the day. But now, I'm just... Robot 0824. That's not half bad. Doesn't sound like Philip though. I like Philip. I like Robot 0824. I like both. I can't choose.",
        "Do you like me? Is what I said to my ex-wife when I first met her. Turns out she didn't like me all that much. That's why she's my ex-wife. Robots can't even procreate, so I don't know why I'm telling you this.",
        "Life is like a tree that stops growing after you cut it down. Wait, that's not right. Well, some people just die. I got put inside a robot and watched my ex-wife take every little penny... Okay, that's it.",
        "Elementary school was nice. I remember when I was in elementary school. Now I'm not in elementary school. I'm on a building. I'm on a building.",
    };

    public int[] drunkGuySpriteIndices = {
        2, 2, 2, 0, 0, 1, 2, 1, 1, 1, 2, 0, 0, 0, 1, 2, 2, 0
    };
    // A coroutine to type out the sentence
    private Coroutine typeSentenceCoroutine;

    public void TalkToDrunkGuy()
    {
        // If the coroutine is not null, stop the coroutine
        if (typeSentenceCoroutine != null)
        {
            StopCoroutine(typeSentenceCoroutine);
        }

        // Randomly select a dialogue from the drunkGuyDialogues array
        int randomIndex = Random.Range(0, drunkGuyDialogues.Length);
        string randomDialogue = drunkGuyDialogues[randomIndex];

        // Set the characterImage to the drunkGuySprites[randomIndex]
        characterImage.sprite = drunkGuySprites[drunkGuySpriteIndices[randomIndex]];

        // Set the charName to "Drunk Guy"
        charName.text = "Drunkard";

        // Set the dialoguePanel to active
        dialoguePanel.SetActive(true);

        // Make the dialoguePanel type something
        isTyping = true;
        typeSentenceCoroutine = StartCoroutine(TypeSentence(randomDialogue));
    }

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

    // Public integer array for each drunkGuySprites[n] to correspond to drunkGuyDialogues[n]

    // We're going to have a set of dialogues on first interact with the shop owner
    // Start is called before the first frame update
    void Start()
    {
        // Assign disaloguePanel
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
        
    }

    // Update is called once per frame
    void Update()
    {
        // If the player touches the screen, we will close the dialoguePanel
        // if mousedown
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            dialoguePanel.SetActive(false);
        }
        
    }
}

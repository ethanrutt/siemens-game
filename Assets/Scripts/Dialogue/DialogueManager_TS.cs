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

    // Load in the dialoguePanel
    // Look for UI-Panel Dialogue-Panel and assign it
    public GameObject dialoguePanel;

    // Text for the dialoguePanel
    // Text for the name of the NPC

    // public int dialogueindex
    private int dialogueIndex = 0;

    // Shop Panel
    public GameObject shopPanel;

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

    // Initial dialogues for the shopkeeper
    public string[] shopOwnerInitial = {
        "Hello there. I suppose we haven't met before, huh. I'm the shopkeeper of this town.",
        "You can call me Gus. But my real name? That's Ethan. So call me Ethan.",
        "Sorry. I suppose I had a bit too much flux. It's alright though. Let me give you a little rundown.",
        "Here at the Scavenger Shop, we sell all sorts of items. Well, not we, but me. I'm alone, as you can tell.",
        "I can sell you some nice cosmetic items. Too cold? Buy a hoodie. Too hot? Nice safety vests. Need some shoes? I got some Jordans.",
        "And I also happen to sell the flash drives you need to unlock some cool dances. Want to impress a girl you know? Buy a dance.",
        "You get the whole idea. I'm here for you. But this stuff isn't free. You need coins. And I need coins. So let's make a deal.",
        "So, from here on out, you can come to me whenever you need something. I'll be here. I'm always here. I mean, my foot's chained to the dirt. And I got just enough flux to keep me going.",
    };

    //0=serious,nohands, 1=series,hand, 2=happy,hand, 3,quitehappy,hand
    public int[] shopOwnerSpriteIndices_InitialSpeak = {
        2, 3, 0, 1, 2, 3, 1, 1
    };

    public int[] drunkGuySpriteIndices = {
        2, 2, 2, 0, 0, 1, 2, 1, 1, 1, 2, 0, 0, 0, 1, 2, 2, 0
    };

    // Now random  dialogues for the shopKeeper
    public string[] shopOwnerOneLiners = {
        "It's cold out here. I wish I had a hoodie. I bet you wish you had a hoodie too. Why don't you buy one?",
        "I kind of miss the past life. It sucks being chained down. But hey, I got my flux. I'm good. You want something?",
        "Abra-cadabra. Might not be a magician, but I can make your coins disappear. Just kidding. I'm not a magician. I'm a shopkeeper. What can I get you?",
        "Do you want to talk, or do you just want my money? Just kidding, I'm here to take yours. What can I get you?",
        "You know, they recently have been minting some new coins. You got some? I got some. But I want more. What can I get you?",
        "Few decades ago they said AI would take over shopping. AI has nothing on me. Unless... I'm an AI. Jeepers. What can I get you?",
        "Need something? I need something. My life back. But I bet you need a new hoodie, or some nice shoes, huh. First world problems. But I got you. What can I get you?",
        "I'm only snappy on alternating Tuesdays. The other days, well, I'm just your friendly neighborhood shopkeeper. What can I get you?",
        "Psst. Hey. Can you hear me? Well, I hear the coins jiggling in your pocket. Or robot-thingamajig. Man, what did they do to us? Anyways, what can I get you?",
        "You like my monologues? I like my monologues. I also like money. And I bet you have some. What can I get you?",
        "I got some new stuff. Just kidding. Just the same old stuff. But you know what they say. If it ain't broke, don't fix it. What can I get you?",
        "Man the only thing I miss about being human is not being chained to this damn ground. But hey, I got my flux. What can I get you?",
        "You know being chained sucks, but a guy recently dropped all his coins right next to me. I didn't say a word. Don't tell him though. Or... I guess the secret's out. Need something?",
        "Pacha-pacha-pacha-pacha-chocha-chocha-hey! Sorry. I'm just trying to keep myself entertained. What can I get you?",
        "Maybe we would get along if you gave me some money. I'll give you what you're here for. What can I get you?",
        "They say revenge is a dish best served cold. I say it's a dish best served with a side of coins. What can I get you?",
        "You know, I didn't know you liked me so much. I'm flattered. Here's the problem. We don't give discounts out here. If we did I'd be broke. What can I get you?",
        "Money talks, and so do humans. Only difference is, I like money. What can I get you?",
    };

//0=serious,nohands, 1=series,hand, 2=happy,hand, 3,quitehappy,hand
    public int[] shopOwnerSpriteIndices_OneLiners = {
        0, 0, 3, 3, 2, 1, 1, 3, 1, 2, 0, 1, 3, 1, 0, 1, 3, 1
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

    

    public void TalkToShopOwner()
    {
        dialogueIndex = 0;

        // If the coroutine is not null, stop the coroutine
        if (typeSentenceCoroutine != null)
        {
            StopCoroutine(typeSentenceCoroutine);
        }

        // Check if it's the first time talking to the shop owner
        if (playerData.npc_interactions["shopkeeper"] == 0)
        {
            StartCoroutine(InitialShopOwnerDialogue());
        }
        else
        {
            if (typeSentenceCoroutine != null)
            {
                StopCoroutine(typeSentenceCoroutine);
            }

            // Set the characterImage to the appropriate sprite
            characterImage.sprite = shopOwnerSprites[shopOwnerSpriteIndices_OneLiners[dialogueIndex]];

            // Set the charName to "Shop Owner"
            charName.text = "Ethan";

            // Set the dialoguePanel to active
            dialoguePanel.SetActive(true);

            // Start typing the sentence
            isTyping = true;

            typeSentenceCoroutine = StartCoroutine(TypeSentence(shopOwnerOneLiners[dialogueIndex]));
        }
    }

    // Coroutine to handle the initial conversation with the shop owner
    private IEnumerator InitialShopOwnerDialogue()
    {
        // Using dialogue index to type out the initial dialogues
        for (int i = 0; i < shopOwnerInitial.Length; i++)
        {
            // Set the characterImage to the appropriate sprite
            characterImage.sprite = shopOwnerSprites[shopOwnerSpriteIndices_InitialSpeak[i]];

            // Set the charName to "Shop Owner"
            charName.text = "Shop Owner";

            // Set the dialoguePanel to active
            dialoguePanel.SetActive(true);

            // Start typing the sentence
            isTyping = true;
            typeSentenceCoroutine = StartCoroutine(TypeSentence(shopOwnerInitial[i]));

            // the coroutine will be continued on clicking the screen
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.touchCount > 0);
        }

        // Increment the npc_interactions for the shopkeeper

        playerData.npc_interactions["shopkeeper"]++;
    }


    // Get the next sentence
    // public void GetNextSentence()
    // {
    //     // If the coroutine is not null, stop the coroutine
    //     if (typeSentenceCoroutine != null)
    //     {
    //         StopCoroutine(typeSentenceCoroutine);
    //     }

        
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

    // Get the next sentence
    public void GetNextSentence()
    {
        StopAllCoroutines();

        dialogueText.text = "";

        // If the dialogueIndex is less than the length of the shopOwnerOneLiners array
        if (dialogueIndex < shopOwnerOneLiners.Length - 1)
        {
            // Increment the dialogueIndex
            dialogueIndex++;

            // Set the characterImage to the appropriate sprite
            characterImage.sprite = shopOwnerSprites[shopOwnerSpriteIndices_OneLiners[dialogueIndex]];

            // Set the charName to "Shop Owner"
            if (dialogueIndex == 0)
            {
                charName.text = "Shop Owner";
            } else if (dialogueIndex == 1)
            {
                charName.text = "Gus?";
            } else
            {
                charName.text = "Ethan";
            }

            // Start typing the sentence
            isTyping = true;
            typeSentenceCoroutine = StartCoroutine(TypeSentence(shopOwnerOneLiners[dialogueIndex]));
        }
        else
        {
            // Wait for the sentence to end before
            // opening the shopPanel and setting dialogue to false
            dialoguePanel.SetActive(false);

            // Set the shopPanel to active
            // interactable=shopopen
            playerData.interactable = "shopopen";
        }


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

    void Update()
{
    // If player is talking to drunkard, one-liner dialogue
    if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && playerData.interactable == "drunkard")
    {
        dialoguePanel.SetActive(false);
    }

    // If player is talking to shop owner, progress through dialogue
    if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && playerData.interactable == "shopowner")
    {
        if (dialogueIndex == 0 && playerData.npc_interactions["shopkeeper"] == 0)
        {
            // Initial dialogue
            StartCoroutine(InitialShopOwnerDialogue());
        }
        else
        {
            // Progress the shop owner dialogue
            GetNextSentence();
        }
    }
}


}

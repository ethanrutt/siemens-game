using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager2 : MonoBehaviour
{
    //private PlayerData playerData => PlayerData.Instance;
    
    public Camera mainCamera; // Reference to the Main Camera
    private bool isBelowThreshold = false;

    public List<Card> deck  = new List<Card>();
    public List<Card> discardPile  = new List<Card>();
    public List<Card> currentHand  = new List<Card>();
    public List<Card> playerDeck  = new List<Card>();
    public List<int> playerUnlocks  = new List<int>();

    public int[] cardId;   
    public int[] cardCost;
    public int[] cardPower;
    public string[] cardDescription;
    public string[] cardNames;

    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    public Transform[] playSlots;
    public bool[] availablePlaySlots;

    public TextMeshProUGUI deckText; 
    public TextMeshProUGUI discardText;
    public int handSize;

    public Button playTurnButton;
    

    private void Start(){
        playTurnButton.gameObject.SetActive(false);

        //playerUnlocks = playerData.unlocked_cards;


        int j = 0;
        foreach(Card card in deck){
            card.id = cardId[j];
            card.cost = cardCost[j];
            card.power = cardPower[j];
            card.cardName = cardNames[j];
            j++;
        }


        //playerUnlocks.Count
        for(int i = 0; i < 7; i++){
            //Card randCard = deck[playerUnlocks[i]];
            Card randCard = deck[Random.Range(0, deck.Count)];
            playerDeck.Add(randCard);
            
            deck.Remove(randCard);
        }


        for(int i = 0; i < handSize; i++){
            DrawCard(i);
        }

        resizeCardSlots();

    }

    public void Reset(){
        playTurnButton.gameObject.SetActive(false);

        foreach(Card card in discardPile){
            card.hasBeenPlayed = false;
            card.nameLabel.SetActive(false);
        }

        foreach(Card card in currentHand){
            if(card.isInSlot){
                card.isInSlot = false;
                availablePlaySlots[card.playIndex] = true;
            }  
            card.hasBeenPlayed = false;
            availableCardSlots[card.handIndex] = true;
            card.nameLabel.SetActive(false);
            discardPile.Add(card);
            card.gameObject.SetActive(false);
        }

        foreach(Card card in playerDeck){
            card.hasBeenPlayed = false;
            card.nameLabel.SetActive(false);
            discardPile.Add(card);
        }

        currentHand.Clear();
        playerDeck.Clear();

        Shuffle();

        for(int i = 0; i < handSize; i++){
            DrawCard(i);
        }

        resizeCardSlots();

        deckText.text = playerDeck.Count.ToString();
        discardText.text = discardPile.Count.ToString();
    }



    public void Update(){
        CheckPlaySlots();
        if(playerDeck.Count == 0){
            Shuffle();
        }

        resizeCardSlots();

        deckText.text = playerDeck.Count.ToString();
        discardText.text = discardPile.Count.ToString();
    }

    public void CheckPlaySlots(){
        bool fullSlots = true;
        
        foreach(bool available in availablePlaySlots){
            if(available){
                fullSlots = false;
                break;
            }
        }

        playTurnButton.gameObject.SetActive(fullSlots);
    }

    public void DrawCard(int index){
        if(playerDeck.Count >= 1){
            Card randCard = playerDeck[Random.Range(0, playerDeck.Count)];
            if(availableCardSlots[index] == true){
                randCard.gameObject.SetActive(true);
                randCard.handIndex = index;
                randCard.transform.position = cardSlots[randCard.handIndex].localPosition;
                randCard.transform.position += new Vector3(3.23f, -2.24f, 6);

                randCard.hasBeenPlayed = false;
                randCard.isInSlot = false;
                availableCardSlots[index] = false;
                playerDeck.Remove(randCard);
                currentHand.Insert(index, randCard);
            }
        }
    }

    public void resizeCardSlots()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Calculate screen dimensions in world space
        float screenWidthInWorld = mainCamera.orthographicSize * 2.0f * mainCamera.aspect;

        // Check if the state has changed
        if (screenWidthInWorld < 15.5f && !isBelowThreshold)
        {
            // Run the resize logic for below threshold
            for (int i = 0; i < handSize; i++)
            {
                cardSlots[i].localPosition = new Vector3(-4.353f + (i * 1.1272f), cardSlots[i].localPosition.y, cardSlots[i].localPosition.z);
                if(currentHand[i].isInSlot == false){
                    currentHand[i].transform.position = cardSlots[i].localPosition;
                    currentHand[i].transform.position += new Vector3(3.23f, -2.24f, 6);
                }
            }

            isBelowThreshold = true; // Update state
            Debug.Log($"Screen width is below threshold: {screenWidthInWorld}");
        }
        else if (screenWidthInWorld >= 15.5f && isBelowThreshold)
        {
            // Run the resize logic for above threshold
            for (int i = 0; i < handSize; i++)
            {
                cardSlots[i].localPosition = new Vector3(-4.74f + (i * 1.5f), cardSlots[i].localPosition.y, cardSlots[i].localPosition.z);
                if(currentHand[i].isInSlot == false){
                    currentHand[i].transform.position = cardSlots[i].localPosition;
                    currentHand[i].transform.position += new Vector3(3.23f, -2.24f, 6);
                }
            }

            isBelowThreshold = false; // Update state
            Debug.Log($"Screen width is above threshold: {screenWidthInWorld}");
        }
    }

    public void MoveToPlaySlot(Card playCard){
        for(int i = 0; i < 1; i++){
            if(availablePlaySlots[i] == true){
                playCard.transform.position = playSlots[i].position;
                playCard.transform.position += new Vector3(0, 0, 6);
                playCard.isInSlot = true;
                playCard.playIndex = i;
                availablePlaySlots[i] = false;
                break;
            }
        }
    }

    public void UndoPlaySlot(Card playCard){
        playCard.transform.position = cardSlots[playCard.handIndex].localPosition;
        playCard.transform.position += new Vector3(3.23f, -2.24f, 6);
        playCard.isInSlot = false;
        availablePlaySlots[playCard.playIndex] = true;
    }

    public void PlayTurn(){
        for(int i = 0; i < handSize; i++){
            Card card = currentHand[i];

            if(card.isInSlot == true){
                if(playerDeck.Count == 0){
                    Shuffle();
                }
                
                card.hasBeenPlayed = true;
                card.isInSlot = false;
                availableCardSlots[card.handIndex] = true;
                availablePlaySlots[card.playIndex] = true;
                card.nameLabel.SetActive(false);
                discardPile.Add(card);
                currentHand.Remove(card);
                DrawCard(card.handIndex);

                card.gameObject.SetActive(false);

            }
        }
    }

    public void Shuffle(){
        if(discardPile.Count >= 1){
            foreach(Card card in discardPile){
                playerDeck.Add(card);
            }
            discardPile.Clear();
        }
    }
}

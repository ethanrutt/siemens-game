using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<Card> deck  = new List<Card>();
    public List<Card> discardPile  = new List<Card>();
    public int[] cardId;   
    public int[] cardCost;
    public int[] cardPower;
    public string[] cardDescription;

    public Transform[] cardSlots;
    public bool[] availableCardSlots;
    public TextMeshProUGUI deckText; 
    public TextMeshProUGUI discardText;
    public int handSize;



    public void DrawCard(int index){
        if(deck.Count >= 1){
            Card randCard = deck[Random.Range(0, deck.Count)];
            if(availableCardSlots[index] == true){
                randCard.gameObject.SetActive(true);
                randCard.handIndex = index;
                randCard.transform.position = cardSlots[randCard.handIndex].position;
                randCard.transform.position += new Vector3(0, 0, 6);
                randCard.hasBeenPlayed = false;
                availableCardSlots[index] = false;
                deck.Remove(randCard);
            }
        }
    }

    public void Shuffle(){
        if(discardPile.Count >= 1){
            foreach(Card card in discardPile){
                deck.Add(card);
            }
            discardPile.Clear();
        }
    }

    private void Start(){
        int j = 0;
        foreach(Card card in deck){
            card.id = cardId[j];
            card.cost = cardCost[j];
            card.power = cardPower[j];
            j++;
        }

        for(int i = 0; i < handSize; i++){
            DrawCard(i);
        }
    }

    private void Update(){
        if(deck.Count == 0){
            Shuffle();
        }
        deckText.text = deck.Count.ToString();
        discardText.text = discardPile.Count.ToString();
    }
}

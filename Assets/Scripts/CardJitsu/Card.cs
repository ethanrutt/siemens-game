using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private Vector3 originalPosition;
    public bool hasBeenPlayed;
    public int handIndex;

    public int id;
    public string cardName;
    public int cost;
    public int power;

    private GameManager gm;


    private void Start(){
        gm = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown(){
        if(hasBeenPlayed == false){
            transform.position += Vector3.up * 5;
            hasBeenPlayed = true;
            gm.availableCardSlots[handIndex] = true;
            Invoke("AutomatedCardDraw", 2f);
            Invoke("MoveToDiscardPile", 2f);
        }
    }

    private void OnMouseEnter()
    {
        if (!hasBeenPlayed)
        {
            // Move the card up and forward when hovered
            originalPosition = transform.position;
            transform.position += new Vector3(0, 0.05f, -4);
        }
    }

    private void OnMouseExit()
    {
        if (!hasBeenPlayed)
        {
            // Return the card to its original position when no longer hovered
            transform.position = originalPosition;
        }
    }

    void AutomatedCardDraw(){
        //if(gm.deck.Count == 0){
        //    gm.Shuffle();
        //}
        gm.DrawCard(handIndex);
    }

    void MoveToDiscardPile(){
        gm.discardPile.Add(this);
        gameObject.SetActive(false);
    }
}

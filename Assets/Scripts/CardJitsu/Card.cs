using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    private Vector3 originalPosition;
    public bool hasBeenPlayed;
    public bool isInSlot;
    public bool ignoreExit = false;
    public int handIndex;
    public int playIndex;

    public int id;
    public string cardName;
    public int cost;
    public int power;

    private GameManager2 gm;
    public TextMeshProUGUI nameLabel;


    private void Start(){
        gm = FindObjectOfType<GameManager2>();
        nameLabel.text = cardName;      // Set the text of the label to the card name
        nameLabel.enabled = false;
    }

    private void OnMouseDown(){
        if(hasBeenPlayed == false){
            if(isInSlot){
                gm.UndoPlaySlot(this);    
            }
            else{
                gm.MoveToPlaySlot(this);
            }
            ignoreExit = true;
            StartCoroutine(ResetIgnoreMouseExit());
            //transform.position += Vector3.up * 5;
            //hasBeenPlayed = true;
            //gm.availableCardSlots[handIndex] = true;
            nameLabel.enabled = false;
            //Invoke("AutomatedCardDraw", 2f);
            //Invoke("MoveToDiscardPile", 2f);
        }
    }

    private void OnMouseEnter()
    {
        if (!hasBeenPlayed)
        {
            // Move the card up and forward when hovered
            originalPosition = transform.position;
            transform.position += new Vector3(0, 0.05f, -4);
            // Assuming an offset on the x-axis per index, adjust as needed
            Vector3 labelOffset = new Vector3(95f + (handIndex * 65f), 0, 0); // Change offset as needed
            nameLabel.transform.position = labelOffset;
            nameLabel.enabled = true;
        }
        

    }

    private void OnMouseExit()
    {
        if (!hasBeenPlayed && !ignoreExit)
        {
            // Return the card to its original position when no longer hovered
            transform.position = originalPosition;
            nameLabel.enabled = false;
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

    private IEnumerator ResetIgnoreMouseExit()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to allow cursor to re-enter
        ignoreExit = false; // Re-enable OnMouseExit functionality
    }
}

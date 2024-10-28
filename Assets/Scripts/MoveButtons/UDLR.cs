using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Rishi Santhanam
// The Up Down Left Right buttons (using PointerDown) to move the player
// The OnCLick of the button is already on the Movement of the Player
public class UDLR : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject player;
    private bool isMovingUp, isMovingDown, isMovingLeft, isMovingRight;

    private void Update()
    {
        if (isMovingUp)
            player.GetComponent<Character_Movement>().MoveUp();
        if (isMovingDown)
            player.GetComponent<Character_Movement>().MoveDown();
        if (isMovingLeft)
            player.GetComponent<Character_Movement>().MoveLeft();
        if (isMovingRight)
            player.GetComponent<Character_Movement>().MoveRight();
        if (!isMovingUp && !isMovingDown && !isMovingLeft && !isMovingRight)
            player.GetComponent<Character_Movement>().StopMoving();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Start moving based on the button pressed
        switch (eventData.pointerPress.name)
        {
            case "UpButton":
                isMovingUp = true;
                break;
            case "DownButton":
                isMovingDown = true;
                break;
            case "LeftButton":
                isMovingLeft = true;
                break;
            case "RightButton":
                isMovingRight = true;
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Stop movement when the button is released
        isMovingUp = false;
        isMovingDown = false;
        isMovingLeft = false;
        isMovingRight = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This is a constructor to assign a sprite to a dialogue image for the
 * character
 */
[System.Serializable]
public class CharacterImage
{
    public Sprite sprite; // The character sprite
    public int associatedValue; // The associated integer value

    public CharacterImage(Sprite sprite, int associatedValue)
    {
        this.sprite = sprite;
        this.associatedValue = associatedValue;
    }
}

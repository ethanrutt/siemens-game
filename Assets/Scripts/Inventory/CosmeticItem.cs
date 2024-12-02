using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @class CosmeticItem
 * @brief Dataclass for cosmetic items
 */
[System.Serializable]
public class CosmeticItem
{
    public string itemName;
    public Sprite itemIcon;
    public BodyPart bodyPart;
}

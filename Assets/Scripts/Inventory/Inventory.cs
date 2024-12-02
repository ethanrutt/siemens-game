using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/**
 * @class Inventory
 * @brief Simple get and set class for adding and removing items from the
 * inventory
 */
public class Inventory : MonoBehaviour
{
    public List<CosmeticItem> cosmeticItems = new List<CosmeticItem>();

    public void AddItem(CosmeticItem item)
    {
        cosmeticItems.Add(item);
    }

    public void RemoveItem(CosmeticItem item)
    {
        cosmeticItems.Remove(item);
    }
}

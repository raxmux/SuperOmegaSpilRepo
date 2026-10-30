using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    List<Item> items = new List<Item>();
    List<Item> equipment = new List<Item>();
    List<Item> consumables = new List<Item>();


    public void AddItem(Item item)
    {
        // Add the item to the inventory
        Debug.Log($"Added {item.name} to inventory.");
    }
    public void RemoveItem(Item item)
    {
        // Remove the item from the inventory
        Debug.Log($"Removed {item.name} from inventory.");
    }

}

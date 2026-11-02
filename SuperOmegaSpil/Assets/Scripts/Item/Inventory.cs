using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    List<Item> items = new List<Item>();
    List<Item> equipment = new List<Item>();
    List<Item> consumables = new List<Item>();


    public void AddItem(Item item)
    {
        if(item.itemType == itemType.relic)
        {
            if(item.Pickup())
            {
                items.Add(item);
            }
        }
        else if(item.itemType == itemType.Equipment)
        {
            if (item.Pickup())
            {
                equipment.Add(item);
            }
        }
        else if(item.itemType == itemType.Consumable)
        {
            if (item.Pickup())
            {
                consumables.Add(item);
            }
        }

        // Add the item to the inventory
        Debug.Log($"Added {item.name} to inventory : {item.itemType}.");
    }
    public void RemoveItem(Item item)
    {
        if (item.itemType == itemType.relic)
        {
            items.Remove(item);
        }
        else if (item.itemType == itemType.Equipment)
        {
            equipment.Remove(item);
        }
        else if (item.itemType == itemType.Consumable)
        {
            consumables.Remove(item);
        }

        // Remove the item from the inventory
        Debug.Log($"Removed {item.name} from inventory : {item.itemType}.");
    }

}

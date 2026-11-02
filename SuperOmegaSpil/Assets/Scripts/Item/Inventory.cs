using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public List<Item> equipment = new List<Item>();
    public List<Item> consumables = new List<Item>();
    int maxConsumables = 2;

    [Header("")]
    public GameObject chooseEquipmentUI;
    public GameObject chooseConsumableUI;

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

    private bool? pickEquipment; //used to determine if the player wants to keep the new equipment or not, null means they haven't chosen yet
    private int? pickConsumable; //used to determine which consumable the player wants to keep, null means they haven't chosen yet
    public IEnumerator AddItem(Item item_)
    {
        if (item_.itemType == itemType.Equipment)
        {
            if (item_.equipmentType != equipmentType.none)
            {
                //unequip the other equipment
                foreach (Item item in equipment)
                {
                    if (item.equipmentType == item_.equipmentType)
                    {
                        //pick an item to keep
                        chooseEquipmentUI.SetActive(true);

                        pickEquipment = null;

                        // Wait until the player makes a choice
                        yield return new WaitUntil(() => pickEquipment.HasValue);

                        if(pickEquipment == true)
                        {
                            // Keep the new item
                            equipment.Add(item_);
                            equipment.Remove(item);
                        }
                        else
                        {
                            // Remove the new item
                            equipment.Add(item);
                            equipment.Remove(item_);
                        }

                        break;
                    }
                }
                equipment.Add(item_);
            }
        }
        else if (item_.itemType == itemType.Consumable)
        {
            if(consumables.Count < maxConsumables)
            {
                consumables.Add(item_);
            }
            else
            {
                pickConsumable = null;

                // Wait until the player makes a choice
                yield return new WaitUntil(() => pickConsumable.HasValue);

                if (pickConsumable == 0)
                {
                    consumables.RemoveAt(0);
                    consumables.Add(item_);
                }
                else if(pickConsumable == 1)
                {
                    consumables.RemoveAt(1);
                    consumables.Add(item_);
                }
                else if(pickConsumable == 2)
                {
                    consumables.RemoveAt(2);
                    consumables.Add(item_);
                }
            }
        }
        else if (item_.itemType == itemType.relic)
        {
            items.Add(item_);
        }

        //add the trigger to the trigger manager so that it can be called when the event happens
        if (item_.trigger.triggerType != TriggerType.none && item_.trigger.triggerAction != null && item_.trigger.triggerAction.Count > 0)
        {
            TriggerManager.Instance.RegisterListener(item_.trigger.triggerType,
                context =>
                {
                    foreach (TriggerActions action in item_.trigger.triggerAction) 
                    { 
                        if (action != null) action.Execute(context); 
                    }
                });
        }

        if (item_.stats.hasStats)
        {
            //change the stats of the player when the item is picked up


        }

        item_.OnPickup();

        yield return new WaitForEndOfFrame();
    }

    public void PickEquipmentButton(bool keepItem)
    {
        pickEquipment = keepItem;
    }
    public void PickConsumableButton(int index)
    {
        pickConsumable = index;
    }
}

using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Event", menuName = "Event/Shop Event")]
public class ShopEvent : Event
{
    protected override EventType EventType => EventType.Shop;

    [Header("Shop settings")]
    [SerializeField] List<Item> itemsForSale = new List<Item>();

    //[Header("reward settings")]
    //list of rewards to give the player after the event is completed
    

    protected override void StartEvent()
    {
        base.StartEvent();

        EventManager.instance.shopUI.SetActive(true);

    }

    public void BuyItem(int buttonIndex)
    {
        if (Inventory.instance.money >= itemsForSale[buttonIndex].value)
        {
            Inventory.instance.AddMoney(itemsForSale[buttonIndex].value * -1);
            Inventory.instance.AddItem(itemsForSale[buttonIndex]);
        }
    }
}

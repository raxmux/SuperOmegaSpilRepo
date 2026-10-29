using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

public class ShopEvent : Event
{
    protected override EventType EventType => EventType.Shop;

    [Header("Shop settings")]
    List<Item> itemsForSale = new List<Item>();

    [Header("reward settings")]
    //list of rewards to give the player after the event is completed

    [Header("EventUI")]
    [SerializeField] protected GameObject ShopUI;

    protected override void StartEvent()
    {
        base.StartEvent();

        ShopUI.SetActive(true);

    }


}

using Unity.VisualScripting;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Shop Event", menuName = "Event/Shop Event")]
public class ShopEvent : Event
{
    public override EventType EventType => EventType.Shop;

    [Header("Shop settings")]
    [SerializeField] List<Item> itemsForSale = new List<Item>();

    //[Header("reward settings")]
    //list of rewards to give the player after the event is completed
    

    public override void StartEvent()
    {
        base.StartEvent();

        EventManager.instance.shopUI.SetActive(true);
        for (int i = 0; i < EventManager.instance.ShopButtons.Count; i++)
        {
            if (i < itemsForSale.Count)
            {
                EventManager.instance.ShopButtons[i].SetActive(true);

                if (itemsForSale[i].itemSprite)
                {
                    EventManager.instance.ShopButtons[i].transform.GetChild(1).GetComponent<RawImage>().texture = itemsForSale[i].itemSprite.texture;
                }

                EventManager.instance.ShopButtons[i].transform.GetChild(2).GetComponent<TMP_Text>().text = itemsForSale[i].itemName;
                EventManager.instance.ShopButtons[i].transform.GetChild(3).GetComponent<TMP_Text>().text = itemsForSale[i].itemDesc;
            }
            else
            {
                EventManager.instance.ShopButtons[i].SetActive(false);
            }
        }
    }
    public override void EndEvent()
    {
        base.EndEvent();

        EventManager.instance.shopUI.SetActive(false);
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

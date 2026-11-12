using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Shop Event", menuName = "Event/Shop Event")]
public class ShopEvent : Event
{
    public override EventType EventType => EventType.Shop;

    [Header("Shop settings")]
    [SerializeField] List<Item> shopPool = new List<Item>();
    [SerializeField] int itemsPerShop = 3;

    [Header("Rarity chances")]
    [SerializeField] float commonChance = 50f;
    [SerializeField] float uncommonChance = 28f;
    [SerializeField] float rareChance = 14f;
    [SerializeField] float epicChance = 6f;
    [SerializeField] float legendaryChance = 2f;

    [Header("Price")]
    [SerializeField] string priceFormat = "{0}g";
    [SerializeField] string soldText = "SOLD";
    [SerializeField] Color cantAffordColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField, Range(0f, 1f)] float soldOpacity = 0.4f;

    readonly List<Item> stock = new List<Item>();
    readonly List<bool> sold = new List<bool>();

    public override void StartEvent()
    {
        base.StartEvent();

        RollStock();
        EventManager.instance.shopUI.SetActive(true);
        RefreshButtons();
        SelectFirstAvailable();
    }

    public override void EndEvent()
    {
        base.EndEvent();

        EventManager.instance.shopUI.SetActive(false);
    }

    public void BuyItem(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= stock.Count || sold[buttonIndex]) return;

        Item item = stock[buttonIndex];
        if (Inventory.instance.money < item.value) return;

        Inventory.instance.AddMoney(-item.value);
        Inventory.instance.GiveItem(item);
        sold[buttonIndex] = true;

        RefreshButtons();
        SelectFirstAvailable();
    }

    void RollStock()
    {
        stock.Clear();
        sold.Clear();

        for (int i = 0; i < itemsPerShop; i++)
        {
            Item item = RollItem();
            if (item == null) break;

            stock.Add(item);
            sold.Add(false);
        }
    }

    // Picks a rarity by chance (only rarities that exist in the pool), then a random item of that rarity.
    Item RollItem()
    {
        var rarities = (itemRarity[])Enum.GetValues(typeof(itemRarity));

        float total = 0f;
        foreach (itemRarity r in rarities)
            if (PoolHas(r)) total += Chance(r);

        if (total <= 0f) return null;

        float roll = UnityEngine.Random.Range(0f, total);
        itemRarity picked = rarities[0];
        foreach (itemRarity r in rarities)
        {
            if (!PoolHas(r)) continue;
            picked = r;
            if (roll < Chance(r)) break;
            roll -= Chance(r);
        }

        var options = shopPool.FindAll(i => i != null && i.itemRarity == picked);
        return options[UnityEngine.Random.Range(0, options.Count)];
    }

    bool PoolHas(itemRarity rarity)
    {
        return shopPool.Exists(i => i != null && i.itemRarity == rarity);
    }

    float Chance(itemRarity rarity)
    {
        switch (rarity)
        {
            case itemRarity.Common: return commonChance;
            case itemRarity.Uncommon: return uncommonChance;
            case itemRarity.Rare: return rareChance;
            case itemRarity.Epic: return epicChance;
            case itemRarity.Legendary: return legendaryChance;
            default: return 0f;
        }
    }

    void RefreshButtons()
    {
        List<GameObject> buttons = EventManager.instance.ShopButtons;

        for (int i = 0; i < buttons.Count; i++)
        {
            GameObject slot = buttons[i];
            if (i >= stock.Count)
            {
                slot.SetActive(false);
                continue;
            }

            slot.SetActive(true);
            Item item = stock[i];

            SetIcon(slot.transform.GetChild(1).GetComponent<RawImage>(), item.itemSprite);

            string price = PriceText(item, sold[i]);
            Transform priceSlot = slot.transform.Find("Price");
            TMP_Text priceText = priceSlot != null ? priceSlot.GetComponent<TMP_Text>() : null;

            if (priceText != null) priceText.text = price;
            slot.transform.GetChild(2).GetComponent<TMP_Text>().text =
                priceText != null ? item.itemName : $"{item.itemName}  {price}";
            slot.transform.GetChild(3).GetComponent<TMP_Text>().text = item.GetFullDescription();

            // Sold slots fade out but stay selectable, so you can still move past them to Exit.
            CanvasGroup group = slot.GetComponent<CanvasGroup>();
            if (group == null) group = slot.AddComponent<CanvasGroup>();
            group.alpha = sold[i] ? soldOpacity : 1f;

            Button button = slot.GetComponent<Button>();
            if (button != null) button.interactable = true;
        }
    }

    string PriceText(Item item, bool isSold)
    {
        if (isSold) return soldText;

        string price = string.Format(priceFormat, item.value);
        if (Inventory.instance != null && Inventory.instance.money < item.value)
            price = $"<color=#{ColorUtility.ToHtmlStringRGB(cantAffordColor)}>{price}</color>";
        return price;
    }

    // Shows only this sprite, even if it's cut out of a bigger sprite sheet.
    static void SetIcon(RawImage image, Sprite sprite)
    {
        if (image == null) return;

        image.enabled = sprite != null;
        if (sprite == null) return;

        Texture2D tex = sprite.texture;
        Rect r = sprite.textureRect;
        image.texture = tex;
        image.uvRect = new Rect(r.x / tex.width, r.y / tex.height, r.width / tex.width, r.height / tex.height);
    }

    void SelectFirstAvailable()
    {
        if (UIMovement.instance == null) return;

        List<GameObject> buttons = EventManager.instance.ShopButtons;
        for (int i = 0; i < stock.Count && i < buttons.Count; i++)
        {
            if (!sold[i])
            {
                UIMovement.instance.SetSelectedUIElement(buttons[i]);
                return;
            }
        }
    }
}
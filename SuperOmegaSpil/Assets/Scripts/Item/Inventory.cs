using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public int money = 2;
    public List<Item> items = new List<Item>();       // relics
    public List<Item> equipment = new List<Item>();
    public List<Item> consumables = new List<Item>();
    [SerializeField] int maxConsumables = 2;

    [Header("")]
    [SerializeField] Combatant playerCombatant;

    [Header("")]
    public GameObject chooseEquipmentUI;
    public GameObject chooseConsumableUI;
    [Header("")]
    [SerializeField] GameObject moneytxtRoom;
    [SerializeField] GameObject moneytxtWorld;




    readonly List<(Item item, TriggerType type, Action<TriggerContext> listener)> triggerListeners
        = new List<(Item, TriggerType, Action<TriggerContext>)>();

    bool choosing;          // true while a choose screen is open
    bool? pickEquipment;    // null = the player hasn't chosen yet
    int? pickConsumable;    // null = the player hasn't chosen yet

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("FUCK GUYS NOOO theres more than one instance of Inventory!!!");
            return;
        }
        instance = this;
    }

    public event Action<int> MoneyChanged;
    public event Action<Item> ItemAdded;

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
        Debug.Log("Money changed by " + amount + ", new total: " + money);
        MoneyChanged?.Invoke(amount);
    }

    void UpdateMoneyUI()
    {
        moneytxtRoom.GetComponent<TMP_Text>().text = "Money : " + money.ToString();
        moneytxtWorld.GetComponent<TMP_Text>().text = "Money : " + money.ToString();
    }

    // Use this to give the player an item (shop, dialogue, rewards...).
    // AddItem is a coroutine, so it only runs when started like this.
    public void GiveItem(Item item)
    {
        if (item == null) return;
        StartCoroutine(AddItem(item));
    }

    public IEnumerator AddItem(Item item_)
    {
        if (item_ == null) yield break;

        // Only one choose screen at a time. If two items arrive together, the second one waits.
        while (choosing) yield return null;

        switch (item_.itemType)
        {
            case itemType.Equipment:
                // Is something already in this slot?
                Item old = item_.equipmentType == equipmentType.none
                    ? null
                    : equipment.Find(e => e != null && e.equipmentType == item_.equipmentType);

                if (old != null)
                {
                    pickEquipment = null;

                    TMP_Text[] text = chooseEquipmentUI.GetComponentsInChildren<TMP_Text>();

                    text[0].text = $"{item_.name} : {item_.stats.MaxHealth} HP, " +
                        $"{item_.stats.Armor} Armor \n {item_.itemDesc}";
                    text[1].text = $"{old.name} : {old.stats.MaxHealth} HP, " +
                        $"{old.stats.Armor} Armor \n {old.itemDesc}";

                    yield return Choose(chooseEquipmentUI, () => pickEquipment.HasValue,
                        () => pickEquipment = true);

                    UIMovement.instance.SetRandomSelectedUIElement();

                    if (pickEquipment == false) yield break; // kept the old one, new one is thrown away

                    RemoveItem(old);
                }

                equipment.Add(item_);
                break;

            case itemType.Consumable:
                if (consumables.Count < maxConsumables)
                {
                    consumables.Add(item_);
                    break;
                }

                pickConsumable = null;
                yield return Choose(chooseConsumableUI, () => pickConsumable.HasValue,
                    () => pickConsumable = maxConsumables);

                int slot = pickConsumable.Value;
                if (slot < 0 || slot >= consumables.Count) yield break; // dropped the new one

                // The new consumable takes the dropped one's slot.
                UnregisterTrigger(consumables[slot]);
                consumables[slot] = item_;
                break;

            case itemType.relic:
                items.Add(item_);
                break;
        }

        // Only reached if the item was actually kept.
        RegisterTrigger(item_);
        ItemAdded?.Invoke(item_);

        if (item_.stats.hasStats)
        {
            //change the stats of the player when the item is picked up
            playerCombatant.maxHp += item_.stats.MaxHealth;
            playerCombatant.armor += item_.stats.Armor;
        }

        item_.OnPickup();
    }

    // Removes an item from the inventory and stops its trigger.
    public void RemoveItem(Item item)
    {
        bool removed = false;

        if (item.itemType == itemType.relic) removed = items.Remove(item);
        else if (item.itemType == itemType.Equipment) removed = equipment.Remove(item);
        else if (item.itemType == itemType.Consumable) removed = consumables.Remove(item);

        if (removed) UnregisterTrigger(item);
        if (removed) playerCombatant.maxHp -= item.stats.MaxHealth;
        if (removed) playerCombatant.armor -= item.stats.Armor;
    }

    public void PickEquipmentButton(bool keepNewItem)
    {
        pickEquipment = keepNewItem;
    }

    public void PickConsumableButton(int index)
    {
        pickConsumable = index;
    }

    // Opens a choose screen and waits until the player has picked.
    IEnumerator Choose(GameObject ui, Func<bool> hasPicked, Action fallback)
    {
        if (ui == null)
        {
            Debug.LogWarning("Inventory: choose UI isn't set up, picking automatically.");
            fallback();
            yield break;
        }

        choosing = true;

        ui.SetActive(true);

        UIMovement.instance.SetSelectedUIElement(ui.GetComponentInChildren<Button>().gameObject);
        yield return new WaitUntil(hasPicked);
        ui.SetActive(false);
        choosing = false;
    }

    void RegisterTrigger(Item item)
    {
        if (item.trigger == null || item.trigger.triggerType == TriggerType.none) return;
        if (item.trigger.triggerAction == null || item.trigger.triggerAction.Count == 0) return;

        Action<TriggerContext> listener = context =>
        {
            foreach (TriggerActions action in item.trigger.triggerAction)
            {
                if (action != null) action.Execute(context);
            }
        };

        TriggerManager.Instance.RegisterListener(item.trigger.triggerType, listener);
        triggerListeners.Add((item, item.trigger.triggerType, listener));
    }

    void UnregisterTrigger(Item item)
    {
        // Only removes one, in case you're holding two of the same item.
        int index = triggerListeners.FindIndex(t => t.item == item);
        if (index < 0) return;

        var entry = triggerListeners[index];
        TriggerManager.Instance.UnregisterListener(entry.type, entry.listener);
        triggerListeners.RemoveAt(index);
    }
}
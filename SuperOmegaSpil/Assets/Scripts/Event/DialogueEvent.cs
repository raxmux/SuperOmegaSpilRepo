using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
struct Consequence
{
    [Header("")]
    [SerializeField] public Item itemToAdd;
    [SerializeField] public int moneyToAdd;

    [Header("")]
    [SerializeField] public int healthloss;


}

[CreateAssetMenu(fileName = "New Dialogue Event", menuName = "Event/Dialogue Event")]
public class DialogueEvent : Event
{
    protected override EventType EventType => EventType.Dialogue;

    [Header("Dialogue settings")]

    [SerializeField, TextArea(0, 10)] string dialoguePromt;
    [SerializeField] List<string> dialogueOptions = new List<string>() { "Yes", "No" };

    [Header("Consequence settings")]
    [SerializeField] List<Consequence> consequences = new List<Consequence>();

    protected override void StartEvent()
    {
        base.StartEvent();

        EventManager.instance.dialogueUI.SetActive(true);

    }
    public void ChooseOption(int buttonIndex)
    {
        Inventory.instance.AddMoney(consequences[buttonIndex].moneyToAdd);
        Inventory.instance.AddItem(consequences[buttonIndex].itemToAdd);

        EndEvent();
    }


}

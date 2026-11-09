using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

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
    public override EventType EventType => EventType.Dialogue;

    [Header("Dialogue settings")]

    [SerializeField, TextArea(0, 10)] string dialoguePromt;
    [SerializeField] List<string> dialogueOptions = new List<string>() { "Yes", "No" };

    [Header("Consequence settings")]
    [SerializeField] List<Consequence> consequences = new List<Consequence>();

    public override void StartEvent()
    {
        base.StartEvent();

        EventManager.instance.dialogueUI.SetActive(true);
        EventManager.instance.dialoguePrompt.text = dialoguePromt;

        for (int i = 0; i < dialogueOptions.Count; i++)
        {
            EventManager.instance.dialogueOptions[i].SetActive(true);
            EventManager.instance.dialogueOptions[i].GetComponentInChildren<TMP_Text>().text = dialogueOptions[i];
        }
    }
    public void ChooseOption(int buttonIndex)
    {
        Inventory.instance.AddMoney(consequences[buttonIndex].moneyToAdd);
        Inventory.instance.AddItem(consequences[buttonIndex].itemToAdd);

        EndEvent();
    }
    public override void EndEvent()
    {
        base.EndEvent();

        for (int i = 0; i < dialogueOptions.Count; i++)
        {
            EventManager.instance.dialogueOptions[i].SetActive(false);
        }

        EventManager.instance.dialogueUI.SetActive(false);
    }

}

using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

[Serializable]
struct Consequence
{

    [SerializeField, TextArea(0, 10)] public string ConsequencePrompt;

    [Header("")]
    [SerializeField] public Item itemToAdd;
    [SerializeField] public int moneyToAdd;

    [Header("")]
    [SerializeField] public int healthloss;

    [Header("Custom Consequence")]
    [SerializeField] public CustomConsequences customConsequence;

}

[CreateAssetMenu(fileName = "New Dialogue Event", menuName = "Event/Dialogue Event")]
public class DialogueEvent : Event
{
    public override EventType EventType => EventType.Dialogue;

    [Header("Dialogue settings")]

    [SerializeField, TextArea(0, 10)] string dialoguePrompt;
    [SerializeField] List<string> dialogueOptions = new List<string>() { "Yes", "No" };

    [Header("Consequence settings")]
    [SerializeField] List<Consequence> consequences = new List<Consequence>();

    public override void StartEvent()
    {
        base.StartEvent();

        EventManager.instance.dialogueUI.SetActive(true);
        EventManager.instance.dialoguePrompt.text = dialoguePrompt;

        UIMovement.instance.SetSelectedUIElement(EventManager.instance.dialogueOptions[0]);

        for (int i = 0; i < dialogueOptions.Count; i++)
        {
            EventManager.instance.dialogueOptions[i].SetActive(true);
            EventManager.instance.dialogueOptions[i].GetComponentInChildren<TMP_Text>().text = dialogueOptions[i];
        }
    }
    public void ChooseOption(int buttonIndex)
    {
        EventManager.instance.dialoguePrompt.text = consequences[buttonIndex].ConsequencePrompt;
        
        EventManager.instance.continueOption.SetActive(true);
        UIMovement.instance.SetSelectedUIElement(EventManager.instance.continueOption);

        for (int i = 0; i < dialogueOptions.Count; i++)
        {
            EventManager.instance.dialogueOptions[i].SetActive(false);
        }

        Inventory.instance.AddMoney(consequences[buttonIndex].moneyToAdd);
        Inventory.instance.GiveItem(consequences[buttonIndex].itemToAdd);

        Combatant player = GameObject.FindGameObjectWithTag("CombatPlayer").GetComponent<Combatant>();
        player.TakeDamage(consequences[buttonIndex].healthloss);

        consequences[buttonIndex].customConsequence?.Execute();
    }
    public override void EndEvent()
    {
        base.EndEvent();

        for (int i = 0; i < dialogueOptions.Count; i++)
        {
            EventManager.instance.dialogueOptions[i].SetActive(false);
        }

        EventManager.instance.continueOption.SetActive(false);
        EventManager.instance.dialogueUI.SetActive(false);
    }

}
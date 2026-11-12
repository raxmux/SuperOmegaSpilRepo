using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "End Event", menuName = "Event/End Event")]
public class EndEvent : Event
{
    public override EventType EventType => EventType.end;
    public override void StartEvent()
    {
        base.StartEvent();

        EventManager.instance.endUI.SetActive(true);
            
        UIMovement.instance.SetSelectedUIElement(EventManager.instance.endUI.transform.GetComponentInChildren<Button>().gameObject);
        Debug.LogWarning("End Event Started");
    } 
}

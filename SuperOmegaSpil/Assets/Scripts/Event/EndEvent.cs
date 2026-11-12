using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EndEvent : Event
{
    public override void StartEvent()
    {
        base.StartEvent();

        EventManager.instance.endUI.SetActive(true);
    }
}

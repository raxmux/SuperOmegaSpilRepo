using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EndEvent : Event
{
    public override void StartEvent()
    {
        base.StartEvent();

        EventManager.instance.endUI.SetActive(false);
    }
    public void BackButton()
    {
        EventManager.instance.endUI.SetActive(false);
        
        EndEvent();
    }
    public void EndButton()
    {
        MapMovement.instance.gameObject.transform.position = new Vector3(0, 0, 0);
        MapMovement.instance.roomSpawner.SpawnRooms();

        EndEvent();
    }
}

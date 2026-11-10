using Unity.VisualScripting;
using UnityEngine;

public enum EventType
{
    Shop,
    Enemy,
    Dialogue,
    Other
}

[CreateAssetMenu(fileName = "New Event", menuName = "Event/Other Event")]
public class Event : ScriptableObject
{
    private EventType eventType = EventType.Other;
    public virtual EventType EventType => eventType;

    public virtual void StartEvent()
    {
        //start the event
        

        OnEventStart();
    }
    protected virtual void OnEventStart()
    {
        //everything you want to do when the event starts, like setting variables, etc. Set in the child class
    }
    public virtual void EndEvent()
    {
        //end the event

        OnEventEnd();
    }
    protected virtual void OnEventEnd()
    {
        //everything you want to do when the event ends, like resetting variables, etc. Set in the child class
    }
}

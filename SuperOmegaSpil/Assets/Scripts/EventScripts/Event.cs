using Unity.VisualScripting;
using UnityEngine;

public enum EventType
{
    Shop,
    Enemy,
    Dialogue,
    Other
}
public class Event : MonoBehaviour
{
    private EventType eventType = EventType.Other;
    protected virtual EventType EventType => eventType;

    protected virtual void StartEvent()
    {
        //start the event

        OnEventStart();
    }
    protected virtual void OnEventStart()
    {
        //everything you want to do when the event starts, like setting variables, etc. Set in the child class
    }
    protected virtual void EndEvent()
    {
        //start the event

        OnEventEnd();
    }
    protected virtual void OnEventEnd()
    {
        //everything you want to do when the event ends, like resetting variables, etc. Set in the child class
    }
}

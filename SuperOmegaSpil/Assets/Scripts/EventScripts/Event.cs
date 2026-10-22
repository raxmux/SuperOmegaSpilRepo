using UnityEngine;

public class Event : MonoBehaviour
{
    protected void StartEvent()
    {
        //start the event

        OnEventStart();
    }
    protected void OnEventStart()
    {
        //everything you want to do when the event starts, like setting variables, etc. Set in the child class
    }
    protected void EndEvent()
    {
        //start the event

        OnEventEnd();
    }
    protected void OnEventEnd()
    {
        //everything you want to do when the event ends, like resetting variables, etc. Set in the child class
    }
}

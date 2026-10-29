using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public enum TriggerType
{
    EnemyKilled,
    PlayerDamaged,
    playerHealed

}

public struct TriggerContext
{
    public readonly GameObject Source;
    public readonly GameObject Target;
    public readonly float Amount;

    public TriggerContext(GameObject source, GameObject target, float amount)
    {
        Source = source;
        Target = target;
        Amount = amount;
    }
}
public class TriggerManager : MonoBehaviour
{
    private Dictionary<TriggerType,List<Action<TriggerContext>>> listeners 
        = new Dictionary<TriggerType, List<Action<TriggerContext>>>();

    public void TriggerEvent(TriggerType triggerType, TriggerContext context)
    {
        // Check if the trigger type has any listeners
        if (listeners.ContainsKey(triggerType))
        {
            // Invoke all registered functions for the specified trigger type
            foreach (var listener in listeners[triggerType])
            {
                listener.Invoke(context);
            }
        }
    }

    public void RegisterListener(TriggerType triggerType, Action<TriggerContext> function)
    {
        // Check if the trigger type already has a list of listeners, if not, create one
        if (!listeners.ContainsKey(triggerType)) listeners[triggerType] = new List<Action<TriggerContext>>();
        
        //Add the function to the list of listeners for the specified trigger type
        listeners[triggerType].Add(function);
    }
    public void UnregisterListener(TriggerType triggerType, Action<TriggerContext> function)
    {
        // Check if the trigger type has a list of listeners
        if (listeners.ContainsKey(triggerType))
        {
            // Remove the function from the list of listeners for the specified trigger type
            listeners[triggerType].Remove(function);
        }
    }




}

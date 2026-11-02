using UnityEngine;

public class TimeClock : MonoBehaviour
{
    public float timeclockTime = 0f; //current time of the clock, in seconds
    float timeclockSpeed = 1f;
    float timeclockResetTime = 15f; 

    int nextTimeclockPoint = 0; //index of the next timeclock point to check against
    float[] timeclockPoints; //the lil points allong the clock yk



    void Update()
    {
        timeclockTime += Time.deltaTime * timeclockSpeed * Time.timeScale;

        if (timeclockTime >= timeclockPoints[nextTimeclockPoint])
        {
            TriggerManager.Instance.TriggerEvent(TriggerType.TimeclockPoint, new TriggerContext(null, null, timeclockTime));

            nextTimeclockPoint++;
        }


        if (timeclockTime >= timeclockResetTime)
        {
            TriggerManager.Instance.TriggerEvent(TriggerType.TimeclockReset, new TriggerContext(null, null, timeclockTime));

            timeclockTime = 0f;
        }
    }






}

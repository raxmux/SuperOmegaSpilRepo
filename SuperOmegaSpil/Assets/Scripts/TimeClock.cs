using System.Collections.Generic;
using UnityEngine;

public class TimeClock : MonoBehaviour
{
    public static TimeClock Instance { get; private set; }

    [SerializeField] float loopLength = 12f; // seconds per loop
    [SerializeField] float speed = 1f;       // 1 = real time

    public float CurrentTime { get; private set; }
    public float LoopLength => loopLength;
    public bool Paused { get; set; }

    [SerializeField] List<float> overworldPoints = new List<float> { 3f, 6f, 9f, 12f };

    // The lil points along the clock
    readonly List<float> points = new List<float>();
    public IReadOnlyList<float> Points => points;

    int nextPoint;      // index of the next point that hasn't fired this loop
    bool advancing;     // true while points are firing
    float queuedTime;   // time added while points were firing

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        ResetPoints();
    }

    void Update()
    {
        if (Paused) return;
        AdvanceTime(Time.deltaTime * speed);
    }

    public void ResetPoints() => SetPoints(overworldPoints);

    public void SetPoints(IEnumerable<float> times)
    {
        points.Clear();
        foreach (float t in times)
            points.Add(Mathf.Clamp(t, 0f, loopLength));
        points.Sort();

        // Points the hand has already passed wait until the next loop,
        // so an enemy doesn't attack the instant the fight starts.
        nextPoint = 0;
        while (nextPoint < points.Count && points[nextPoint] <= CurrentTime)
            nextPoint++;
    }

    public void ClearPoints()
    {
        points.Clear();
        nextPoint = 0;
    }


    public void AdvanceTime(float seconds)
    {
        if (seconds <= 0f) return;

        // If something adds time while a point is firing, do it afterwards,
        // so points never fire out of order.
        if (advancing)
        {
            queuedTime += seconds;
            return;
        }

        advancing = true;
        while (seconds > 0f)
        {
            Advance(seconds);
            seconds = queuedTime;
            queuedTime = 0f;
        }
        advancing = false;
    }

    void Advance(float seconds)
    {
        while (seconds > 0f)
        {
            // Go as far as the end of the loop at most, then wrap around.
            float toEnd = loopLength - CurrentTime;
            bool wraps = seconds >= toEnd;
            float target = wraps ? loopLength : CurrentTime + seconds;
            seconds = wraps ? seconds - toEnd : 0f;

            // points.Count is re-checked each time, so ClearPoints() during a point is safe.
            while (nextPoint < points.Count && points[nextPoint] <= target)
            {
                CurrentTime = points[nextPoint];
                nextPoint++;
                TriggerManager.Instance.TriggerEvent(TriggerType.TimeclockPoint,
                    new TriggerContext(null, null, CurrentTime));
            }

            CurrentTime = target;

            if (wraps)
            {
                CurrentTime = 0f;
                nextPoint = 0;
                TriggerManager.Instance.TriggerEvent(TriggerType.TimeclockReset,
                    new TriggerContext(null, null, loopLength));
            }
        }
    }
}
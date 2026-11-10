using System.Collections.Generic;
using UnityEngine;

public class ClockDisplay : MonoBehaviour
{
    [Header("Marker sprites")]
    [SerializeField] Sprite attackMarker;
    [SerializeField] Sprite bigMarker;
    [SerializeField] Sprite smallMarker;

    [Header("Markers")]
    [SerializeField] Material markerMaterial;
    [SerializeField] float markerRadius = 1.7f;
    [SerializeField] int markerSortingOrder = 20;

    [Header("Hand")]
    [SerializeField] Sprite handSprite;
    [SerializeField] int handSortingOrder = 21;
    [SerializeField] float tickDuration = 0.25f;

    [Header("Screen position")]
    [SerializeField] List<Camera> cameras = new List<Camera>();
    [SerializeField] Vector2 cornerOffset = new Vector2(2f, -2f);

    static readonly int MainColorId = Shader.PropertyToID("_mainColor");
    static readonly int SecoColorId = Shader.PropertyToID("_secoColor");

    // Index 1-12 = that second. Index 0 is unused.
    readonly SpriteRenderer[] markers = new SpriteRenderer[13];
    readonly bool[] isAttack = new bool[13];
    MaterialPropertyBlock block;
    Color mainColor, secoColor;

    // Hand movement
    Transform hand;
    int lastSecond;
    float shownAngle, fromAngle, toAngle, tweenTime;

    // Scaling between cameras
    Vector3 baseScale;
    float baseCameraSize;

    void Awake()
    {
        block = new MaterialPropertyBlock();
        mainColor = markerMaterial.GetColor(MainColorId);
        secoColor = markerMaterial.GetColor(SecoColorId);

        for (int second = 1; second <= 12; second++)
            markers[second] = SpawnMarker(second);

        hand = SpawnHand();
    }

    Transform SpawnHand()
    {
        var go = new GameObject("Hand");
        go.transform.SetParent(transform, false);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = handSprite;
        sr.sharedMaterial = markerMaterial;
        sr.sortingOrder = handSortingOrder;
        return go.transform;
    }

    void Start()
    {
        baseScale = transform.localScale;
        Camera cam = ActiveCamera();
        baseCameraSize = cam != null ? cam.orthographicSize : 1f;

        lastSecond = CurrentSecond();
        shownAngle = fromAngle = toAngle = lastSecond * 30f;
        tweenTime = tickDuration;

        // Force every marker to draw once.
        for (int s = 1; s <= 12; s++) SetMarker(s, s == 12, true);
    }

    void LateUpdate()
    {
        FollowCamera();
        UpdateMarkers();
        UpdateHand();
    }

    SpriteRenderer SpawnMarker(int second)
    {
        float angle = second * 30f; // degrees clockwise from 12
        float rad = angle * Mathf.Deg2Rad;

        var go = new GameObject("Marker " + second);
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f) * markerRadius;
        go.transform.localRotation = Quaternion.Euler(0f, 0f, 180f - angle); // points at the center

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sharedMaterial = markerMaterial;
        sr.sortingOrder = markerSortingOrder;
        return sr;
    }

    void UpdateMarkers()
    {
        bool inCombat = CombatManager.Instance != null && CombatManager.Instance.InCombat;
        var points = TimeClock.Instance != null ? TimeClock.Instance.Points : null;

        for (int s = 1; s <= 12; s++)
        {
            bool attack = s == 12; // 12 is always marked
            if (!attack && inCombat && points != null)
            {
                foreach (float p in points)
                    if (Mathf.RoundToInt(p) == s) { attack = true; break; }
            }

            SetMarker(s, attack, false);
        }
    }

    // Only touches the renderer when something changed.
    void SetMarker(int second, bool attack, bool force)
    {
        if (!force && isAttack[second] == attack) return;
        isAttack[second] = attack;

        SpriteRenderer sr = markers[second];
        sr.sprite = attack ? attackMarker
                  : second % 3 == 0 ? bigMarker
                  : smallMarker;

        sr.GetPropertyBlock(block);
        block.SetColor(MainColorId, attack ? secoColor : mainColor);
        sr.SetPropertyBlock(block);
    }
    int CurrentSecond()
    {
        return TimeClock.Instance != null ? Mathf.FloorToInt(TimeClock.Instance.CurrentTime) % 12 : 0;
    }

    void UpdateHand()
    {
        if (hand == null) return;

        int second = CurrentSecond();
        if (second != lastSecond)
        {
            int steps = (second - lastSecond + 12) % 12;
            lastSecond = second;
            fromAngle = shownAngle;
            toAngle += steps * 30f;
            tweenTime = 0f;
        }

        if (tweenTime < tickDuration)
        {
            tweenTime += Time.deltaTime;
            float t = Mathf.Clamp01(tweenTime / tickDuration);
            t = t * t * (3f - 2f * t); // speeds up, then slows down
            shownAngle = Mathf.Lerp(fromAngle, toAngle, t);
        }
        else
        {
            shownAngle = toAngle;

            // Keep the numbers small once a full loop is done.
            if (toAngle >= 360f)
            {
                toAngle -= 360f;
                fromAngle -= 360f;
                shownAngle = toAngle;
            }
        }

        hand.localRotation = Quaternion.Euler(0f, 0f, -shownAngle);
    }
    Camera ActiveCamera()
    {
        foreach (Camera cam in cameras)
            if (cam != null && cam.enabled && cam.gameObject.activeInHierarchy) return cam;
        return Camera.main;
    }

    void FollowCamera()
    {
        Camera cam = ActiveCamera();
        if (cam == null) return;

        float scale = cam.orthographicSize / baseCameraSize;
        transform.localScale = baseScale * scale;

        Vector3 corner = cam.ViewportToWorldPoint(new Vector3(0f, 1f, -cam.transform.position.z));
        Vector3 pos = corner + (Vector3)(cornerOffset * scale);
        pos.z = 0f;
        transform.position = pos;
    }
}
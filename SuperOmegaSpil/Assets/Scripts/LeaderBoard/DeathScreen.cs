using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text statsText;

    [Header("Score tally")]
    [SerializeField] float firstStepTime = 0.6f;
    [SerializeField] float speedUp = 0.8f;
    [SerializeField] float fastestStepTime = 0.05f;

    [Header("Keyboard")]
    [SerializeField] Transform keyGrid;
    [SerializeField] Button keyTemplate;
    [SerializeField] int maxNameLength = 6;

    [SerializeField] string mainMenuScene = "MainMenu";

    RunStats stats;
    List<ScoreLine> breakdown;
    string playerName = "";
    Button okKey;
    bool tallying;
    bool skipTally;
    bool saved;

    void Start()
    {
        stats = RunTracker.LastRun ?? new RunStats();
        breakdown = ScoreCalculator.GetBreakdown(stats);

        keyTemplate.gameObject.SetActive(false);
        keyGrid.gameObject.SetActive(false);
        if (nameText != null) nameText.text = "";

        StartCoroutine(Tally());
    }

    IEnumerator Tally()
    {
        tallying = true;

        string lines = KilledByLine();
        int total = 0;
        float step = firstStepTime;

        SetStats(lines);
        SetScore(0);

        foreach (ScoreLine line in breakdown)
        {
            if (skipTally) break;

            total += line.points;
            lines += $"{line.label}: {line.value}\n";
            SetStats(lines);
            SetScore(total);

            for (float t = 0f; t < step && !skipTally; t += Time.deltaTime)
                yield return null;

            step = Mathf.Max(fastestStepTime, step * speedUp);
        }

        SetStats(FullStatsText());
        SetScore(ScoreCalculator.Calculate(stats));
        yield return new WaitForSecondsRealtime(0.3f);

        tallying = false;
        keyGrid.gameObject.SetActive(true);
        BuildKeyboard();
        UpdateName();
    }

    // Selection action, for if ur a bum
    public void Skip(InputAction.CallbackContext context)
    {
        if (context.performed && tallying) skipTally = true;
    }

    // Back action type shit
    public void Back(InputAction.CallbackContext context)
    {
        if (context.performed && !tallying) Delete();
    }

    void BuildKeyboard()
    {
        Button first = null;
        for (char c = 'A'; c <= 'Z'; c++)
        {
            char letter = c;
            Button key = MakeKey(letter.ToString(), () => Type(letter));
            if (first == null) first = key;
        }
        MakeKey("DEL", Delete);
        okKey = MakeKey("OK", Submit);

        if (EventSystem.current != null && first != null)
            EventSystem.current.SetSelectedGameObject(first.gameObject);
    }

    Button MakeKey(string label, UnityAction onClick)
    {
        Button key = Instantiate(keyTemplate, keyGrid);
        key.gameObject.SetActive(true);
        key.name = "Key " + label;

        var text = key.GetComponentInChildren<TMP_Text>(true);
        if (text != null) text.text = label;

        key.onClick.AddListener(onClick);
        return key;
    }

    void Type(char letter)
    {
        if (playerName.Length >= maxNameLength) return;
        playerName += letter;
        UpdateName();
    }

    public void Delete()
    {
        if (playerName.Length == 0) return;
        playerName = playerName.Substring(0, playerName.Length - 1);
        UpdateName();
    }

    public void Submit()
    {
        if (saved || tallying || playerName.Length == 0) return;
        saved = true;

        Leaderboard.AddEntry(playerName, stats);
        SceneManager.LoadScene(mainMenuScene);
    }

    void UpdateName()
    {
        if (nameText != null) nameText.text = playerName.PadRight(maxNameLength, '_');
        if (okKey != null) okKey.interactable = playerName.Length > 0;
    }

    void SetScore(int score)
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }

    void SetStats(string text)
    {
        if (statsText != null) statsText.text = text;
    }

    string KilledByLine()
    {
        return string.IsNullOrEmpty(stats.killedBy) ? "" : $"Killed by {stats.killedBy}\n\n";
    }

    string FullStatsText()
    {
        string text = KilledByLine();
        foreach (ScoreLine line in breakdown)
            text += $"{line.label}: {line.value}\n";

        text += $"Damage taken: {Mathf.RoundToInt(stats.damageTaken)}\n";
        text += $"Money spent: {stats.moneySpent}";
        return text;
    }
}
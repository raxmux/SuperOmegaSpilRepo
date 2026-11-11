using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

// For the main menu. Fills two text boxes with the daily and global top 10.
public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text dailyText;
    [SerializeField] TMP_Text globalText;
    [SerializeField] int count = 10;
    [SerializeField] Color newScoreColor = new Color(1f, 0.85f, 0.2f);

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (dailyText != null) dailyText.text = Format(Leaderboard.GetDaily(count));
        if (globalText != null) globalText.text = Format(Leaderboard.GetGlobal(count));
    }

    string Format(List<LeaderboardEntry> entries)
    {
        if (entries.Count == 0) return "No scores yet";

        string hex = ColorUtility.ToHtmlStringRGB(newScoreColor);
        string text = "<mspace=0.6em>";

        for (int i = 0; i < entries.Count; i++)
        {
            LeaderboardEntry e = entries[i];
            string line = $"{i + 1,2}. {e.name,-6} {e.score,8}";

            bool isNew = Leaderboard.LastEntry != null
                && e.name == Leaderboard.LastEntry.name
                && e.date == Leaderboard.LastEntry.date;

            text += isNew ? $"<color=#{hex}>{line}</color>\n" : line + "\n";
        }

        return text + "</mspace>";
    }
}
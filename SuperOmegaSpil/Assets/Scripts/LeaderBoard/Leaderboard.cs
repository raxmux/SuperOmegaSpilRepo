using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string name;
    public int score;
    public string date;
    public RunStats stats;
}

[Serializable]
class LeaderboardFile
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public static class Leaderboard
{
    public static LeaderboardEntry LastEntry { get; private set; }

    static string Folder => Path.Combine(Application.persistentDataPath, "Leaderboard");

    public static LeaderboardEntry AddEntry(string name, RunStats stats)
    {
        var entry = new LeaderboardEntry
        {
            name = name,
            score = ScoreCalculator.Calculate(stats),
            date = DanishNow().ToString("yyyy-MM-dd HH:mm:ss"),
            stats = stats
        };

        string path = PathFor(DanishNow());
        LeaderboardFile file = Load(path);
        file.entries.Add(entry);
        Save(path, file);

        LastEntry = entry;
        return entry;
    }

    public static List<LeaderboardEntry> GetDaily(int count = 10)
    {
        return Top(Load(PathFor(DanishNow())).entries, count);
    }

    public static List<LeaderboardEntry> GetGlobal(int count = 10)
    {
        var all = new List<LeaderboardEntry>();
        if (Directory.Exists(Folder))
            foreach (string path in Directory.GetFiles(Folder, "*.json"))
                all.AddRange(Load(path).entries);

        return Top(all, count);
    }

    static List<LeaderboardEntry> Top(List<LeaderboardEntry> entries, int count)
    {
        var sorted = new List<LeaderboardEntry>(entries);
        sorted.Sort((a, b) => b.score != a.score
            ? b.score.CompareTo(a.score)
            : string.CompareOrdinal(a.date, b.date)); // ties: whoever got it first
        if (sorted.Count > count) sorted.RemoveRange(count, sorted.Count - count);
        return sorted;
    }

    static string PathFor(DateTime day) => Path.Combine(Folder, day.ToString("yyyy-MM-dd") + ".json");

    static LeaderboardFile Load(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                var file = JsonUtility.FromJson<LeaderboardFile>(File.ReadAllText(path));
                if (file != null && file.entries != null) return file;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Leaderboard: couldn't read {path}: {e.Message}");
        }
        return new LeaderboardFile();
    }

    // Writes to a temp file first, so errors wont just fucking slime out the entire leaderboard yo :>
    static void Save(string path, LeaderboardFile file)
    {
        Directory.CreateDirectory(Folder);
        string temp = path + ".tmp";
        File.WriteAllText(temp, JsonUtility.ToJson(file, true));
        if (File.Exists(path)) File.Delete(path);
        File.Move(temp, path);
    }

    static TimeZoneInfo danish;

    public static DateTime DanishNow()
    {
        if (danish == null)
            danish = FindZone("Europe/Copenhagen") ?? FindZone("Romance Standard Time")
                  ?? TimeZoneInfo.CreateCustomTimeZone("Denmark", TimeSpan.FromHours(1), "Denmark", "Denmark");

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, danish); //Fuck jeg elsker og hader den makker der lavede det her shit >:(
    }

    static TimeZoneInfo FindZone(string id)
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
        catch { return null; }
    }
}
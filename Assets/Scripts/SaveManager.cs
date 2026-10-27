using UnityEngine;

public readonly struct LevelResult
{
    public readonly int level;
    public readonly int stars;
    public readonly float time;
    public readonly float integrityPercent;
    public readonly int score;

    public LevelResult(int level, int stars, float time, float integrityPercent, int score)
    {
        this.level = level;
        this.stars = stars;
        this.time = time;
        this.integrityPercent = integrityPercent;
        this.score = score;
    }
}

public readonly struct LevelRecord
{
    public readonly int stars;
    public readonly float time;
    public readonly float integrityPercent;
    public readonly int score;

    public bool Completed => stars > 0;

    public LevelRecord(int stars, float time, float integrityPercent, int score)
    {
        this.stars = stars;
        this.time = time;
        this.integrityPercent = integrityPercent;
        this.score = score;
    }
}

public static class SaveManager
{
    private const string Prefix = "OrbitalFlux.";
    private const string UnlockedKey = Prefix + "UnlockedLevel";
    private const string PushKey = Prefix + "Settings.PushStrength";
    private const string PullKey = Prefix + "Settings.PullStrength";
    private const string VolumeKey = Prefix + "Settings.MasterVolume";

    public static int UnlockedLevel => Mathf.Max(1, PlayerPrefs.GetInt(UnlockedKey, 1));

    public static bool IsLevelUnlocked(int level) => level <= UnlockedLevel;

    public static void UnlockLevel(int level)
    {
        if (level > UnlockedLevel)
            PlayerPrefs.SetInt(UnlockedKey, level);
    }

    private static string Key(int level, string field) => $"{Prefix}Level{level}.{field}";

    public static LevelRecord GetRecord(int level)
    {
        return new LevelRecord(
            PlayerPrefs.GetInt(Key(level, "Stars"), 0),
            PlayerPrefs.GetFloat(Key(level, "Time"), 0f),
            PlayerPrefs.GetFloat(Key(level, "Integrity"), 0f),
            PlayerPrefs.GetInt(Key(level, "Score"), 0));
    }

    public static bool SubmitResult(LevelResult result)
    {
        LevelRecord old = GetRecord(result.level);

        bool isBetter = false;
        if (!old.Completed)
        {
            isBetter = true;
        }
        else if (result.score > old.score)
        {
            isBetter = true;
        }

        if (!isBetter) return false;

        PlayerPrefs.SetInt(Key(result.level, "Stars"), result.stars);
        PlayerPrefs.SetFloat(Key(result.level, "Time"), result.time);
        PlayerPrefs.SetFloat(Key(result.level, "Integrity"), result.integrityPercent);
        PlayerPrefs.SetInt(Key(result.level, "Score"), result.score);
        PlayerPrefs.Save();
        return true;
    }

    public static float PushStrength
    {
        get => PlayerPrefs.GetFloat(PushKey, 1f);
        set => PlayerPrefs.SetFloat(PushKey, Mathf.Clamp01(value));
    }

    public static float PullStrength
    {
        get => PlayerPrefs.GetFloat(PullKey, 1f);
        set => PlayerPrefs.SetFloat(PullKey, Mathf.Clamp01(value));
    }

    public static float MasterVolume
    {
        get => PlayerPrefs.GetFloat(VolumeKey, 1f);
        set => PlayerPrefs.SetFloat(VolumeKey, Mathf.Clamp01(value));
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
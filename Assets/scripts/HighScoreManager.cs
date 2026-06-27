using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class HighScoreEntry
{
    public string playerName;
    public float lapTime;
}

[System.Serializable]
public class HighScoreData
{
    public List<HighScoreEntry> scores = new List<HighScoreEntry>();
}

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;
    private const string PREF_HIGH_SCORES = "LapHighScores";
    private const int MAX_SCORES = 5;

    private HighScoreData scoreData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadScores()
    {
        if (PlayerPrefs.HasKey(PREF_HIGH_SCORES))
        {
            string json = PlayerPrefs.GetString(PREF_HIGH_SCORES);
            scoreData = JsonUtility.FromJson<HighScoreData>(json);
        }
        else
        {
            scoreData = new HighScoreData();
        }
    }

    public void AddScore(string name, float time)
    {
        scoreData.scores.Add(new HighScoreEntry { playerName = name, lapTime = time });
        
        // Sort ascending (lower time is better)
        scoreData.scores = scoreData.scores.OrderBy(s => s.lapTime).ToList();

        // Keep only top MAX_SCORES
        if (scoreData.scores.Count > MAX_SCORES)
        {
            scoreData.scores.RemoveRange(MAX_SCORES, scoreData.scores.Count - MAX_SCORES);
        }

        SaveScores();
    }

    private void SaveScores()
    {
        string json = JsonUtility.ToJson(scoreData);
        PlayerPrefs.SetString(PREF_HIGH_SCORES, json);
        PlayerPrefs.Save();
    }

    public List<HighScoreEntry> GetTopScores()
    {
        return scoreData.scores;
    }
}

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class KnownPlayersData
{
    public List<string> players = new List<string>();
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public string CurrentPlayerName { get; private set; } = "Guest";
    
    public List<string> KnownPlayers { get; private set; } = new List<string>();

    private const string PREF_PLAYER_NAME = "LastPlayerName";
    private const string PREF_KNOWN_PLAYERS = "KnownPlayersList";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
            newName = "Guest";
            
        CurrentPlayerName = newName;
        PlayerPrefs.SetString(PREF_PLAYER_NAME, CurrentPlayerName);
        
        // Add to known players if new
        if (!KnownPlayers.Contains(CurrentPlayerName))
        {
            KnownPlayers.Add(CurrentPlayerName);
            SaveKnownPlayers();
        }

        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey(PREF_PLAYER_NAME))
        {
            CurrentPlayerName = PlayerPrefs.GetString(PREF_PLAYER_NAME);
        }

        if (PlayerPrefs.HasKey(PREF_KNOWN_PLAYERS))
        {
            string json = PlayerPrefs.GetString(PREF_KNOWN_PLAYERS);
            KnownPlayersData data = JsonUtility.FromJson<KnownPlayersData>(json);
            if (data != null && data.players != null)
            {
                KnownPlayers = data.players;
            }
        }
    }

    private void SaveKnownPlayers()
    {
        KnownPlayersData data = new KnownPlayersData();
        data.players = KnownPlayers;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(PREF_KNOWN_PLAYERS, json);
    }
}

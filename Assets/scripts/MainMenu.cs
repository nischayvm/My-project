using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInputField;
    public TMP_Dropdown playerDropdown;
    public TMP_Text highScoresText;

    private void Start()
    {
        // Initialize managers if they don't exist yet (useful when testing MainMenu directly)
        if (PlayerManager.Instance == null)
        {
            GameObject pm = new GameObject("PlayerManager");
            pm.AddComponent<PlayerManager>();
        }
        if (HighScoreManager.Instance == null)
        {
            GameObject hm = new GameObject("HighScoreManager");
            hm.AddComponent<HighScoreManager>();
        }

        InitializeUI();
    }

    private void InitializeUI()
    {
        // 1. Setup Dropdown
        if (playerDropdown != null && PlayerManager.Instance != null)
        {
            playerDropdown.ClearOptions();
            
            List<string> options = new List<string>();
            options.Add("-- Select Existing Player --");
            options.AddRange(PlayerManager.Instance.KnownPlayers);
            
            playerDropdown.AddOptions(options);
            
            // Add listener
            playerDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        // 2. Setup Input Field
        if (nameInputField != null && PlayerManager.Instance != null)
        {
            nameInputField.text = PlayerManager.Instance.CurrentPlayerName;
        }

        // 3. Setup High Scores
        UpdateHighScoresDisplay();
    }

    private void OnDropdownValueChanged(int index)
    {
        if (index > 0 && nameInputField != null && PlayerManager.Instance != null)
        {
            // Index 0 is "-- Select Existing Player --"
            string selectedName = PlayerManager.Instance.KnownPlayers[index - 1];
            nameInputField.text = selectedName;
        }
    }

    private void UpdateHighScoresDisplay()
    {
        if (highScoresText == null || HighScoreManager.Instance == null) return;

        string display = "<b>HIGH SCORES</b>\n\n";
        var scores = HighScoreManager.Instance.GetTopScores();
        if (scores.Count == 0)
        {
            display += "No scores yet!\nBe the first to complete a lap.";
        }
        else
        {
            for (int i = 0; i < scores.Count; i++)
            {
                display += $"{i + 1}. {scores[i].playerName} - {FormatTime(scores[i].lapTime)}\n";
            }
        }
        highScoresText.text = display;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = time % 60;
        return string.Format("{0:00}:{1:00.000}", minutes, seconds);
    }

    public void PlayGame()
    {
        if (nameInputField != null && PlayerManager.Instance != null)
        {
            PlayerManager.Instance.SetPlayerName(nameInputField.text);
        }
        SceneManager.LoadScene("v1");
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
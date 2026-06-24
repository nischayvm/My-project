using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    public TMP_Text bestLapText;
    public TMP_Text recentLapsText;

    public LapTimer lapTimer;

    bool paused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        paused = !paused;

        pausePanel.SetActive(paused);

        if (paused)
        {
            UpdateLapDisplay();
        }

        Time.timeScale = paused ? 0f : 1f;
    }

    void UpdateLapDisplay()
    {
        if (lapTimer == null)
            return;

        bestLapText.text =
            "BEST LAP\n" +
            lapTimer.FormatTime(
                lapTimer.bestLapTime
            );

        recentLapsText.text = "";

        int start =
            Mathf.Max(
                0,
                lapTimer.lapHistory.Count - 5
            );

        for (
            int i =
                lapTimer.lapHistory.Count - 1;
            i >= start;
            i--
        )
        {
            recentLapsText.text +=
                "Lap " +
                (i + 1) +
                "   " +
                lapTimer.FormatTime(
                    lapTimer.lapHistory[i]
                ) +
                "\n";
        }
    }

    public void Resume()
    {
        paused = false;

        pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            "MainMenu"
        );
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LapTimer : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text timerText;
    public TMP_Text bestLapText;
    public TMP_Text lapCountText;
    public TMP_Text announcementText;

    private float currentLapTime = 0f;
    public float bestLapTime = Mathf.Infinity;
    public List<float> lapHistory = new List<float>();
    private int completedLaps = 0;
    private bool running = false;
    private Coroutine announcementCoroutine;

    void Start()
    {
        if (bestLapText != null) bestLapText.text = "BEST\n--:--.---";
        if (lapCountText != null) lapCountText.text = "LAP 1";
        if (announcementText != null) announcementText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!running)
            return;

        currentLapTime += Time.deltaTime;
        if (timerText != null)
        {
            timerText.text = FormatTime(currentLapTime);
        }
    }

    public void LapCompleted()
    {
        try
        {
            // Prevent completing a lap instantly (e.g., if the car starts inside the finish line trigger)
            if (currentLapTime < 3f)
            {
                Debug.LogWarning("Lap completed too fast (" + currentLapTime + "s). Ignored. Did you start inside the finish line?");
                return;
            }

            bool isNewBest = false;

            if (completedLaps == 0 || currentLapTime < bestLapTime)
            {
                bestLapTime = currentLapTime;
                isNewBest = true;
                if (bestLapText != null)
                {
                    bestLapText.text = "BEST\n" + FormatTime(bestLapTime);
                }
            }

            completedLaps++;
            
            if (lapCountText != null)
            {
                lapCountText.text = "LAP " + (completedLaps + 1);
            }

            if (isNewBest)
            {
                ShowAnnouncement("NEW BEST LAP!");
            }
            else
            {
                ShowAnnouncement("LAP COMPLETE!");
            }

            lapHistory.Add(currentLapTime);

            if (HighScoreManager.Instance != null && PlayerManager.Instance != null)
            {
                HighScoreManager.Instance.AddScore(PlayerManager.Instance.CurrentPlayerName, currentLapTime);
            }

            // Reset current lap time, but keep running
            currentLapTime = 0f;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("CRASH IN LAPCOMPLETED! Error: " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    private void ShowAnnouncement(string message)
    {
        if (announcementText == null) return;

        if (announcementCoroutine != null)
        {
            StopCoroutine(announcementCoroutine);
        }
        announcementCoroutine = StartCoroutine(AnnouncementRoutine(message, 2f));
    }

    private IEnumerator AnnouncementRoutine(string message, float duration)
    {
        announcementText.text = message;
        announcementText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(duration);
        
        announcementText.gameObject.SetActive(false);
    }

    public string FormatTime(float time)
    {
        if (time <= 0 || float.IsInfinity(time))
        {
            return "--:--.---";
        }

        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = time % 60;
        return string.Format("{0:00}:{1:00.000}", minutes, seconds);
    }

    public void StartTimer()
    {
        running = true;
    }

    public void StopTimer()
    {
        running = false;
    }
}
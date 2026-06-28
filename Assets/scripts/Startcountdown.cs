using UnityEngine;
using TMPro;
using System.Collections;

public class StartCountdown : MonoBehaviour
{
    public TMP_Text countdownText;
    public CarController carController;
    public LapTimer lapTimer;

    public AudioSource engineRoarAudio;

    // Drag the marshal here in the Inspector
    public Animator marshalAnimator;

    IEnumerator Start()
    {
        carController.canDrive = false;

        if (engineRoarAudio != null)
        {
            engineRoarAudio.Play();
        }

        countdownText.text = "3";
        yield return new WaitForSeconds(1);

        countdownText.text = "2";
        yield return new WaitForSeconds(1);

        countdownText.text = "1";
        yield return new WaitForSeconds(1);

        countdownText.text = "GO!";

        // Make the marshal wave
        if (marshalAnimator != null)
        {
            marshalAnimator.SetTrigger("Wave");
        }

        carController.StartRace();
        lapTimer.StartTimer();

        yield return new WaitForSeconds(1);

        countdownText.gameObject.SetActive(false);
    }
}
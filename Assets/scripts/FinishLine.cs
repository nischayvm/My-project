using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public LapTimer lapTimer;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("FINISH LINE HIT BY: " + other.gameObject.name + " | TAG: " + other.tag);

        if (other.CompareTag("Player"))
        {
            if (lapTimer != null)
            {
                Debug.Log("Crossed Finish Line! Calling LapCompleted().");
                lapTimer.LapCompleted();
            }
            else
            {
                Debug.LogWarning("Player crossed Finish Line, but lapTimer is NOT assigned in the Inspector!");
            }
        }
    }
}
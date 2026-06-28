using UnityEngine;

public class CrashDetector : MonoBehaviour
{
    public AudioSource crashAudio;
    public AudioClip crashClip;

    public float cooldown = 0.5f;

    private float lastCrashTime;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Traffic"))
            return;

        if (Time.time - lastCrashTime < cooldown)
            return;

        lastCrashTime = Time.time;

        crashAudio.Stop();
        crashAudio.clip = crashClip;
        crashAudio.volume = 1f;
        crashAudio.Play();
    }
}
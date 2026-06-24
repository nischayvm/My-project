using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public Rigidbody rb;
    public TMP_Text speedText;

    void Update()
    {
        if (rb == null || speedText == null)
            return;

        // Calculate speed in km/h
        float speedKmh = rb.linearVelocity.magnitude * 3.6f;

        // Display integer speed
        speedText.text = Mathf.RoundToInt(speedKmh).ToString() + " KM/H";
    }
}

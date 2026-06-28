using UnityEngine;

public class RampBoost : MonoBehaviour
{
    public float upwardBoost = 4f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;

        if (rb == null)
            return;

        Vector3 vel = rb.linearVelocity;

        // Cap upward launch speed
        if (vel.y > upwardBoost)
        {
            vel.y = upwardBoost;
            rb.linearVelocity = vel;
        }
    }
}

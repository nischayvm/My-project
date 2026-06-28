using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class TrafficFollower : MonoBehaviour
{
    [Header("References")]
    public SplineContainer spline;

    [Header("Movement")]
    public float speed = 10f;
    [Header("Wheels")]
    public Transform frontLeft;
    public Transform frontRight;
    public Transform rearLeft;
    public Transform rearRight;

    public float wheelSpinSpeed = 800f;

    [Range(0f, 1f)]
    public float startPosition = 0f;

    public float heightOffset = 0f;

    private float t;

    void Start()
    {
        t = startPosition;
    }

    void Update()
    {
        if (spline == null)
            return;

        float length = spline.Spline.GetLength();

        t += (speed / length) * Time.deltaTime;

        if (t > 1f)
            t -= 1f;

        float3 pos = spline.EvaluatePosition(t);
        float3 tangent = spline.EvaluateTangent(t);

        transform.position = new Vector3(
            pos.x,
            pos.y + heightOffset,
            pos.z
        );

        Vector3 forward = new Vector3(
            tangent.x,
            tangent.y,
            tangent.z
        );

        if (forward.sqrMagnitude > 0.001f)
        {
            transform.rotation =
    Quaternion.LookRotation(forward.normalized, Vector3.up) *
    Quaternion.Euler(0f, 180f, 0f);
        }
        float spin = wheelSpinSpeed * Time.deltaTime;

        if (frontLeft) frontLeft.Rotate(Vector3.right, spin, Space.Self);
        if (frontRight) frontRight.Rotate(Vector3.right, spin, Space.Self);
        if (rearLeft) rearLeft.Rotate(Vector3.right, spin, Space.Self);
        if (rearRight) rearRight.Rotate(Vector3.right, spin, Space.Self);
    }
}
using UnityEngine;

public class AudienceJump : MonoBehaviour
{
    public float jumpHeight = 0.25f;
    public float jumpSpeed = 3f;

    private Vector3 startPos;
    private float randomOffset;

    void Start()
    {
        startPos = transform.localPosition;
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float y = Mathf.Abs(Mathf.Sin(Time.time * jumpSpeed + randomOffset));
        transform.localPosition = startPos + Vector3.up * y * jumpHeight;
    }
}
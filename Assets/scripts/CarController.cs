using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 120f;
    public float maxSpeed = 140f;
    public float reverseSpeed = 15f;
    public float brakeForce = 80f;
    public float turnSpeed = 140f;
    public float downforceStrength = 450f;

    [HideInInspector]
    public bool canDrive = false;

    [Header("Wheels")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    public float maxSteerAngle = 30f;
    public float wheelSpinSpeed = 500f;

    private float currentWheelRotation = 0f;

    [Header("Visual Effects")]
    public Camera mainCamera;
    public float minFOV = 60f;
    public float maxFOV = 90f;

    [Header("Audio")]
    private AudioSource engineLoopAudio;

    public float minPitch = 0.7f;
    public float maxPitch = 2.0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        engineLoopAudio = GetComponents<AudioSource>()[0];

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (rb != null)
        {
            rb.mass = 798f;
            rb.linearDamping = 0.02f;
            rb.angularDamping = 10f;
            rb.useGravity = true;
            rb.centerOfMass = new Vector3(0, -0.5f, 0);
        }

        canDrive = false;
    }

    public void StartRace()
    {
        canDrive = true;

        if (engineLoopAudio != null)
            engineLoopAudio.Play();
    }

    void FixedUpdate()
    {
        if (!canDrive || rb == null)
            return;

        bool outOfFuel = FuelManager.Instance != null &&
                         FuelManager.Instance.currentFuel <= 0f;

        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        Vector3 forwardDir = -transform.forward;

        float currentSpeed = rb.linearVelocity.magnitude;

        // Consume fuel based on speed
        if (FuelManager.Instance != null && FuelManager.Instance.currentFuel > 0f)
        {
            FuelManager.Instance.currentFuel -=
                currentSpeed * FuelManager.Instance.fuelBurnRate * Time.fixedDeltaTime;

            FuelManager.Instance.currentFuel =
                Mathf.Clamp(
                    FuelManager.Instance.currentFuel,
                    0f,
                    FuelManager.Instance.maxFuel
                );
        }
        float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);

        // Downforce
        rb.AddForce(
            -transform.up *
            currentSpeed *
            downforceStrength,
            ForceMode.Force
        );

        // Reduce sideways sliding
        Vector3 localVelocity =
            transform.InverseTransformDirection(
                rb.linearVelocity
            );

        localVelocity.x *= 0.55f;

        rb.linearVelocity =
            transform.TransformDirection(
                localVelocity
            );

        // Coasting behaviour
        if (Mathf.Abs(move) > 0.05f)
        {
            rb.linearDamping = 0.02f;
        }
        else
        {
            rb.linearDamping = 0.15f;
        }

        // Forward throttle
        if (!outOfFuel && move > 0)
        {
            rb.AddForce(
                forwardDir *
                move *
                acceleration,
                ForceMode.Acceleration
            );
        }
        // Brake / Reverse
        else if (!outOfFuel && move < 0)
        {
            float forwardVelocity =
                Vector3.Dot(
                    rb.linearVelocity,
                    forwardDir
                );

            if (forwardVelocity > 1f)
            {
                rb.AddForce(
                    -forwardDir *
                    Mathf.Abs(move) *
                    brakeForce,
                    ForceMode.Acceleration
                );
            }
            else
            {
                rb.AddForce(
                    forwardDir *
                    move *
                    reverseSpeed,
                    ForceMode.Acceleration
                );
            }
        }

        // Speed limiter
        Vector3 horizontalVel =
            new Vector3(
                rb.linearVelocity.x,
                0,
                rb.linearVelocity.z
            );

        if (horizontalVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel =
                horizontalVel.normalized *
                maxSpeed;

            rb.linearVelocity =
                new Vector3(
                    limitedVel.x,
                    rb.linearVelocity.y,
                    limitedVel.z
                );
        }

        // Steering
        if (currentSpeed > 0.5f &&
            Mathf.Abs(move) > 0.1f)
        {
            float direction =
                Vector3.Dot(
                    rb.linearVelocity,
                    forwardDir
                ) >= 0 ? 1f : -1f;

            float steeringMultiplier =
                Mathf.Lerp(
                    1f,
                    0.75f,
                    speedFactor
                );

            float rotationAmount =
                turn *
                turnSpeed *
                steeringMultiplier *
                direction *
                Time.fixedDeltaTime;

            Quaternion turnRotation =
                Quaternion.Euler(
                    0f,
                    rotationAmount,
                    0f
                );

            rb.MoveRotation(
                rb.rotation *
                turnRotation
            );
        }
    }

    void Update()
    {
        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        AnimateWheels(move, turn);

        if (mainCamera != null && rb != null)
        {
            float speedFactor =
                Mathf.Clamp01(
                    rb.linearVelocity.magnitude /
                    maxSpeed
                );

            mainCamera.fieldOfView =
                Mathf.Lerp(
                    minFOV,
                    maxFOV,
                    speedFactor
                );
        }

        if (engineLoopAudio != null && rb != null)
        {
            if (Time.timeScale == 0f)
            {
                if (engineLoopAudio.isPlaying)
                    engineLoopAudio.Pause();
            }
            else
            {
                if (!engineLoopAudio.isPlaying && canDrive)
                    engineLoopAudio.UnPause();

                if (!canDrive)
                {
                    engineLoopAudio.pitch = minPitch;
                }
                else
                {
                    float speedFactor =
                        Mathf.Clamp01(
                            rb.linearVelocity.magnitude /
                            maxSpeed
                        );

                    engineLoopAudio.pitch =
                        Mathf.Lerp(
                            minPitch,
                            maxPitch,
                            speedFactor
                        );
                }
            }
        }
    }

    void AnimateWheels(float move, float turn)
    {
        float steerAngle = turn * maxSteerAngle;

        float speed = rb != null ? rb.linearVelocity.magnitude : 0f;
        float dir = 1f;

        if (rb != null)
        {
            dir = Vector3.Dot(rb.linearVelocity, -transform.forward) >= 0 ? 1f : -1f;
        }

        // Physically accurate wheel rotation: degrees per second based on speed
        float wheelRadius = 0.33f;
        float circumference = 2f * Mathf.PI * wheelRadius;
        float baseSpinDegPerSec = (speed / circumference) * 360f;

        // Apply the user's wheelSpinSpeed as a multiplier (500 is default, so dividing by 500 normalizes it to 1)
        float spinDegPerSec = baseSpinDegPerSec * (wheelSpinSpeed / 500f);

        currentWheelRotation += spinDegPerSec * dir * Time.deltaTime;
        currentWheelRotation %= 360f;
        if (frontLeftWheel != null)
            frontLeftWheel.localRotation = Quaternion.Euler(-currentWheelRotation, steerAngle, 0);

        if (frontRightWheel != null)
            frontRightWheel.localRotation = Quaternion.Euler(-currentWheelRotation, steerAngle, 0);

        if (rearLeftWheel != null)
            rearLeftWheel.localRotation = Quaternion.Euler(-currentWheelRotation, 0, 0);

        if (rearRightWheel != null)
            rearRightWheel.localRotation = Quaternion.Euler(-currentWheelRotation, 0, 0);
    }
}
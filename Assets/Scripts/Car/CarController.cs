using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CarInput))]
public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform centerOfMass;

    [Header("Rollover Assist")]
    [SerializeField] private float maxTiltAngle = 35f;
    [SerializeField] private float rolloverStrength = 5000f;
    [SerializeField] private float rolloverDamping = 500f;

    [Header("Transmission")]
    [SerializeField] private int currentGear = 1;
    [SerializeField] private int maxGear = 5;
    [SerializeField] private float reverseRatio = -.50f;
    [SerializeField] private float[] gearRatios =
    {
        0f, // N
        1f, // 1st
        0.75f, // 2nd
        0.55f,  // 3rd
        0.40f,  // 4th
        0.30f   // 5th
    };
    [SerializeField] private float[] gearMinSpeeds =
    { 
        0f,   // Neutral
        0f,   // 1st
        15f,  // 2nd
        30f,  // 3rd
        50f,  // 4th
        70f   // 5th
    };
    [SerializeField] private float maxGearSpeed = 120f;
    public int CurrentGear => currentGear;

    private Rigidbody rb;
    private CarInput carInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        carInput = GetComponent<CarInput>();

        if (centerOfMass != null)
        {
            rb.centerOfMass =
                transform.InverseTransformPoint(centerOfMass.position);
        }
    }

    private void FixedUpdate()
    {
        CheckRollover();
    }

    private void Update()
    {
        UpdateTransmission();
    }

    private void CheckRollover()
    {
        float tiltAngle =
            Vector3.Angle(transform.up, Vector3.up);

        if (tiltAngle <= maxTiltAngle)
            return;

        Vector3 correctionAxis =
            Vector3.Cross(transform.up, Vector3.up);

        float tiltAmount =
            tiltAngle - maxTiltAngle;

        Vector3 correctiveTorque =
            correctionAxis.normalized *
            tiltAmount *
            rolloverStrength;

        Vector3 angularVelocity =
            rb.angularVelocity;

        Vector3 dampingTorque =
            -angularVelocity *
            rolloverDamping;

        rb.AddTorque(
            correctiveTorque + dampingTorque,
            ForceMode.Force
        );
    }

    private void UpdateTransmission()
    {
        float speed =
            rb.linearVelocity.magnitude;

        bool isStopped =
            speed < 1f;
        
        if (carInput.UpShift)
        {
            currentGear++;

            if (currentGear > maxGear)
                currentGear = maxGear;
        }

        if (carInput.DownShift)
        {
            if (currentGear == 0 && !isStopped)
            return;

            currentGear--;

            if (currentGear < -1)
                currentGear = -1;
        }
    }

    public float GetGearRatio()
    {
        if (currentGear == -1)
            return reverseRatio;
        
        if (currentGear == 0)
            return 0f;
        
        return gearRatios[currentGear];
    }

    public float GetGearPowerMulitplier()
    {
        if (currentGear <= 0)
            return 0f;

        float speedKmh =
            rb.linearVelocity.magnitude * 3.6f;
        
        float minSped =
            gearMinSpeeds[currentGear];

        float minSpeed =
            gearMinSpeeds[currentGear];

        float maxSpeed;

        if (currentGear == maxGear)
        {
            maxSpeed = maxGearSpeed;
        }
        else
        {
            maxSpeed =
                gearMinSpeeds[currentGear + 1];
        }

        if (speedKmh < minSped)
            return 0.5f;
        
        if (speedKmh >= maxSpeed)
            return 0.5f;
        
        return 1f;
    }

    public float GetCurrentGearMinSpeed()
    {
        if (currentGear <= 0)
            return 0f;

        return gearMinSpeeds[currentGear];
    }

    public float GetCurrentGearMaxSpeed()
    {
        if (currentGear <= 0)
            return 0f;

        if (currentGear == maxGear)
            return maxGearSpeed;

        return gearMinSpeeds[currentGear + 1];
    }

    private void OnDrawGizmos()
    {
        // Vehicle's current up direction
        Gizmos.color = Color.green;

        Gizmos.DrawLine(
            transform.position,
            transform.position + transform.up * 2f
        );

        // World up direction
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.up * 2f
        );
    }
}
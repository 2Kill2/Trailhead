using UnityEngine;

/// <summary>
/// Represents one wheel's suspension and tire contact point.
/// </summary>
public class Wheel : MonoBehaviour
{
    private CarInput carInput;
    private Rigidbody carRigidbody;

    [Header("Wheel Settings")]
    [SerializeField] private float wheelRadius = 0.5f;

    [Header("Suspension Settings")]
    [SerializeField] private float suspensionDistance = 0.65f;
    [SerializeField] private float suspensionSpring = 1000f;
    [SerializeField] private float suspensionDamper = 0f;

    [Header("Steering Settings")]
    [SerializeField] private bool isSteeringWheel;
    [SerializeField] private float maxSteeringAngle = 30f;

    [Header("Drive Settings")]
    [SerializeField] private float driveForce = 8000f;

    public bool IsGrounded { get; private set; }
    public RaycastHit GroundHit { get; private set; }

    public float SuspensionCompression { get; private set; }

    private float previousCompression;

    private void Awake()
    {
        carRigidbody = GetComponentInParent<Rigidbody>();
        carInput = GetComponentInParent<CarInput>();

        Debug.Log(
            gameObject.name +
            " | Rigidbody: " +
            (carRigidbody != null ? "FOUND" : "NULL") +
            " | CarInput: " +
            (carInput != null ? "FOUND" : "NULL")
        );
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        
        ApplySteering();

        if (IsGrounded)
        {
            ApplyDriveForce();
            //ApplySteeringForce();
            //CalculateSuspension();
            //ApplySuspension();
        }
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray (transform.position, -transform.up);

        float raylength = wheelRadius + suspensionDistance;

        IsGrounded = Physics.Raycast(
            ray,
            out RaycastHit hit,
            raylength
        );
        
        if(IsGrounded)
        {
            GroundHit = hit;

           // Debug.Log(
            //gameObject.name +
            //" | Hit: " + hit.collider.gameObject.name +
            //" | Distance: " + hit.distance.ToString("F3") +
            //" | Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer)
            //);
        }
    }

        

    private void CalculateSuspension()
    {
        float rideHeight = 0.50f; // Desired ride height from the wheel's center to the ground

        float currentHeight = GroundHit.distance;

        SuspensionCompression = (rideHeight - currentHeight) / suspensionDistance;

        SuspensionCompression = Mathf.Clamp01(SuspensionCompression);

        Debug.Log(
            gameObject.name +
            " | Ground: " + GroundHit.distance.ToString("F3") +
            " | Compression: " + SuspensionCompression.ToString("F3") +
            " | Spring Force: " +
            (SuspensionCompression * suspensionSpring).ToString("F1")
            );
    }

    private void ApplySuspension()
    {
        float springForce = suspensionSpring * SuspensionCompression;

        carRigidbody.AddForceAtPosition(
            transform.up * springForce,
            transform.position,
            ForceMode.Force
        );
    }

    private void ApplyDriveForce()
    {
        
        float throttleInput = carInput.Throttle;

        Vector3 driveDirection = transform.forward;

        carRigidbody.AddForceAtPosition(
            driveDirection *
            throttleInput *
            driveForce, transform.position,
            ForceMode.Force
        );

        Debug.DrawRay(
            transform.position,
            driveDirection * throttleInput * driveForce * 0.001f,
            Color.blue
        );
    }

    private void ApplySteering()
    {
        if (!isSteeringWheel) return;

        float steeringInput = carInput.Steering;

        float steeringAngle = steeringInput * maxSteeringAngle;

        transform.localRotation = Quaternion.Euler(0f, steeringAngle, 0f);

        Debug.Log(
    gameObject.name +
    " | Steering: " + steeringInput +
    " | Angle: " + steeringAngle + 
    " | Forward: " + transform.forward.ToString("F3")
    );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded
            ? Color.green
            : Color.red;

        Gizmos.DrawLine(
            transform.position,
            transform.position -
            transform.up *
            suspensionDistance
        );
    }
}
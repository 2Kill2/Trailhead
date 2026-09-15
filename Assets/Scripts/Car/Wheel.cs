using UnityEngine;

/// <summary>
/// Represents one wheel's suspension and tire contact point.
/// </summary>
public class Wheel : MonoBehaviour
{
    private CarInput carInput;
    private Rigidbody carRigidbody;

    [Header("Wheel Settings")]
    [SerializeField] private float wheelRadius = 0.50f;
    [SerializeField] private Transform VisualWheel;

    private Vector3 visualWheelStartPosition;

    [Header("Suspension Settings")]
    [SerializeField] private float suspensionDistance = 0.35f;
    [SerializeField] private float suspensionSpring = 3000f;
    [SerializeField] private float suspensionDamper = 500f;

    [Header("Steering Settings")]
    [SerializeField] private bool isSteeringWheel;
    [SerializeField] private float maxSteeringAngle = 30f;

    [Header("Drive Settings")]
    [SerializeField] private float driveForce = 8000f;
    [SerializeField] private float brakeForce = 12000f;

    public bool IsGrounded { get; private set; }
    public RaycastHit GroundHit { get; private set; }

    public float SuspensionCompression { get; private set; }

    private float previousCompression;

    private void Awake()
    {
        carRigidbody = GetComponentInParent<Rigidbody>();
        carInput = GetComponentInParent<CarInput>();
        visualWheelStartPosition = VisualWheel.localPosition;

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
            CalculateSuspension();
            ApplySuspension();

            UpdateWheelVisual();

            ApplyDriveForce();
            ApplyBrake();
        }
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position, -transform.up);

        float rayLength = 0.35f + wheelRadius + suspensionDistance;

        IsGrounded = Physics.Raycast(
            ray,
            out RaycastHit hit,
            rayLength
        );

       Debug.Log(
            gameObject.name +
            " | Grounded: " + IsGrounded +
            " | Ray Distance: " + rayLength.ToString("F2") +
            " | Hit Distance: " + 
            (IsGrounded ? hit.distance.ToString("F3") : "NONE")
        );

        Debug.DrawRay(
            transform.position,
            -transform.up * rayLength,
            IsGrounded ? Color.green : Color.red
        );

        if (IsGrounded)
        {
         GroundHit = hit;
        }
    }

        

    private void CalculateSuspension()
    {
        float restDistance =
            0.35f + wheelRadius;

        float suspensionOffset =
            restDistance - GroundHit.distance;

        float compression =
            suspensionOffset / suspensionDistance;

        Debug.Log(
            gameObject.name +
            " | Ground: " + GroundHit.distance.ToString("F3") +
            " | Rest: " + restDistance.ToString("F3") +
            " | Offset: " + suspensionOffset.ToString("F3") +
            " | Raw Compression: " + compression.ToString("F3")
        );

        SuspensionCompression = Mathf.Clamp01(compression);
    }

    private void ApplySuspension()
    {
        float restDistance = 0.35f + wheelRadius;

        float suspensionOffset =
            restDistance - GroundHit.distance;

        float springForce =
            suspensionOffset * suspensionSpring;

        Vector3 wheelVelocity =
            carRigidbody.GetPointVelocity(transform.position);

        float suspensionVelocity =
            Vector3.Dot(transform.up, wheelVelocity);

        float damperForce =
            suspensionVelocity * suspensionDamper;

        float totalForce =
            springForce - damperForce;

        carRigidbody.AddForceAtPosition(
            transform.up * totalForce,
            transform.position,
            ForceMode.Force
        );
    }

    private void ApplyDriveForce()
    {
        
        float throttleInput = carInput.Throttle;

        Vector3 driveDirection = transform.forward;

        Debug.Log(
            gameObject.name +
            " | Throttle: " + throttleInput.ToString("F2") +
            " | Grounded: " + IsGrounded
        );

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

        //Debug.Log(
        //    gameObject.name +
        //    " | Steering: " + steeringInput +
        //    " | Angle: " + steeringAngle + 
        //    " | Forward: " + transform.forward.ToString("F3")
        //    );
    }

    private void ApplyBrake()
    {
       // Debug.Log(
        //    gameObject.name +
        //    " | Brake: " + carInput.Brake
      //  );

        float brakeInput = carInput.Brake;

        if (brakeInput <= 0f) return;

        Vector3 forwardVelocity = Vector3.Project(carRigidbody.linearVelocity, transform.forward);

        if (forwardVelocity.sqrMagnitude < 0.01f) return;

        Vector3 brakeDirection = -forwardVelocity.normalized;

        carRigidbody.AddForceAtPosition(
            brakeDirection *
            brakeInput *
            brakeForce, transform.position,
            ForceMode.Force
        );
    }

    private void UpdateWheelVisual()
    {
        if (VisualWheel == null)
            return;

        float restDistance = 0.85f;

        float wheelMovement =
            restDistance - GroundHit.distance;

        VisualWheel.localPosition =
            visualWheelStartPosition +
            new Vector3(0f, wheelMovement, 0f);
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
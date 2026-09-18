using UnityEngine;
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

    [Header("Tire Settings")]
    [SerializeField] private float tireGrip = 3000f;
    [SerializeField] private float longitudinalGrip = 1500f;

    [Header("Wheel Rotation Settings")]
    [SerializeField] private float wheelInertia = 2.0f;
    private float wheelAngularVelocity;

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

            //ApplyDriveForce();
            ApplyBrake();

            ApplyLateralGrip();
            ApplyForwardTraction();
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

     //  Debug.Log(
      //      gameObject.name +
     //       " | Grounded: " + IsGrounded +
      //      " | Ray Distance: " + rayLength.ToString("F2") +
     //       " | Hit Distance: " + 
       //     (IsGrounded ? hit.distance.ToString("F3") : "NONE")
     //   );

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

     //   Debug.Log(
    //        gameObject.name +
     //       " | Ground: " + GroundHit.distance.ToString("F3") +
     //       " | Rest: " + restDistance.ToString("F3") +
     //       " | Offset: " + suspensionOffset.ToString("F3") +
     //       " | Raw Compression: " + compression.ToString("F3")
      //  );

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

      //  Debug.Log(
      //      gameObject.name +
       //     " | Throttle: " + throttleInput.ToString("F2") +
      //      " | Grounded: " + IsGrounded
       // );

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

        float speed =
            carRigidbody.linearVelocity.magnitude;

        float speedFactor =
            Mathf.Clamp01(speed / 27.78f);

        float steeringMultiplier =
            Mathf.Lerp(1f, 0.55f, speedFactor);

        float steeringAngle =
            steeringInput *
            maxSteeringAngle *
            steeringMultiplier;

        transform.localRotation =
            Quaternion.Euler(0f, steeringAngle, 0f);
    }

    private void ApplyBrake()
    {
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

    private void ApplyLateralGrip()
    {
        Vector3 wheelVelocity = carRigidbody.GetPointVelocity(transform.position);

        float lateralVelocity = Vector3.Dot(wheelVelocity, transform.right);

        Vector3 lateralForce = -transform.right * lateralVelocity * tireGrip;

        carRigidbody.AddForceAtPosition(
            lateralForce,
            transform.position,
            ForceMode.Force
        );
    }

    private void ApplyForwardTraction()
    {
        Vector3 wheelVelocity =
            carRigidbody.GetPointVelocity(transform.position);

        float forwardVelocity =
            Vector3.Dot(wheelVelocity, transform.forward);

        float targetWheelSpeed =
            forwardVelocity + 
            carInput.Throttle * 5f;

        wheelAngularVelocity = 
            targetWheelSpeed / wheelRadius;

        float wheelSurfaceVelocity =
            wheelAngularVelocity * wheelRadius;

        float slipVelocity =
            wheelSurfaceVelocity - forwardVelocity;

        float tireForce =
            slipVelocity * longitudinalGrip;

        float maxTireForce =
            carRigidbody.mass *
            Physics.gravity.magnitude *
            0.5f;

        tireForce =
            Mathf.Clamp(
                tireForce,
                -maxTireForce,
                maxTireForce
            );

        Vector3 tireForceVector =
            transform.forward * tireForce;

        carRigidbody.AddForceAtPosition(
            tireForceVector,
            transform.position,
            ForceMode.Force
        );

        Debug.Log(
            gameObject.name +
            " | Throttle: " +
            carInput.Throttle.ToString("F2") +
            " | Ground: " +
            forwardVelocity.ToString("F2") +
            " | Wheel: " +
            wheelSurfaceVelocity.ToString("F2") +
            " | Slip: " +
            slipVelocity.ToString("F2") +
            " | Tire Force: " +
            tireForce.ToString("F1")
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

    private void OnDrawGizmos()
    {
        // Wheel position
        Vector3 origin = transform.position;

        // Forward direction
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(
            origin,
            origin + transform.forward * 1.0f
        );

        // Right direction
        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            origin,
            origin + transform.right * 0.5f
        );

        // Up / suspension direction
        Gizmos.color = Color.green;

        Gizmos.DrawLine(
            origin,
            origin + transform.up * 0.75f
        );
    }
}
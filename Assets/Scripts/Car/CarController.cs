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
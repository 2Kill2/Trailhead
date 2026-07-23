using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Applies physics forces to move the vehicle.
/// This class never reads player input directly.
/// </summary>

public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform centerOfMass;

    [Header("Movement")]
    [SerializeField] private float accelerationForce = 8000f;
    [SerializeField] private float maxSpeed = 30f;

    private Rigidbody rb;
    private CarInput carInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        carInput = GetComponent<CarInput>();

        if (centerOfMass != null)
        {
            rb.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
        }
    }

    private void FixedUpdate()
    {
        Accelerate();
    }

    private void Accelerate()
    {
        //dont accel forever
        if (rb.linearVelocity.magnitude >= maxSpeed)
        return;

        rb.AddForce(transform.forward * carInput.Throttle * accelerationForce, ForceMode.Force);
    }
}

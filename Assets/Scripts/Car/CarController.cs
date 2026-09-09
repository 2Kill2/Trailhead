using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CarInput))]
public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform centerOfMass;

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
}
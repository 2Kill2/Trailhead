using UnityEngine;

/// <summary>
/// Represents a single wheel contact point.
/// Handles ground detection only.
/// </summary>

public class Wheel : MonoBehaviour
{
    [Header("Wheel Settings")]
    [SerializeField] private float wheelRadius = 0.35f;
    [SerializeField] private float suspensionDistance = 0.5f;

    public bool isGrounded {get; private set;}

    public RaycastHit GroundHit {get; private set;}

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        Ray ray = new Ray(transform.position, -transform.up);

        isGrounded = Physics.Raycast(ray, out RaycastHit hit, wheelRadius + suspensionDistance);

        if (isGrounded)
        {
            GroundHit = hit;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;

        Gizmos.DrawLine(transform.position, transform.position - transform.up * (wheelRadius + suspensionDistance));
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads player input and exposes normalized values for the vehicle.
/// This class never applies physics.
/// </summary>

public class CarInput : MonoBehaviour
{
    private TrailHeadControls controls;

    public float Steering {get; private set;}
    public float Throttle {get; private set;}
    public float Brake {get; private set;}

    public bool Handbrake {get; private set;}

    private void Awake()
    {
        controls = new TrailHeadControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        Steering = controls.Driving.Steer.ReadValue<float>();
        Throttle = controls.Driving.Throttle.ReadValue<float>();
        Brake = controls.Driving.Brake.ReadValue<float>();

        Handbrake = controls.Driving.HandBrake.IsPressed();

        //Debug.Log($"Steer: {Steering:F2}" + $"Throttle: {Throttle:F2} " + $"Brake: {Brake:F2}");
    }
}

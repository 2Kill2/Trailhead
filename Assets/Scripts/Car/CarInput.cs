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
    public float Throttle
    {
        get
        {
            return controls.Driving.Throttle.ReadValue<float>();
        }
    }
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
        //Debug.Log(
          //  "Steering: " + controls.Driving.Steer.ReadValue<float>().ToString("F3") +
            //" | Throttle: " + controls.Driving.Throttle.ReadValue<float>().ToString("F3") +
            //" | Brake: " + controls.Driving.Brake.ReadValue<float>().ToString("F3") +
            //" | Handbrake: " + controls.Driving.HandBrake.ReadValue<float>().ToString("F3")
        //);

        if (controls == null) return;

        Debug.Log("Driving: " + controls.Driving != null ? "FOUND" : "NULL");

        Steering = controls.Driving.Steer.ReadValue<float>();
        Brake = controls.Driving.Brake.ReadValue<float>();
        Handbrake = controls.Driving.HandBrake.IsPressed();
    }
}
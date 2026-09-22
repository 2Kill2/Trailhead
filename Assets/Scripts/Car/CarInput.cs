using UnityEngine;
using UnityEngine.InputSystem;

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

    public bool UpShift {get; private set;}
    public bool DownShift {get; private set;}

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
        if (controls == null) return;

        Steering = controls.Driving.Steer.ReadValue<float>();
        Brake = controls.Driving.Brake.ReadValue<float>();
        Handbrake = controls.Driving.HandBrake.IsPressed();

        UpShift = controls.Driving.UpShift.WasPressedThisFrame();
        DownShift = controls.Driving.DownShift.WasPressedThisFrame();

        if (DownShift)
        {
            Debug.Log("Downshift PRESSED");
        }

        if (UpShift)
        {
            Debug.Log("UpShift PRESSED");
        }
    }
}
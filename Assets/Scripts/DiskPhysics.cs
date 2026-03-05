using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class VRFrisbeePhysics : MonoBehaviour
{
    [Header("Aerodynamics")]
    public float liftPower = 0.1f;         // Lower this if it "launches" too high
    public float dragCoefficient = 0.15f;  // Air resistance to slow it down
    public float spinStabilization = 1.5f; // How hard the disk tries to stay flat

    [Header("Throw Settings")]
    public float manualSpinPower = 20f;    // The "flick" added on release

    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Ensure physics settings are correct for VR
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.mass = 0.1f; // Standard Frisbee weight in kg
    }

    void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    // This runs when you let go of the trigger in VR
    void OnRelease(SelectExitEventArgs args)
    {
        // Add a horizontal spin (Torque) to stabilize flight
        // This simulates the finger flick that VR controllers often miss
        rb.AddRelativeTorque(Vector3.up * manualSpinPower, ForceMode.Impulse);

        // Optional: Add a slight curve based on wrist tilt (Z-axis roll)
        float tilt = transform.localEulerAngles.z;
        if (tilt > 180) tilt -= 360;
        rb.AddRelativeTorque(Vector3.forward * (-tilt * 0.05f), ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (grabInteractable.isSelected) return;

        Vector3 velocity = rb.linearVelocity;
        float speed = velocity.magnitude;

        // Only apply flight physics if moving fast enough
        if (speed > 1.0f)
        {
            // 1. Better Angle of Attack
            // We compare the disk's "Forward" direction to the way it's moving
            float dot = Vector3.Dot(transform.up, velocity.normalized);
            float angleOfAttack = Mathf.Asin(dot) * Mathf.Rad2Deg;

            // 2. Simplified Lift (More generous for long flights)
            // Lift increases with speed squared, but we use a lower drag to keep momentum
            Vector3 liftDirection = Vector3.Cross(velocity, transform.right).normalized;
            float liftForce = speed * speed * liftPower;
            rb.AddForce(liftDirection * liftForce);

            // 3. Low Drag (The key to long distance)
            // We use a very small drag so the disk doesn't lose speed immediately
            rb.AddForce(-velocity * (speed * dragCoefficient));

            // 4. Gyroscopic Stability (Crucial)
            // If the disk tips, it falls. This keeps it flat so it glides.
            Quaternion targetRot = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * spinStabilization));
        }
    }
}
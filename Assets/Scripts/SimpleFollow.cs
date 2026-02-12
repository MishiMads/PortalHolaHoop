using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    [Header("Setup")]
    public Transform cameraTransform; // Drag 'Main Camera' here
    public float verticalOffset = -0.5f; // Adjust to put belt at waist level

    [Header("Settings")]
    public float followSpeed = 10f; // Higher = snappier, Lower = smoother

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // 1. Get the camera's position with a downward offset
        Vector3 targetPosition = cameraTransform.position + new Vector3(0, verticalOffset, 0);

        // 2. Get the camera's rotation but strip out X and Z tilting
        // This ensures the belt stays level with the floor
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0; // Flatten the vector
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

        // 3. Smoothly move and rotate the belt
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
    }
}
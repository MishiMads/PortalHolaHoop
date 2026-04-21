using UnityEngine;
using System.Collections;

public class PortalTeleporter : MonoBehaviour
{
    [Header("Settings")]
    public Transform playerTransform;
    public float transitionDuration = 0.5f;
    public float minDistanceToTeleport = 1.5f; // Distance in meters
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private GameObject activePortal;

    // Called when the frisbee lands and spawns a portal
    public void SetPortalLocation(GameObject portal)
    {
        if (activePortal != null)
        {
            Debug.Log("[PortalTeleporter] Replacing existing portal with new one.");
            Destroy(activePortal);
        }

        activePortal = portal;
        Debug.Log($"[PortalTeleporter] New portal registered at {portal.transform.position}");
    }

    // Triggered by your Palm-Fist-Palm Sequence
    public void OnGestureComplete()
    {
        if (activePortal == null)
        {
            Debug.Log("[PortalTeleporter] Gesture triggered but no active portal exists.");
            return;
        }

        float distance = Vector3.Distance(playerTransform.position, activePortal.transform.position);
        Debug.Log($"[PortalTeleporter] Distance to portal: {distance:F2}m (min required: {minDistanceToTeleport}m)");

        if (distance >= minDistanceToTeleport)
        {
            Debug.Log("[PortalTeleporter] Starting teleport...");
            StartCoroutine(SmoothTeleport(activePortal.transform.position));
        }
        else
        {
            Debug.Log($"[PortalTeleporter] Too close to portal! Move {minDistanceToTeleport - distance:F2}m further away.");
        }
    }

    IEnumerator SmoothTeleport(Vector3 targetPosition)
    {
        Vector3 startPosition = playerTransform.position;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / transitionDuration;
            float curvedPercent = easeCurve.Evaluate(percent);

            playerTransform.position = Vector3.Lerp(startPosition, targetPosition, curvedPercent);
            yield return null;
        }

        playerTransform.position = targetPosition;
        Debug.Log($"[PortalTeleporter] Teleport complete. Player now at {targetPosition}");

        Destroy(activePortal);
        activePortal = null;
    }
}
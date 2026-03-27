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
        activePortal = portal;
    }

    // Triggered by your Palm-Fist-Palm Sequence
    public void OnGestureComplete()
    {
        // 1. Check if a portal even exists
        if (activePortal == null) return;

        // 2. Calculate distance between player and portal
        float distance = Vector3.Distance(playerTransform.position, activePortal.transform.position);

        // 3. Only teleport if the player is far enough away
        if (distance >= minDistanceToTeleport)
        {
            StartCoroutine(SmoothTeleport(activePortal.transform.position));
        }
        else
        {
            Debug.Log("Too close to portal to teleport!");
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

        // Clean up portal after successful move
        Destroy(activePortal);
        activePortal = null;
    }
}
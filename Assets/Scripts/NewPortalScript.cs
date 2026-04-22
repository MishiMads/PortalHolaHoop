using UnityEngine;
using System.Collections;
using System;

public class NewPortalScript : MonoBehaviour
{
    public static event Action OnTeleportComplete;
    [Header("Portal Settings")]
    public GameObject portalPrefab;
    public LayerMask groundLayer;

    [Header("Teleport Settings")]
    public Transform playerTransform;
    public float transitionDuration = 0.5f;
    public float minDistanceToTeleport = 1.5f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Height added above ground after teleporting. Leave 0 for OVRCameraRig (it manages its own height).")]
    public float playerHeightOffset = 0f;
    [Tooltip("Raycast distance downward to snap to the ground surface at the destination.")]
    public float groundCheckDistance = 5f;
    [Tooltip("Assign CenterEyeAnchor (or the camera transform) so the rig offset is accounted for during teleport.")]
    public Transform cameraTransform;

    private GameObject _activePortal;
    private bool _isTeleporting = false;

    // Automatically spawns a portal when the frisbee hits the ground
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Vector3 landingPoint = collision.contacts[0].point;
            Debug.Log($"[NewPortalScript] Frisbee landed at {landingPoint}");
            SpawnPortal(landingPoint);
        }
    }

    private void SpawnPortal(Vector3 position)
    {
        if (_activePortal != null)
        {
            Debug.Log("[NewPortalScript] Replacing existing portal.");
            Destroy(_activePortal);
        }

        if (portalPrefab != null)
        {
            _activePortal = Instantiate(portalPrefab, position + Vector3.up * 0.05f, Quaternion.identity);
            Debug.Log($"[NewPortalScript] Portal spawned at {_activePortal.transform.position}");
        }
        else
        {
            // No prefab assigned — use an invisible marker to track the location
            _activePortal = new GameObject("PortalMarker");
            _activePortal.transform.position = position + Vector3.up * 0.05f;
            Debug.Log($"[NewPortalScript] No prefab assigned. Invisible portal marker placed at {_activePortal.transform.position}");
        }
    }

    // Call this method from your OK gesture event in the Inspector
    public void TeleportToPortal()
    {
        if (_isTeleporting) { Debug.Log("[NewPortalScript] Already teleporting."); return; }
        if (_activePortal == null) { Debug.Log("[NewPortalScript] No active portal to teleport to."); return; }
        if (playerTransform == null) { Debug.LogWarning("[NewPortalScript] Player Transform is not assigned!"); return; }

        float distance = Vector3.Distance(playerTransform.position, _activePortal.transform.position);
        Debug.Log($"[NewPortalScript] Distance to portal: {distance:F2}m (min required: {minDistanceToTeleport}m)");

        if (distance >= minDistanceToTeleport)
        {
            // Raycast downward from above the portal to find the exact ground surface
            Vector3 portalPos = _activePortal.transform.position;
            Vector3 rayOrigin = portalPos + Vector3.up * 2f;
            Vector3 safeTarget = portalPos;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundCheckDistance + 2f, groundLayer))
            {
                safeTarget = hit.point + Vector3.up * playerHeightOffset;
                Debug.Log($"[NewPortalScript] Ground found at {hit.point}, teleporting to {safeTarget}");
            }
            else
            {
                Debug.LogWarning("[NewPortalScript] No ground found below portal — using raw portal position.");
            }

            // Account for OVRCameraRig tracking offset so the rig moves
            // to where the player's feet actually are, not where the headset is
            if (cameraTransform != null)
            {
                Vector3 headOffset = cameraTransform.position - playerTransform.position;
                headOffset.y = 0f; // only correct horizontal drift
                safeTarget -= headOffset;
                Debug.Log($"[NewPortalScript] Applied camera rig offset: {headOffset}");
            }

            Debug.Log("[NewPortalScript] Starting teleport...");
            StartCoroutine(SmoothTeleport(safeTarget));
        }
        else
        {
            Debug.Log($"[NewPortalScript] Too close! Move {minDistanceToTeleport - distance:F2}m further away.");
        }
    }

    private IEnumerator SmoothTeleport(Vector3 targetPosition)
    {
        _isTeleporting = true;
        Vector3 startPosition = playerTransform.position;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float curvedPercent = easeCurve.Evaluate(elapsed / transitionDuration);
            playerTransform.position = Vector3.Lerp(startPosition, targetPosition, curvedPercent);
            yield return null;
        }

        playerTransform.position = targetPosition;
        Debug.Log($"[NewPortalScript] Teleport complete. Player now at {targetPosition}");
        OnTeleportComplete?.Invoke();

        Destroy(_activePortal);
        _activePortal = null;
        _isTeleporting = false;
    }
}


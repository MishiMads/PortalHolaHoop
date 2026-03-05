using UnityEngine;
using System.Collections;
using Oculus.Interaction;

public class PortalTeleporter : MonoBehaviour
{
    [Header("Settings")]
    public float dashDuration = 0.5f;
    public string portalTag = "ActivePortal";
    public Transform playerRig;
    public float minDistanceToTeleport = 1.5f; // Won't teleport if closer than this

    [Header("Gesture Settings")]
    public ActiveStateGroup fistActiveState;

    private bool _isCharging = false;
    private bool _isTeleporting = false;

    void Update()
    {
        GameObject portal = GameObject.FindWithTag(portalTag);
        if (portal == null) return;

        // Calculate distance between your hand and the frisbee
        float dist = Vector3.Distance(transform.position, portal.transform.position);

        // 1. Detect the "Grab": Only charge if we are FAR from the frisbee
        if (fistActiveState != null && fistActiveState.Active && !_isTeleporting)
        {
            if (dist > minDistanceToTeleport)
            {
                _isCharging = true;
            }
        }

        // 2. Detect the "Release": Fire if we were charged
        if (_isCharging && !fistActiveState.Active && !_isTeleporting)
        {
            _isCharging = false;
            InitiateTeleport(portal.transform.position);
        }

        // Safety: If you walk up to the frisbee while "charged", cancel the charge
        if (_isCharging && dist < (minDistanceToTeleport - 0.5f))
        {
            _isCharging = false;
        }
    }

    public void InitiateTeleport(Vector3 targetPos)
    {
        StartCoroutine(EaseInDash(targetPos));
    }

    private IEnumerator EaseInDash(Vector3 targetPos)
    {
        _isTeleporting = true;
        Vector3 startPos = playerRig.position;
        float elapsed = 0;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / dashDuration;
            float easeIn = percent * percent;
            playerRig.position = Vector3.Lerp(startPos, targetPos, easeIn);
            yield return null;
        }

        playerRig.position = targetPos;
        _isTeleporting = false;
    }
}
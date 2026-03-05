using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject portalPrefab;
    public LayerMask groundLayer;

    private GameObject _currentPortal;

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            // 1. Log the position to the console
            Debug.Log("Frisbee landed at: " + collision.contacts[0].point);

            // 2. Call the spawn function
            SpawnPortal(collision.contacts[0].point);
        }
    }

    private void SpawnPortal(Vector3 spawnPosition)
    {
        // --- COMMENT OUT THIS LOGIC FOR NOW ---

        /*
        if (_currentPortal != null)
        {
            Destroy(_currentPortal);
        }

        _currentPortal = Instantiate(portalPrefab, spawnPosition, Quaternion.identity);
        _currentPortal.transform.position += Vector3.up * 0.05f;
        */

        // -------------------------------------

        Debug.Log("Waiting for portal prefab... but logic passed!");
    }
}
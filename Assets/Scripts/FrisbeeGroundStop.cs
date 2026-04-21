using UnityEngine;
using Oculus.Interaction.Throw;

public class FrisbeeGroundStop : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ThrowTuner throwTuner;
    
    [Header("Ground Physics")]
    [SerializeField] private float groundDampingFactor = 0.95f;
    [SerializeField] private float angularDampingFactor = 0.90f;
    [SerializeField] private float stopThreshold = 0.3f;

    private bool isGrounded = false;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[FrisbeeGroundStop] Hit: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        
        // Detect ground collision - try multiple ways
        if (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.CompareTag("Floor") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
            Debug.Log("[FrisbeeGroundStop] GROUNDED - Switching to ground physics");
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.CompareTag("Floor") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.CompareTag("Floor") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
            Debug.Log("[FrisbeeGroundStop] Left ground");
        }
    }

    void FixedUpdate()
    {
        if (isGrounded && rb != null && !rb.isKinematic)
        {
            // Apply ground physics dampening (similar to BowlingExtendedVelocity)
            Vector3 currentVel = rb.linearVelocity;
            Vector3 dampedVel = currentVel * groundDampingFactor;
            
            Vector3 currentAngVel = rb.angularVelocity;
            Vector3 dampedAngVel = currentAngVel * angularDampingFactor;

            rb.linearVelocity = dampedVel;
            rb.angularVelocity = dampedAngVel;

            // Stop completely if BOTH are very slow
            if (dampedVel.magnitude < stopThreshold && dampedAngVel.magnitude < stopThreshold)
            {
                Debug.Log("[FrisbeeGroundStop] Stopping completely");
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                // Optionally disable the ThrowTuner here if needed
            }
        }
    }
}
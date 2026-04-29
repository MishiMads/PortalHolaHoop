using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class FrisbeeSpawner : MonoBehaviour
{
    // Reference to the active state (from HandPoseInputDevice or ShapeRecognizerActiveState)
    [SerializeField] private MonoBehaviour gestureComponent;

    [Tooltip("Add both your Frisbee and PaperPlane GameObjects here. The one that is active in the scene will be used at runtime.")]
    [SerializeField] private GameObject[] throwableObjects;

    [SerializeField] private float spawnDistance = 1.5f;
    [SerializeField] private float cooldownTime = 1f;

    private IActiveState activeState;
    private bool previousGestureState = false;
    private float lastSpawnTime = -999f;

    /// <summary>Returns the first throwable object that is currently active in the hierarchy.</summary>
    private GameObject ActiveThrowable
    {
        get
        {
            if (throwableObjects == null) return null;
            foreach (var obj in throwableObjects)
                if (obj != null && obj.activeInHierarchy) return obj;
            return null;
        }
    }

    void Start()
    {
        if (gestureComponent == null)
        {
            Debug.LogError("[FrisbeeSpawner] Gesture Component is not assigned in the Inspector!");
            return;
        }

        // Get the IActiveState interface from the component
        activeState = gestureComponent as IActiveState;

        if (activeState == null)
        {
            Debug.LogError($"[FrisbeeSpawner] '{gestureComponent.GetType().Name}' does not implement IActiveState! Make sure you assigned a ShapeRecognizerActiveState, HandPoseActiveState, or similar.");
        }
        else
        {
            Debug.Log($"[FrisbeeSpawner] Successfully linked gesture component: {gestureComponent.GetType().Name}");
        }

        var throwable = ActiveThrowable;
        if (throwable == null)
            Debug.LogWarning("[FrisbeeSpawner] No active throwable object found in ThrowableObjects list. Make sure at least one is active in the scene.");
        else
            Debug.Log($"[FrisbeeSpawner] Active throwable: {throwable.name}");
    }

    void Update()
    {
        if (activeState == null) return;

        bool currentGestureState = activeState.Active;

        // Log when gesture state changes
        if (currentGestureState != previousGestureState)
        {
            Debug.Log($"[FrisbeeSpawner] Gesture state changed: {(currentGestureState ? "ACTIVE" : "INACTIVE")}");
        }

        // Detect gesture activation with cooldown
        if (currentGestureState && !previousGestureState)
        {
            float timeSinceLastSpawn = Time.time - lastSpawnTime;
            if (timeSinceLastSpawn >= cooldownTime)
            {
                SpawnFrisbee();
                lastSpawnTime = Time.time;
            }
            else
            {
                Debug.Log($"[FrisbeeSpawner] Gesture detected but cooldown not ready. {(cooldownTime - timeSinceLastSpawn):F2}s remaining.");
            }
        }

        previousGestureState = currentGestureState;
    }

    private void SpawnFrisbee()
    {
        GameObject frisbee = ActiveThrowable;
        if (frisbee == null)
        {
            Debug.LogError("[FrisbeeSpawner] Cannot reposition throwable — no active object found in ThrowableObjects list!");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError("[FrisbeeSpawner] Camera.main is null! Make sure your camera is tagged as MainCamera.");
            return;
        }

        Transform cameraTransform = Camera.main.transform;
        Vector3 newPosition = cameraTransform.position + cameraTransform.forward * spawnDistance;
        frisbee.transform.position = newPosition;
        frisbee.transform.rotation = cameraTransform.rotation;

        // Freeze physics so the frisbee doesn't carry over any previous momentum
        Rigidbody rb = frisbee.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            Debug.Log("[FrisbeeSpawner] Frisbee physics frozen.");
        }

        Debug.Log($"[FrisbeeSpawner] Frisbee repositioned to {newPosition}");
    }
}
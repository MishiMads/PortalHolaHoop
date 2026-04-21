using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class FrisbeeSpawner : MonoBehaviour
{
    // Reference to the active state (from HandPoseInputDevice or ShapeRecognizerActiveState)
    [SerializeField] private MonoBehaviour gestureComponent;
    [SerializeField] private GameObject frisbee;
    [SerializeField] private float spawnDistance = 1.5f;
    [SerializeField] private float cooldownTime = 1f;
    
    private IActiveState activeState;
    private bool previousGestureState = false;
    private float lastSpawnTime = -999f;
    
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

        if (frisbee == null)
        {
            Debug.LogError("[FrisbeeSpawner] No frisbee GameObject assigned in the Inspector!");
        }
        else
        {
            Debug.Log($"[FrisbeeSpawner] Frisbee assigned: {frisbee.name}");
        }
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
        if (frisbee == null)
        {
            Debug.LogError("[FrisbeeSpawner] Cannot reposition frisbee - reference is null!");
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
        
        Debug.Log($"[FrisbeeSpawner] Frisbee repositioned to {newPosition}");
    }
}
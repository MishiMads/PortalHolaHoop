using UnityEngine;

public class Canvas : MonoBehaviour
{
    [Header("Hover Settings")]
    public float amplitude = 0.5f; // How high it bobs
    public float frequency = 1f;   // How fast it bobs

    private Vector3 startPos;

    void Start()
    {
        // Store the initial local position relative to the parent object
        startPos = transform.localPosition;
    }

    void LateUpdate()
    {
        // 1. Handle the "Look at Camera" logic
        transform.LookAt(transform.position + Camera.main.transform.forward);

        // 2. Handle the "Up and Down" logic
        Vector3 tempPos = startPos;
        // Use a Sine wave to calculate the Y offset
        tempPos.y += Mathf.Sin(Time.time * frequency) * amplitude;

        transform.localPosition = tempPos;
    }
}
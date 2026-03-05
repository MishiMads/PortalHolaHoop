using UnityEngine;
using Oculus.Interaction;
using System.Linq;
using Oculus.Interaction.HandGrab;

public class TrajectoryPredictor : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;
    public Rigidbody frisbeeRigidbody;
    public HandGrabInteractable grabInteractable;

    [Header("Settings")]
    public int resolution = 30;
    public float timeStep = 0.05f;
    public float throwForceMultiplier = 1.5f;
    public LayerMask collisionLayer;

    private bool _isHeld = false;
    private Vector3 _lastHandPos;
    private HandGrabInteractor _currentInteractor;

    public void OnGrab()
    {
        // 1. Find the interactor holding the frisbee
        _currentInteractor = grabInteractable.SelectingInteractors.FirstOrDefault() as HandGrabInteractor;

        if (_currentInteractor != null)
        {
            _isHeld = true;

            // 2. Access the transform of the hand interactor directly
            _lastHandPos = _currentInteractor.transform.position;

            // 3. FIX: Disable physics while held
            frisbeeRigidbody.isKinematic = true;
            frisbeeRigidbody.linearVelocity = Vector3.zero;
            frisbeeRigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void OnRelease()
    {
        if (_currentInteractor != null)
        {
            // FIX: Access the transform of the hand interactor directly
            Vector3 handPos = _currentInteractor.transform.position;
            Vector3 handForward = _currentInteractor.transform.forward;

            Vector3 throwVelocity = handForward * (Vector3.Distance(handPos, _lastHandPos) / Time.deltaTime * throwForceMultiplier);

            frisbeeRigidbody.isKinematic = false;
            frisbeeRigidbody.linearVelocity = throwVelocity;
        }

        _isHeld = false;
        lineRenderer.enabled = false;
        _currentInteractor = null;
    }

    void Update()
    {
        if (_isHeld && _currentInteractor != null)
        {
            // FIX: Access the transform of the hand interactor directly
            Vector3 currentHandPos = _currentInteractor.transform.position;
            Vector3 handForward = _currentInteractor.transform.forward;

            Vector3 handVelocity = (currentHandPos - _lastHandPos) / Time.deltaTime;
            Vector3 throwVelocity = handForward * (handVelocity.magnitude + throwForceMultiplier);

            DrawTrajectory(currentHandPos, throwVelocity);
            _lastHandPos = currentHandPos;
        }
    }

    private void DrawTrajectory(Vector3 startPos, Vector3 velocity)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = resolution;

        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;
            Vector3 point = startPos + (velocity * t) + (0.5f * Physics.gravity * t * t);
            lineRenderer.SetPosition(i, point);

            if (i > 0 && Physics.Linecast(lineRenderer.GetPosition(i - 1), point, out RaycastHit hit, collisionLayer))
            {
                lineRenderer.positionCount = i + 1;
                lineRenderer.SetPosition(i, hit.point);
                break;
            }
        }
    }
}
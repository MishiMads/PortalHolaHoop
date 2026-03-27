using UnityEngine;
using Oculus.Interaction;

public class FrisbeeGestureGuard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gestureHandler;
    [SerializeField] private Grabbable grabbable; // Drag the 'Grabbable' component here

    private void Update()
    {
        if (grabbable == null || gestureHandler == null) return;

        // If the frisbee is currently being grabbed by any pointer/hand
        if (grabbable.SelectingPointsCount > 0)
        {
            if (gestureHandler.activeSelf) gestureHandler.SetActive(false);
        }
        else
        {
            if (!gestureHandler.activeSelf) gestureHandler.SetActive(true);
        }
    }
}
using UnityEngine;
using System;

public class GoalAreaTrigger : MonoBehaviour
{
    public event Action OnGoal;

    [Tooltip("Tag of the object that triggers the goal (e.g. your frisbee)")]
    [SerializeField] private string frisbeeTag = "Frisbee";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(frisbeeTag))
        {
            Debug.Log("[GoalAreaTrigger] GOAL! Frisbee entered the goal area.");
            OnGoal?.Invoke();
        }
    }
}


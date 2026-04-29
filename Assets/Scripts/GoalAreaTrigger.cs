using UnityEngine;
using System;

public class GoalAreaTrigger : MonoBehaviour
{
    /// <summary>Instance event — used if GoalDisplay has a direct reference.</summary>
    public event Action OnGoal;

    /// <summary>Static event — GoalDisplay listens to this automatically, no Inspector wiring needed.</summary>
    public static event Action OnGoalStatic;

    [Tooltip("Tag of the object that triggers the goal (e.g. your frisbee)")]
    [SerializeField] private string frisbeeTag = "Frisbee";

    [Tooltip("Also fire the goal event when the PLAYER enters the area (useful if you teleport into the goal zone).")]
    [SerializeField] private bool triggerOnPlayer = true;
    [SerializeField] private string playerTag = "Player";

    private bool _goalFired = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_goalFired) return; // only fire once per run

        bool isFrisbee = other.CompareTag(frisbeeTag);
        bool isPlayer  = triggerOnPlayer && other.CompareTag(playerTag);

        if (isFrisbee || isPlayer)
        {
            _goalFired = true;
            Debug.Log($"[GoalAreaTrigger] GOAL triggered by: {other.gameObject.name} (tag: {other.tag})");
            OnGoal?.Invoke();
            OnGoalStatic?.Invoke();
        }
    }
}


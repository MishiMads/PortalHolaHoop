using UnityEngine;
using TMPro;
using System.Collections;

public class GoalDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private float displayDuration = 2.5f;

    [Header("Score Panel (shown at goal)")]
    [Tooltip("The parent GameObject that contains goalText and teleportCounter 3D text — hidden until goal is reached.")]
    [SerializeField] private GameObject textHider;
    [Tooltip("The TeleportCounter script that tracks how many teleports the player made.")]
    [SerializeField] private TeleportCounter teleportCounter;
    [Tooltip("The 3D TMP text object that displays the teleport count on the score canvas.")]
    [SerializeField] private TextMeshPro teleportCounterText;
    [SerializeField] private string teleportPrefix = "Teleports: ";

    [Header("Trigger")]
    [SerializeField] private GoalAreaTrigger goalTrigger;

    private Coroutine _hideCoroutine;

    private void Start()
    {
        // Hide everything until the goal is reached
        if (textHider != null)
            textHider.SetActive(false);
        else if (goalText != null)
            goalText.gameObject.SetActive(false);

        // Subscribe to static event (works without Inspector reference)
        GoalAreaTrigger.OnGoalStatic += ShowGoal;

        // Also subscribe to instance event if one is assigned directly
        if (goalTrigger != null)
            goalTrigger.OnGoal += ShowGoal;
        else
            Debug.Log("[GoalDisplay] No GoalAreaTrigger assigned — using static event (auto-connect).");
    }

    private void OnDestroy()
    {
        GoalAreaTrigger.OnGoalStatic -= ShowGoal;

        if (goalTrigger != null)
            goalTrigger.OnGoal -= ShowGoal;
    }

    private bool _goalShown = false;

    private void ShowGoal()
    {
        if (_goalShown) return;
        _goalShown = true;
        // Update teleport counter text before revealing
        if (teleportCounterText != null && teleportCounter != null)
            teleportCounterText.text = teleportPrefix + teleportCounter.GetCount();

        // Reveal the score panel (TextHider + all children)
        if (textHider != null)
            textHider.SetActive(true);
        else if (goalText != null)
            goalText.gameObject.SetActive(true);

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        // Only auto-hide goalText if there's no persistent score panel
        if (textHider == null && goalText != null)
            _hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        if (goalText != null)
            goalText.gameObject.SetActive(false);
    }
}


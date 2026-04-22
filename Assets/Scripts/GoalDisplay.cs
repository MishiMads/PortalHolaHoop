using UnityEngine;
using TMPro;
using System.Collections;

public class GoalDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private float displayDuration = 2.5f;

    [Header("Trigger")]
    [SerializeField] private GoalAreaTrigger goalTrigger;

    private Coroutine _hideCoroutine;

    private void Start()
    {
        if (goalText != null)
            goalText.gameObject.SetActive(false);

        if (goalTrigger != null)
            goalTrigger.OnGoal += ShowGoal;
        else
            Debug.LogWarning("[GoalDisplay] GoalAreaTrigger not assigned!");
    }

    private void OnDestroy()
    {
        if (goalTrigger != null)
            goalTrigger.OnGoal -= ShowGoal;
    }

    private void ShowGoal()
    {
        if (goalText == null) return;


        goalText.gameObject.SetActive(true);

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        _hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        if (goalText != null)
            goalText.gameObject.SetActive(false);
    }
}


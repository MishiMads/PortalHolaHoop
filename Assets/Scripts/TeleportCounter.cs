using UnityEngine;
using TMPro;

public class TeleportCounter : MonoBehaviour
{
    [Header("Frisbee Label (3D world space)")]
    [SerializeField] private TextMeshPro frisbeeText;       // Child TMP on the frisbee

    [Header("Score Canvas (UGUI)")]
    [SerializeField] private TextMeshProUGUI canvasText;    // TMP inside your ScoreCanvas

    [SerializeField] private string prefix = "Teleports: ";

    private int _count = 0;

    private void OnEnable()
    {
        NewPortalScript.OnTeleportComplete += OnTeleport;
    }

    private void OnDisable()
    {
        NewPortalScript.OnTeleportComplete -= OnTeleport;
    }

    private void Start()
    {
        UpdateText();
    }

    private void OnTeleport()
    {
        _count++;
        Debug.Log($"[TeleportCounter] Teleport count: {_count}");
        UpdateText();
    }

    private void UpdateText()
    {
        string display = prefix + _count;

        if (frisbeeText != null)
            frisbeeText.text = display;

        if (canvasText != null)
            canvasText.text = display;
    }

    /// <summary>
    /// Returns the current count — useful for end-of-game score screens.
    /// </summary>
    public int GetCount() => _count;
}


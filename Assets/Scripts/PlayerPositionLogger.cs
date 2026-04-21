using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class PlayerPositionLogger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The transform to track — assign your Camera Rig or player root.")]
    public Transform playerTransform;

    [Header("Recording Settings")]
    [Tooltip("How often (in seconds) the position is sampled.")]
    public float sampleInterval = 0.5f;
    public string fileName = "PlayerHeatmap";

    private StreamWriter _writer;
    private float _sessionStartTime;

    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("[PlayerPositionLogger] No player transform assigned — disabling.");
            enabled = false;
            return;
        }

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string folder = Path.Combine(Application.dataPath, "RecordedData", "HeatmapData");
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, $"{fileName}_{timestamp}.csv");

        Debug.Log($"[PlayerPositionLogger] Saving heatmap data to: {path}");

        _writer = new StreamWriter(path, false, Encoding.UTF8);
        _writer.WriteLine("Time(s),X,Y,Z");
        _writer.Flush();

        _sessionStartTime = Time.realtimeSinceStartup;
        StartCoroutine(LogLoop());
    }

    private IEnumerator LogLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(sampleInterval);

            float sessionTime = Time.realtimeSinceStartup - _sessionStartTime;
            Vector3 pos = playerTransform.position;

            _writer.WriteLine($"{sessionTime:F2},{pos.x:F3},{pos.y:F3},{pos.z:F3}");
            _writer.Flush();
        }
    }

    private void OnDestroy()
    {
        if (_writer != null)
        {
            _writer.Close();
            Debug.Log("[PlayerPositionLogger] CSV closed.");
        }
    }
}


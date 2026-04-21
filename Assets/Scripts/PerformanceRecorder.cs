using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class PerformanceRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    [Tooltip("How often (in seconds) stats are sampled and written.")]
    public float sampleInterval = 1.0f;
    [Tooltip("File will be saved to the persistent data path (see console on Start for exact location).")]
    public string fileName = "PerformanceStats";

    [Header("Options")]
    public bool recordFPS = true;
    public bool recordFrameTime = true;
    public bool recordMemory = true;

    private StreamWriter _writer;
    private float _fpsAccumulator = 0f;
    private int _fpsFrameCount = 0;
    private float _timer = 0f;
    private float _sessionStartTime;

    private void Start()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string folder = Path.Combine(Application.dataPath, "RecordedData", "PerformanceStats");
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, $"{fileName}_{timestamp}.txt");

        Debug.Log($"[PerformanceRecorder] Saving stats to: {path}");

        _writer = new StreamWriter(path, false, Encoding.UTF8);
        WriteHeader();

        _sessionStartTime = Time.realtimeSinceStartup;
        StartCoroutine(RecordLoop());
    }

    private void Update()
    {
        _fpsAccumulator += 1f / Time.unscaledDeltaTime;
        _fpsFrameCount++;
    }

    private void WriteHeader()
    {
        _writer.WriteLine("PortalHolaHoop — Performance Stats");
        _writer.WriteLine($"Session started: {DateTime.Now}");
        _writer.WriteLine(new string('-', 60));

        var header = new StringBuilder("Time(s)");
        if (recordFPS)        header.Append("\tAvg FPS");
        if (recordFrameTime)  header.Append("\tFrame Time (ms)");
        if (recordMemory)     header.Append("\tMemory (MB)");

        _writer.WriteLine(header.ToString());
        _writer.Flush();
    }

    private IEnumerator RecordLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(sampleInterval);

            float sessionTime = Time.realtimeSinceStartup - _sessionStartTime;
            float avgFPS = _fpsFrameCount > 0 ? _fpsAccumulator / _fpsFrameCount : 0f;
            float frameTimeMs = avgFPS > 0 ? (1000f / avgFPS) : 0f;
            float memoryMB = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);

            var line = new StringBuilder($"{sessionTime:F1}");
            if (recordFPS)        line.Append($"\t{avgFPS:F1}");
            if (recordFrameTime)  line.Append($"\t{frameTimeMs:F2}");
            if (recordMemory)     line.Append($"\t{memoryMB:F1}");

            _writer.WriteLine(line.ToString());
            _writer.Flush();

            // Reset accumulators
            _fpsAccumulator = 0f;
            _fpsFrameCount = 0;
        }
    }

    private void OnDestroy()
    {
        if (_writer != null)
        {
            _writer.WriteLine(new string('-', 60));
            _writer.WriteLine($"Session ended: {DateTime.Now}");
            _writer.Close();
        }
    }
}


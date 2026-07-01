using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 蘑菇谜题 60 秒倒计时。超时后仅重置蘑菇段，不影响光点收集。
/// </summary>
public class PuzzleTimer : MonoBehaviour
{
    [SerializeField] private float durationSeconds = 60f;
    [SerializeField] private float remainingSeconds;

    public float RemainingSeconds => remainingSeconds;
    public bool IsRunning { get; private set; }

    [Header("Warning Thresholds")]
    [SerializeField] private float warningThreshold1 = 30f;
    [SerializeField] private float warningThreshold2 = 10f;

    public UnityEvent OnTimeout;

    private Coroutine timerRoutine;

    public void StartTimer()
    {
        remainingSeconds = durationSeconds;
        IsRunning = true;
        timerRoutine = StartCoroutine(TimerRoutine());
        Debug.Log($"[PuzzleTimer] Started: {durationSeconds}s");
    }

    public void StopTimer()
    {
        IsRunning = false;
        if (timerRoutine != null)
            StopCoroutine(timerRoutine);
        Debug.Log("[PuzzleTimer] Stopped");
    }

    private IEnumerator TimerRoutine()
    {
        while (remainingSeconds > 0)
        {
            remainingSeconds -= Time.deltaTime;

            if (remainingSeconds <= warningThreshold2 && remainingSeconds + Time.deltaTime > warningThreshold2)
                ShowWarning("时间快到了...");
            else if (remainingSeconds <= warningThreshold1 && remainingSeconds + Time.deltaTime > warningThreshold1)
                ShowWarning("还剩 30 秒...");

            if (remainingSeconds <= 0)
            {
                remainingSeconds = 0;
                OnTimeout?.Invoke();
                ShowWarning("时间到了...蘑菇们恢复了原样，再试一次吧。");
                IsRunning = false;
                break;
            }

            yield return null;
        }
    }

    private void ShowWarning(string msg)
    {
        if (SubtitleCutscene.Instance != null)
            SubtitleCutscene.Instance.Play(msg);
    }
}

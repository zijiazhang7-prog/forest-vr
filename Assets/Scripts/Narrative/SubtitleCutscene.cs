using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 黑屏字幕系统。全屏黑底 + 居中白字，支持逐条播放。
/// 播放期间锁定玩家输入。
/// </summary>
public class SubtitleCutscene : MonoBehaviour
{
    public static SubtitleCutscene Instance { get; private set; }

    [SerializeField] private Canvas canvas;
    [SerializeField] private Image blackPanel;
    [SerializeField] private Text subtitleText;

    [Header("Animation")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float holdDuration = 2f;

    public bool IsPlaying { get; private set; }

    private readonly Queue<string> pendingLines = new Queue<string>();
    private Coroutine playRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (blackPanel != null) blackPanel.gameObject.SetActive(false);
        if (canvas != null) canvas.gameObject.SetActive(false);
    }

    /// <summary>播放一系列字幕。如果正在播放则追加到队列。</summary>
    public void Play(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        foreach (var line in lines)
            pendingLines.Enqueue(line);

        if (!IsPlaying)
            playRoutine = StartCoroutine(PlayRoutine());
    }

    /// <summary>播放单条字幕。</summary>
    public void Play(string line)
    {
        Play(new[] { line });
    }

    private IEnumerator PlayRoutine()
    {
        IsPlaying = true;
        SetInputLocked(true);
        if (canvas != null) canvas.gameObject.SetActive(true);

        while (pendingLines.Count > 0)
        {
            var line = pendingLines.Dequeue();

            // Fade in
            if (subtitleText != null) subtitleText.text = line;
            if (blackPanel != null) blackPanel.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvas(0f, 1f, fadeInDuration));

            // Hold - wait for confirm button or timeout
            var elapsed = 0f;
            while (elapsed < holdDuration)
            {
                if (Input.GetKeyDown(KeyCode.Space) ||
                    (BluetoothButton.Instance != null && BluetoothButton.Instance.IsPressedThisFrame()))
                {
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Fade out
            yield return StartCoroutine(FadeCanvas(1f, 0f, fadeOutDuration));
            if (subtitleText != null) subtitleText.text = "";
        }

        if (blackPanel != null) blackPanel.gameObject.SetActive(false);
        if (canvas != null) canvas.gameObject.SetActive(false);
        IsPlaying = false;
        SetInputLocked(false);
    }

    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        if (blackPanel == null) yield break;

        var elapsed = 0f;
        var color = blackPanel.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / duration;
            color.a = Mathf.Lerp(from, to, t);
            blackPanel.color = color;
            yield return null;
        }

        color.a = to;
        blackPanel.color = color;
    }

    private void SetInputLocked(bool locked)
    {
        if (PlayerRigSetup.Instance != null)
            PlayerRigSetup.Instance.SetInputLocked(locked);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 独角兽图案 QTE 主控。3 轮摇杆描线，每轮匹配一个手势模板。
/// </summary>
public class PatternGestureQTE : MonoBehaviour
{
    [SerializeField] private float matchThreshold = 0.35f;
    [SerializeField] private float inputRecordInterval = 0.02f;

    [Header("UI")]
    [SerializeField] private RawImage gestureDisplay; // 显示模板图案的区域
    [SerializeField] private Text roundText;
    [SerializeField] private Image resultFeedback; // 匹配结果颜色闪烁

    [Header("Rounds")]
    [SerializeField] private GestureKind[] rounds = { GestureKind.StraightLine, GestureKind.WaveLine, GestureKind.ZigzagLine };

    [Header("Outcome")]
    [SerializeField] private UnicornReward unicornReward;

    public bool IsActive { get; private set; }
    private int currentRound;
    private List<Vector2> currentInput;
    private bool isRecording;
    private float recordTimer;

    private void Start()
    {
        enabled = false;
    }

    public void StartQTE()
    {
        if (GameProgressManager.Instance?.Stage != GameStage.UnicornPuzzle) return;

        IsActive = true;
        currentRound = 0;
        StartRound();
    }

    private void StartRound()
    {
        if (currentRound >= rounds.Length)
        {
            OnAllRoundsComplete();
            return;
        }

        currentInput = new List<Vector2>();
        isRecording = true;
        recordTimer = 0;

        if (roundText != null)
            roundText.text = $"第 {currentRound + 1} / {rounds.Length} 次画符";

        Debug.Log($"[PatternGestureQTE] Round {currentRound + 1}: {rounds[currentRound]}");
    }

    private void Update()
    {
        if (!IsActive || !isRecording) return;

        // 读取摇杆输入
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        var input = new Vector2(h, v);

        recordTimer += Time.deltaTime;

        if (recordTimer >= inputRecordInterval && input.magnitude > 0.05f)
        {
            currentInput.Add(input);
            recordTimer = 0;
        }

        // 松开摇杆一段时间后判定
        if (input.magnitude < 0.05f && currentInput.Count > 8)
        {
            isRecording = false;
            EvaluateRound();
        }

        // Editor 测试：Space 键提交
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return) && currentInput.Count > 4)
        {
            isRecording = false;
            EvaluateRound();
        }
        #endif
    }

    private void EvaluateRound()
    {
        var template = GestureTemplateData.GetTemplate(rounds[currentRound]);
        var distance = GestureMatcher.Match(currentInput, template);

        Debug.Log($"[PatternGestureQTE] Round {currentRound + 1}: distance={distance:F3}, threshold={matchThreshold}");

        if (distance < matchThreshold)
        {
            OnRoundPassed();
        }
        else
        {
            OnRoundFailed();
        }
    }

    private void OnRoundPassed()
    {
        StartCoroutine(ShowResultFeedback(Color.green, 0.5f));
        currentRound++;

        if (currentRound >= rounds.Length)
            OnAllRoundsComplete();
        else
            StartCoroutine(DelayedStartNextRound());
    }

    private void OnRoundFailed()
    {
        StartCoroutine(ShowResultFeedback(Color.red, 0.5f));
        if (SubtitleCutscene.Instance != null)
            SubtitleCutscene.Instance.Play("手势未能识别，请再试一次。");

        StartCoroutine(DelayedRetry());
    }

    private IEnumerator DelayedStartNextRound()
    {
        yield return new WaitForSeconds(0.8f);
        StartRound();
    }

    private IEnumerator DelayedRetry()
    {
        yield return new WaitForSeconds(0.8f);
        currentInput.Clear();
        isRecording = true;
    }

    private IEnumerator ShowResultFeedback(Color color, float duration)
    {
        if (resultFeedback != null)
        {
            resultFeedback.color = color;
            resultFeedback.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            resultFeedback.gameObject.SetActive(false);
        }
    }

    private void OnAllRoundsComplete()
    {
        IsActive = false;
        Debug.Log("[PatternGestureQTE] All rounds complete!");

        if (unicornReward != null)
            unicornReward.GrantReward();
    }
}

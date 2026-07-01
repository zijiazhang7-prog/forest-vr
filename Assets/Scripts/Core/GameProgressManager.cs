using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 全局状态机单例。管理 GameStage 的推进，通过 OnStageChanged 事件通知各模块。
/// 阶段只能单向推进，不可回退。
/// </summary>
public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    [SerializeField] private GameStage currentStage = GameStage.Intro;

    public GameStage Stage => currentStage;

    /// <summary>阶段变化事件，传递新阶段</summary>
    public UnityEvent<GameStage> OnStageChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[GameProgressManager] Duplicate instance destroyed", this);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (OnStageChanged == null)
            OnStageChanged = new UnityEvent<GameStage>();
    }

    private void Start()
    {
        OnStageChanged?.Invoke(currentStage);
    }

    /// <summary>
    /// 推进到下一阶段。如果目标阶段不在当前阶段之后则忽略。
    /// </summary>
    public void AdvanceStage(GameStage nextStage)
    {
        if (nextStage <= currentStage)
        {
            Debug.LogWarning($"[GameProgressManager] Ignored backward/redundant advance: {currentStage} -> {nextStage}");
            return;
        }

        currentStage = nextStage;
        Debug.Log($"[GameProgressManager] Stage → {currentStage}");
        OnStageChanged?.Invoke(currentStage);
    }

    /// <summary>仅 Editor 调试用，强制跳转阶段</summary>
    #if UNITY_EDITOR
    public void DebugSetStage(GameStage stage)
    {
        currentStage = stage;
        Debug.Log($"[GameProgressManager] DEBUG SET Stage → {currentStage}");
        OnStageChanged?.Invoke(currentStage);
    }
    #endif
}

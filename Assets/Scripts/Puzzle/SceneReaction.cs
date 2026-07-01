using UnityEngine;

/// <summary>
/// 通用场景反应脚本。监听阶段变化，激活/停用指定物体。
/// </summary>
public class SceneReaction : MonoBehaviour
{
    [SerializeField] private GameStage triggerStage;
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;
    [SerializeField] private bool activateOnOrAfter; // true: >= triggerStage 时激活

    private bool triggered;

    private void OnEnable()
    {
        if (GameProgressManager.Instance != null)
            GameProgressManager.Instance.OnStageChanged.AddListener(OnStageChanged);
    }

    private void OnDisable()
    {
        if (GameProgressManager.Instance != null)
            GameProgressManager.Instance.OnStageChanged.RemoveListener(OnStageChanged);
    }

    private void OnStageChanged(GameStage stage)
    {
        if (triggered) return;

        var shouldTrigger = activateOnOrAfter ? stage >= triggerStage : stage == triggerStage;
        if (!shouldTrigger) return;

        triggered = true;

        foreach (var obj in objectsToActivate)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in objectsToDeactivate)
            if (obj != null) obj.SetActive(false);
    }
}

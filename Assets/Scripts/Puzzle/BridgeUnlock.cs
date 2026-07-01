using UnityEngine;

/// <summary>
/// 桥的显示控制。监听 BridgeOpen 阶段，激活桥。
/// </summary>
public class BridgeUnlock : MonoBehaviour
{
    [SerializeField] private GameObject bridgeRoot;

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

    private void Start()
    {
        if (bridgeRoot != null)
            bridgeRoot.SetActive(false);
    }

    private void OnStageChanged(GameStage stage)
    {
        if (stage >= GameStage.BridgeOpen && bridgeRoot != null)
            bridgeRoot.SetActive(true);
    }
}

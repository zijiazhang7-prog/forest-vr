using UnityEngine;

/// <summary>
/// 终章区域触发器。进入后切到 EndingChoice 阶段。
/// </summary>
[RequireComponent(typeof(Collider))]
public class EndingZoneTrigger : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameProgressManager.Instance?.Stage != GameStage.HasThreeCores) return;
        if (MagicCoreInventory.Instance?.HasAll != true) return;

        GameProgressManager.Instance.AdvanceStage(GameStage.EndingChoice);
    }
}

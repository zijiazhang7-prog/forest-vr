using UnityEngine;

/// <summary>
/// 区域闸门。在 stage < BridgeOpen 时阻挡玩家过河。
/// TODO(B): 联调时补「把玩家推回安全侧」逻辑。
/// </summary>
[RequireComponent(typeof(Collider))]
public class ZoneGate : MonoBehaviour
{
    [SerializeField] private string blockedMessage = "河水湍急，你暂时无法通过。";
    [SerializeField] private Transform pushBackPoint;

    private bool isOpen;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOpen) return;
        if (!other.CompareTag("Player")) return;

        var stage = GameProgressManager.Instance?.Stage;
        if (stage >= GameStage.BridgeOpen) return;

        if (SubtitleCutscene.Instance != null)
            SubtitleCutscene.Instance.Play(new[] { blockedMessage });

        // TODO(B): 把玩家推回 pushBackPoint 位置
        if (pushBackPoint != null)
        {
            other.transform.position = pushBackPoint.position;
        }
    }

    public void OpenGate()
    {
        isOpen = true;
        if (SubtitleCutscene.Instance != null)
            SubtitleCutscene.Instance.Play(new[] { "河面上浮现出一座光桥..." });
    }
}

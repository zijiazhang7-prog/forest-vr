using UnityEngine;

/// <summary>
/// 魔法石底座。接收光点能量（OrbCollector 交付）和魔法核心（终章）。
/// </summary>
[RequireComponent(typeof(Collider))]
public class MagicPedestal : MonoBehaviour
{
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private bool isEndingPedestal; // 终章底座 vs 河岸底座

    private bool activated;
    private bool playerInRange;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        if (glowEffect != null)
            glowEffect.SetActive(false);
    }

    private void OnEnable()
    {
        if (BluetoothButton.Instance != null)
            BluetoothButton.Instance.OnInteractPressed += OnInteract;
    }

    private void OnDisable()
    {
        if (BluetoothButton.Instance != null)
            BluetoothButton.Instance.OnInteractPressed -= OnInteract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    private void OnInteract()
    {
        if (!playerInRange || activated) return;
        if (GameProgressManager.Instance == null) return;

        // 河岸底座：收光点
        if (!isEndingPedestal)
        {
            var collector = PlayerRigSetup.Instance?.GetComponent<OrbCollector>();
            if (collector != null && collector.TryDeliver(3))
            {
                activated = true;
                if (glowEffect != null) glowEffect.SetActive(true);
                GameProgressManager.Instance.AdvanceStage(GameStage.AfterOrbs);

                StartCoroutine(DelayedBridgeOpen());
            }
            else
            {
                if (SubtitleCutscene.Instance != null)
                    SubtitleCutscene.Instance.Play("还需要收集更多的光点能量...");
            }
        }
        // 终章底座：收核心
        else
        {
            var inventory = MagicCoreInventory.Instance;
            if (inventory != null && inventory.HasAll)
            {
                activated = true;
                if (glowEffect != null) glowEffect.SetActive(true);
                GameProgressManager.Instance.AdvanceStage(GameStage.EndingMagic);

                if (SubtitleCutscene.Instance != null)
                    SubtitleCutscene.Instance.Play("石座上的符文依次点亮，一扇宏伟的门缓缓打开...");
            }
        }
    }

    private System.Collections.IEnumerator DelayedBridgeOpen()
    {
        // 等字幕播完
        while (SubtitleCutscene.Instance != null && SubtitleCutscene.Instance.IsPlaying)
            yield return null;
        yield return new WaitForSeconds(0.5f);
        GameProgressManager.Instance.AdvanceStage(GameStage.BridgeOpen);
    }
}

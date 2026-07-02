using ForestVR;
using UnityEngine;

/// <summary>
/// 探索互动物体：玩家进入 Trigger 范围 + 按确认键 → 播放台词。
/// A 挂在树/花/河等物体上，在 Inspector 中填 promptLines。
/// </summary>
[RequireComponent(typeof(Collider))]
public class InspectableObject : MonoBehaviour
{
    [SerializeField] private string[] promptLines = new[] { "你发现了一些有趣的东西。" };

    private bool playerInRange;

    private void Start()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        if (BluetoothButtonInput.Instance != null)
            BluetoothButtonInput.Instance.OnInteractPressed += OnInteractPressed;
    }

    private void OnDisable()
    {
        if (BluetoothButtonInput.Instance != null)
            BluetoothButtonInput.Instance.OnInteractPressed -= OnInteractPressed;
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

    private void OnInteractPressed()
    {
        if (!playerInRange) return;
        if (SubtitleCutscene.Instance != null && SubtitleCutscene.Instance.IsPlaying) return;

        if (SubtitleCutscene.Instance != null)
            SubtitleCutscene.Instance.Play(promptLines);
    }
}

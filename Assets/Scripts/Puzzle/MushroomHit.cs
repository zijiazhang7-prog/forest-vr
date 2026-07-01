using UnityEngine;

/// <summary>
/// 单个蘑菇的碰撞检测。触发 OnInteract 时通知 MushroomSequencePuzzle。
/// B 已从 MushroomSequencePuzzle 中拆分此类为独立文件。
/// </summary>
[RequireComponent(typeof(Collider))]
public class MushroomHit : MonoBehaviour
{
    [SerializeField] private int mushroomId; // 蘑菇编号，对应 correctSequence 中的值

    public int MushroomId => mushroomId;

    private bool playerInRange;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
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
        if (!playerInRange) return;
        MushroomSequencePuzzle.Instance?.OnMushroomHit(mushroomId);
    }

    public void PlayCorrectFeedback()
    {
        // 发光 + 音效（由 MushroomSequencePuzzle 调用）
        Debug.Log($"[MushroomHit] Mushroom {mushroomId} correct");
    }

    public void PlayWrongFeedback()
    {
        // 变红 + 抖动（由 MushroomSequencePuzzle 调用）
        Debug.Log($"[MushroomHit] Mushroom {mushroomId} wrong");
    }
}

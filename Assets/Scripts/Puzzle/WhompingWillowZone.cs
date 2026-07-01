using System.Collections;
using UnityEngine;

/// <summary>
/// 打人柳警戒区。玩家进入 → 黑屏警告 → 不后退则传送回安全点。
/// </summary>
[RequireComponent(typeof(Collider))]
public class WhompingWillowZone : MonoBehaviour
{
    [SerializeField] private Transform safePoint;
    [SerializeField] private float warningDuration = 3f;
    [SerializeField] private string[] warningLines = {
        "打人柳的枝条向你抽来！快后退！",
        "危险！离开这棵树的范围！"
    };

    private Coroutine warningRoutine;
    private bool playerInside;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameProgressManager.Instance?.Stage != GameStage.UnicornPuzzle) return;

        playerInside = true;
        warningRoutine = StartCoroutine(WarningSequence(other.transform));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        if (warningRoutine != null)
        {
            StopCoroutine(warningRoutine);
            warningRoutine = null;
        }
    }

    private IEnumerator WarningSequence(Transform playerTransform)
    {
        // 显示警告
        if (SubtitleCutscene.Instance != null)
            SubtitleCutscene.Instance.Play(warningLines);

        var elapsed = 0f;
        while (elapsed < warningDuration && playerInside)
        {
            elapsed += Time.deltaTime;
            // 屏幕逐渐变暗
            yield return null;
        }

        // 仍未离开 → 传送
        if (playerInside)
        {
            if (safePoint != null)
                playerTransform.position = safePoint.position;

            if (SubtitleCutscene.Instance != null)
                SubtitleCutscene.Instance.Play("你被打了，回到了安全的地方。保持距离！");
        }
    }
}

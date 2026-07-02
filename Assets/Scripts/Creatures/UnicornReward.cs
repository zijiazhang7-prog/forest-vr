using UnityEngine;

/// <summary>
/// QTE 完成后独角兽给予 2 个魔法核心奖励。
/// </summary>
public class UnicornReward : MonoBehaviour
{
    [SerializeField] private MagicCore magicCorePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private NarrativeDirector narrativeDirector;

    public void GrantReward()
    {
        Debug.Log("[UnicornReward] Granting 2 cores");

        // 直接在独角兽位置给核心计数
        MagicCoreInventory.Instance?.Add(2);

        // 生成视觉核心
        if (magicCorePrefab != null && spawnPoints != null)
        {
            foreach (var sp in spawnPoints)
            {
                if (sp != null)
                    Instantiate(magicCorePrefab, sp.position, Quaternion.identity);
            }
        }

        // 字幕
        if (narrativeDirector != null)
            narrativeDirector.PlayUnicornComplete();

        StartCoroutine(DelayedStageAdvance());
    }

    private System.Collections.IEnumerator DelayedStageAdvance()
    {
        const float safetyTimeout = 30f;
        var waited = 0f;
        while (SubtitleCutscene.Instance != null && SubtitleCutscene.Instance.IsPlaying && waited < safetyTimeout)
        {
            waited += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        if (this != null)
            GameProgressManager.Instance?.AdvanceStage(GameStage.HasThreeCores);
    }
}

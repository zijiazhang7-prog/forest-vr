using System.Collections;
using UnityEngine;

/// <summary>
/// 蘑菇序列敲击谜题。管理正确序列比对、错误重置和完成逻辑。
/// </summary>
public class MushroomSequencePuzzle : MonoBehaviour
{
    public static MushroomSequencePuzzle Instance { get; private set; }

    [SerializeField] private int[] correctSequence = { 0, 2, 1, 3 };
    [SerializeField] private MushroomHit[] mushrooms;

    [Header("References")]
    [SerializeField] private MagicCore magicCorePrefab;
    [SerializeField] private Transform coreSpawnPoint;
    [SerializeField] private NifflerStealSequence nifflerSteal;
    [SerializeField] private NarrativeDirector narrativeDirector;

    [Header("Feedback Strings")]
    [SerializeField] private string[] complainLines = {
        "喂，敲错啦！",
        "不是这根蘑菇...",
        "痛！轻一点！",
        "你再敲错我可不客气了！"
    };

    private int currentIndex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        enabled = false; // 由 StoryDirector 在 MushroomPuzzle 阶段启用
    }

    private void OnEnable()
    {
        if (GameProgressManager.Instance?.Stage != GameStage.MushroomPuzzle) return;
        currentIndex = 0;
    }

    public void OnMushroomHit(int mushroomId)
    {
        if (GameProgressManager.Instance?.Stage != GameStage.MushroomPuzzle) return;

        if (mushroomId == correctSequence[currentIndex])
        {
            // 通过 mushroomId 找到对应蘑菇做反馈
            var hitMushroom = System.Array.Find(mushrooms, m => m != null && m.MushroomId == mushroomId);
            hitMushroom?.PlayCorrectFeedback();

            currentIndex++;
            Debug.Log($"[MushroomSequencePuzzle] Correct! Progress: {currentIndex}/{correctSequence.Length}");

            if (currentIndex >= correctSequence.Length)
                OnPuzzleComplete();
        }
        else
        {
            // 错误
            currentIndex = 0;
            Debug.Log($"[MushroomSequencePuzzle] Wrong! Expected {correctSequence[0]}, got {mushroomId}");

            // 重置所有蘑菇视觉
            if (mushrooms != null)
            {
                foreach (var m in mushrooms)
                {
                    if (m != null) m.PlayWrongFeedback();
                }
            }

            // 随机抱怨字幕
            var line = complainLines[Random.Range(0, complainLines.Length)];
            if (SubtitleCutscene.Instance != null)
                SubtitleCutscene.Instance.Play(line);

            StartCoroutine(ResetMushroomsVisual());
        }
    }

    private void OnPuzzleComplete()
    {
        Debug.Log("[MushroomSequencePuzzle] Puzzle complete!");

        // 生成魔法核心
        if (magicCorePrefab != null && coreSpawnPoint != null)
            Instantiate(magicCorePrefab, coreSpawnPoint.position, coreSpawnPoint.rotation);

        // 字幕
        if (narrativeDirector != null)
            narrativeDirector.PlayMushroomComplete();

        // 嗅嗅出场
        if (nifflerSteal != null)
            nifflerSteal.OnMushroomComplete();

        enabled = false;
    }

    private IEnumerator ResetMushroomsVisual()
    {
        yield return new WaitForSeconds(0.5f);
        // 视觉重置可在联调时加发光材质切换
    }
}

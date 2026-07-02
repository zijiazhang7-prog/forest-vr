using System.Collections;
using UnityEngine;

/// <summary>
/// 嗅嗅抢夺过场：蘑菇完成后从树后冲出，抢走核心，触发追逐。
/// </summary>
public class NifflerStealSequence : MonoBehaviour
{
    [SerializeField] private GameObject nifflerObject;
    [SerializeField] private Transform stealTarget;
    [SerializeField] private NifflerEscapeSequence escapeSequence;
    [SerializeField] private NarrativeDirector narrativeDirector;

    private void Start()
    {
        if (nifflerObject != null)
            nifflerObject.SetActive(false);
    }

    public void OnMushroomComplete()
    {
        StartCoroutine(StealRoutine());
    }

    private IEnumerator StealRoutine()
    {
        const float safetyTimeout = 30f;

        // 等蘑菇完成字幕结束
        var waited = 0f;
        while (SubtitleCutscene.Instance != null && SubtitleCutscene.Instance.IsPlaying && waited < safetyTimeout)
        {
            waited += Time.deltaTime;
            yield return null;
        }

        // 播放嗅嗅出场字幕
        if (narrativeDirector != null)
            narrativeDirector.PlayNifflerIntro();

        waited = 0f;
        while (SubtitleCutscene.Instance != null && SubtitleCutscene.Instance.IsPlaying && waited < safetyTimeout)
        {
            waited += Time.deltaTime;
            yield return null;
        }

        // 嗅嗅冲出
        if (nifflerObject != null)
            nifflerObject.SetActive(true);

        // 冲向核心抢走
        var startPos = nifflerObject.transform.position;
        var endPos = stealTarget != null ? stealTarget.position : startPos + Vector3.forward * 3f;
        var elapsed = 0f;
        var duration = 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / duration;
            nifflerObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // 抢走核心后开始逃跑
        if (escapeSequence != null)
            escapeSequence.StartEscape();
    }
}

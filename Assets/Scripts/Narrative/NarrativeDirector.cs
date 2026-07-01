using System;
using UnityEngine;

/// <summary>
/// 各阶段剧情字幕表。监听 OnStageChanged，自动播放对应的字幕。
/// 错误反馈字幕由谜题脚本直接调 SubtitleCutscene.Play()，不经过此类。
/// </summary>
public class NarrativeDirector : MonoBehaviour
{
    [Header("Intro")]
    [SerializeField] private string[] introLines = new[] {
        "你睁开眼睛...",
        "发现自己躺在一片陌生的森林里。",
        "身旁有一块发光的大石头。你的手中，握着一本泛黄的魔法书。",
        "也许...四处看看？"
    };

    [Header("Exploring - optional")]
    [SerializeField] private string[] exploringHint = new[] {
        "森林里似乎有什么东西在闪烁..."
    };

    [Header("After Orbs Collected")]
    [SerializeField] private string[] afterOrbsLines = new[] {
        "三道光点在你的魔杖尖端汇聚...",
        "河边的石座开始发光。或许可以把能量注入其中。"
    };

    [Header("Bridge Open")]
    [SerializeField] private string[] bridgeOpenLines = new[] {
        "石座吸收了光点能量，河面上一座光桥浮现出来。",
        "桥的对岸...似乎有什么在等着你。"
    };

    [Header("Mushroom Puzzle Start")]
    [SerializeField] private string[] mushroomPuzzleLines = new[] {
        "一只圆滚滚的蒲绒绒跳到你的脚边，示意你跟上它。"
    };

    [Header("Mushroom Complete")]
    [SerializeField] private string[] mushroomCompleteLines = new[] {
        "蘑菇们发出和谐的光芒，一颗魔法核心浮现出来——"
    };

    [Header("Niffler Intro")]
    [SerializeField] private string[] nifflerIntroLines = new[] {
        "一只嗅嗅从树后窜了出来！它抢走了核心！",
        "追上去，抢回属于你的东西！"
    };

    [Header("Niffler Recovered")]
    [SerializeField] private string[] nifflerRecoveredLines = new[] {
        "你夺回了魔法核心。嗅嗅悻悻地跑掉了。"
    };

    [Header("Unicorn Puzzle Start")]
    [SerializeField] private string[] unicornPuzzleLines = new[] {
        "蒲绒绒又一次跳了出来，带你走向森林更深处...",
        "一棵巨大的打人柳矗立在前方。",
        "在它身后，一只银白色的独角兽静静地站着。",
        "保持距离，用魔杖画出魔法轨迹来安抚它..."
    };

    [Header("Unicorn Complete")]
    [SerializeField] private string[] unicornCompleteLines = new[] {
        "独角兽信任了你。它走到你身边，将两颗魔法核心放入你的手中。"
    };

    [Header("Has Three Cores")]
    [SerializeField] private string[] hasThreeCoresLines = new[] {
        "三颗魔法核心在你手中散发着温暖的光芒...",
        "魔法书轻轻震动，似乎在告诉你可以做出选择了。",
        "走向森林的尽头吧。"
    };

    [Header("Ending Choice")]
    [SerializeField] private string[] endingChoiceLines = new[] {
        "三个魔法核心...你可以用它们激活魔法书，回到你的世界。",
        "也可以放入那个石座，打开通往魔法世界的门。",
        "选择吧。"
    };

    [Header("Ending Real")]
    [SerializeField] private string[] endingRealLines = new[] {
        "你选择了回到现实世界。",
        "魔法书发出耀眼的光芒，一条通道在你面前展开...",
        "再见了，位面交界点。"
    };

    [Header("Ending Magic")]
    [SerializeField] private string[] endingMagicLines = new[] {
        "你选择了进入魔法世界。",
        "石座上的符文依次点亮，一扇宏伟的门缓缓打开...",
        "新的冒险，从此刻开始。"
    };

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

    private void OnStageChanged(GameStage stage)
    {
        switch (stage)
        {
            case GameStage.Intro:
                PlayIfNotEmpty(introLines);
                break;
            case GameStage.AfterOrbs:
                PlayIfNotEmpty(afterOrbsLines);
                break;
            case GameStage.BridgeOpen:
                StartCoroutine(PlayBridgeOpenThenExploringHint());
                break;
            case GameStage.MushroomPuzzle:
                PlayIfNotEmpty(mushroomPuzzleLines);
                break;
            case GameStage.AfterNifflerChase:
                PlayIfNotEmpty(nifflerRecoveredLines);
                break;
            case GameStage.UnicornPuzzle:
                PlayIfNotEmpty(unicornPuzzleLines);
                break;
            case GameStage.HasThreeCores:
                PlayIfNotEmpty(hasThreeCoresLines);
                break;
            case GameStage.EndingChoice:
                PlayIfNotEmpty(endingChoiceLines);
                break;
            case GameStage.EndingReal:
                PlayIfNotEmpty(endingRealLines);
                break;
            case GameStage.EndingMagic:
                PlayIfNotEmpty(endingMagicLines);
                break;
        }
    }

    private void PlayIfNotEmpty(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;
        if (SubtitleCutscene.Instance != null)
            SubtitleCutscene.Instance.Play(lines);
    }

    private System.Collections.IEnumerator PlayBridgeOpenThenExploringHint()
    {
        PlayIfNotEmpty(bridgeOpenLines);
        // 等上一段字幕播完
        while (SubtitleCutscene.Instance != null && SubtitleCutscene.Instance.IsPlaying)
            yield return null;
        yield return new WaitForSeconds(1f);
        PlayIfNotEmpty(exploringHint);
    }

    /// <summary>谜题专用：播放蘑菇完成字幕</summary>
    public void PlayMushroomComplete()
    {
        PlayIfNotEmpty(mushroomCompleteLines);
    }

    /// <summary>谜题专用：播放嗅嗅出场字幕</summary>
    public void PlayNifflerIntro()
    {
        PlayIfNotEmpty(nifflerIntroLines);
    }

    /// <summary>谜题专用：播放独角兽完成字幕</summary>
    public void PlayUnicornComplete()
    {
        PlayIfNotEmpty(unicornCompleteLines);
    }
}

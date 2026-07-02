using ForestVR;
using UnityEngine;

/// <summary>
/// 总调度器。订阅 OnStageChanged，负责激活/停用场景物件、触发引路、开关桥等。
/// 不做字幕——字幕由 NarrativeDirector 负责。
/// </summary>
public class StoryDirector : MonoBehaviour
{
    [Header("Zone & Bridge")]
    [SerializeField] private ZoneGate riverGate;
    [SerializeField] private GameObject bridgeRoot;

    [Header("Orbs")]
    [SerializeField] private GameObject[] lightOrbs;

    [Header("Mushroom Puzzle")]
    [SerializeField] private MushroomSequencePuzzle mushroomPuzzle;
    [SerializeField] private PuzzleTimer puzzleTimer;

    [Header("Creatures")]
    [SerializeField] private GuideCreature puffskeinGuide1;
    [SerializeField] private GuideCreature puffskeinGuide2;
    [SerializeField] private NifflerStealSequence nifflerSteal;

    [Header("Unicorn")]
    [SerializeField] private PatternGestureQTE unicornQTE;
    [SerializeField] private WhompingWillowZone whompingWillow;

    [Header("Ending")]
    [SerializeField] private EndingZoneTrigger endingZone;
    [SerializeField] private EndingChoiceUI endingChoiceUI;

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

    private void Start()
    {
        if (bridgeRoot != null)
            bridgeRoot.SetActive(false);
    }

    private void OnStageChanged(GameStage stage)
    {
        switch (stage)
        {
            case GameStage.OrbsCollecting:
                ActivateOrbs();
                break;

            case GameStage.BridgeOpen:
                OpenBridge();
                break;

            case GameStage.MushroomPuzzle:
                StartMushroomPuzzle();
                break;

            case GameStage.AfterNifflerChase:
                StopPuzzleTimer();
                break;

            case GameStage.UnicornPuzzle:
                StartUnicornPuzzle();
                break;

            case GameStage.EndingChoice:
                ActivateEndingChoice();
                break;

            case GameStage.EndingReal:
            case GameStage.EndingMagic:
                LockInputOnEnding();
                break;
        }
    }

    private void ActivateOrbs()
    {
        if (lightOrbs == null) return;
        foreach (var orb in lightOrbs)
        {
            if (orb != null) orb.SetActive(true);
        }
    }

    private void OpenBridge()
    {
        if (bridgeRoot != null)
            bridgeRoot.SetActive(true);

        if (riverGate != null)
            riverGate.OpenGate();
    }

    private void StartMushroomPuzzle()
    {
        if (mushroomPuzzle != null)
            mushroomPuzzle.enabled = true;

        if (puzzleTimer != null)
            puzzleTimer.StartTimer();

        if (puffskeinGuide1 != null)
            puffskeinGuide1.StartPatrol();
    }

    private void StopPuzzleTimer()
    {
        if (puzzleTimer != null)
            puzzleTimer.StopTimer();
    }

    private void StartUnicornPuzzle()
    {
        if (puffskeinGuide2 != null)
            puffskeinGuide2.StartPatrol();

        if (whompingWillow != null)
            whompingWillow.enabled = true;
    }

    private void ActivateEndingChoice()
    {
        if (endingZone != null)
            endingZone.enabled = true;

        if (endingChoiceUI != null)
            endingChoiceUI.Show();
    }

    private void LockInputOnEnding()
    {
        if (PlayerRigSetup.Instance != null)
            PlayerRigSetup.Instance.SetInputLocked(true);
    }
}

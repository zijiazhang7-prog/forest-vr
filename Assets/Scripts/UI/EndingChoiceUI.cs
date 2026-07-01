using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 终章双结局选择 UI。两按钮对应两个结局。
/// </summary>
public class EndingChoiceUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button realWorldButton;  // 回归现实
    [SerializeField] private Button magicWorldButton; // 进入魔法世界

    private void Start()
    {
        if (canvas != null)
            canvas.gameObject.SetActive(false);

        if (realWorldButton != null)
            realWorldButton.onClick.AddListener(OnChooseRealWorld);

        if (magicWorldButton != null)
            magicWorldButton.onClick.AddListener(OnChooseMagicWorld);
    }

    public void Show()
    {
        if (GameProgressManager.Instance?.Stage != GameStage.EndingChoice) return;
        if (MagicCoreInventory.Instance?.HasAll != true) return;

        if (canvas != null)
            canvas.gameObject.SetActive(true);

        Debug.Log("[EndingChoiceUI] Shown");
    }

    private void OnChooseRealWorld()
    {
        if (GameProgressManager.Instance?.Stage != GameStage.EndingChoice) return;
        Debug.Log("[EndingChoiceUI] Chose: Real World");
        GameProgressManager.Instance.AdvanceStage(GameStage.EndingReal);
        if (canvas != null) canvas.gameObject.SetActive(false);
    }

    private void OnChooseMagicWorld()
    {
        if (GameProgressManager.Instance?.Stage != GameStage.EndingChoice) return;
        Debug.Log("[EndingChoiceUI] Chose: Magic World");
        GameProgressManager.Instance.AdvanceStage(GameStage.EndingMagic);
        if (canvas != null) canvas.gameObject.SetActive(false);
    }
}

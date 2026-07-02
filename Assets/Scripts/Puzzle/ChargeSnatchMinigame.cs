using ForestVR;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 蓄力抢夺小游戏。玩家进入终点 Trigger 后长按确认键 1.5s 蓄力。
/// </summary>
public class ChargeSnatchMinigame : MonoBehaviour
{
    [SerializeField] private float holdDuration = 1.5f;
    [SerializeField] private Slider chargeSlider;

    private bool isActive;
    private bool playerInRange;
    private float holdTimer;

    private void Start()
    {
        if (chargeSlider != null)
        {
            chargeSlider.gameObject.SetActive(false);
            chargeSlider.maxValue = holdDuration;
            chargeSlider.value = 0;
        }
    }

    public void Activate()
    {
        isActive = true;
        if (chargeSlider != null)
            chargeSlider.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            holdTimer = 0;
            UpdateSlider();
        }
    }

    private void Update()
    {
        if (!isActive || !playerInRange) return;

        var btn = BluetoothButtonInput.Instance;
        if (btn != null && btn.IsHeld())
        {
            holdTimer += Time.deltaTime;
            UpdateSlider();

            if (holdTimer >= holdDuration)
            {
                OnSnatchSuccess();
            }
        }
        else
        {
            holdTimer = Mathf.Max(0, holdTimer - Time.deltaTime * 2f);
            UpdateSlider();
        }
    }

    private void UpdateSlider()
    {
        if (chargeSlider != null)
            chargeSlider.value = holdTimer;
    }

    private void OnSnatchSuccess()
    {
        Debug.Log("[ChargeSnatchMinigame] Snatch success!");
        isActive = false;

        if (chargeSlider != null)
            chargeSlider.gameObject.SetActive(false);

        // 核心 +1
        MagicCoreInventory.Instance?.Add(1);

        if (GameProgressManager.Instance != null)
            GameProgressManager.Instance.AdvanceStage(GameStage.AfterNifflerChase);
    }
}

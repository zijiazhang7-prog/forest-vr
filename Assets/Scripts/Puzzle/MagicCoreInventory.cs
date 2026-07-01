using UnityEngine;

/// <summary>
/// 全局魔法核心计数器。挂在 _GameManagers 上。
/// 核心来源：嗅嗅抢夺 +1，独角兽奖励 +2。
/// </summary>
public class MagicCoreInventory : MonoBehaviour
{
    public static MagicCoreInventory Instance { get; private set; }

    [SerializeField] private int coreCount;

    public int Count => coreCount;
    public bool HasAll => coreCount >= 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Add(int amount)
    {
        coreCount += amount;
        Debug.Log($"[MagicCoreInventory] +{amount}, total: {coreCount}");
    }
}

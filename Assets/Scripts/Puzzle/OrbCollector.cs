using UnityEngine;

/// <summary>
/// 挂在 PlayerRig 上，管理玩家携带的光点数量。
/// </summary>
public class OrbCollector : MonoBehaviour
{
    public int OrbCount { get; private set; }

    public void CollectOrb()
    {
        OrbCount++;
        Debug.Log($"[OrbCollector] Orb collected: {OrbCount}/3");
    }

    /// <summary>
    /// 向底座交付光点。返回是否交付成功。
    /// </summary>
    public bool TryDeliver(int count)
    {
        if (OrbCount < count) return false;

        OrbCount -= count;
        Debug.Log($"[OrbCollector] Delivered {count} orbs, remaining: {OrbCount}");
        return true;
    }
}

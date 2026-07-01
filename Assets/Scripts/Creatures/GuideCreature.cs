using UnityEngine;

/// <summary>
/// 蒲绒绒引路控制器。监听阶段变化 → 启动跳跃引导。
/// </summary>
public class GuideCreature : MonoBehaviour
{
    [SerializeField] private HopWalker hopWalker;
    [SerializeField] private GameObject creatureVisual;

    private bool hasGuided;

    private void Start()
    {
        if (creatureVisual != null)
            creatureVisual.SetActive(false);
    }

    public void StartPatrol()
    {
        if (hasGuided) return;
        hasGuided = true;

        if (creatureVisual != null)
            creatureVisual.SetActive(true);

        if (hopWalker != null)
            hopWalker.StartHopping();
    }
}

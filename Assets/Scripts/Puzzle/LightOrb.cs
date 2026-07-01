using UnityEngine;

/// <summary>
/// 漂浮光点。玩家靠近后自动吸附到 OrbCollector。
/// </summary>
public class LightOrb : MonoBehaviour
{
    [SerializeField] private float floatAmplitude = 0.3f;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float collectRadius = 2f;
    [SerializeField] private float collectDuration = 0.5f;

    private Vector3 startPos;
    private bool collected;
    private Transform target;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (collected)
        {
            FlyToTarget();
            return;
        }

        // 悬浮动画
        var offset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = startPos + Vector3.up * offset;
        transform.Rotate(Vector3.up, 60f * Time.deltaTime);

        // 检测玩家距离
        var player = PlayerRigSetup.Instance;
        if (player == null) return;

        var dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist <= collectRadius)
        {
            collected = true;
            target = player.transform;
        }
    }

    private void FlyToTarget()
    {
        if (target == null) return;

        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime / collectDuration);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            // 通知 OrbCollector
            var collector = target.GetComponent<OrbCollector>();
            if (collector != null)
                collector.CollectOrb();

            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}

using System.Collections;
using UnityEngine;

/// <summary>
/// 嗅嗅沿预设路径点逃跑。到终点后开启蓄力抢夺窗口。
/// </summary>
public class NifflerEscapeSequence : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float waypointThreshold = 0.5f;

    [Header("Ending")]
    [SerializeField] private ChargeSnatchMinigame chargeMinigame;

    private bool escaping;
    private int currentWaypointIndex;

    private void Start()
    {
        enabled = false;
    }

    public void StartEscape()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("[NifflerEscapeSequence] No waypoints assigned");
            return;
        }

        escaping = true;
        transform.position = waypoints[0].position;
        currentWaypointIndex = 1;
    }

    private void Update()
    {
        if (!escaping) return;
        if (currentWaypointIndex >= waypoints.Length) return;

        var target = waypoints[currentWaypointIndex].position;
        var direction = (target - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 面向移动方向
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, target) < waypointThreshold)
        {
            currentWaypointIndex++;

            // 到达终点
            if (currentWaypointIndex >= waypoints.Length)
            {
                escaping = false;
                OnReachEnd();
            }
        }
    }

    private void OnReachEnd()
    {
        Debug.Log("[NifflerEscapeSequence] Reached end point");
        if (chargeMinigame != null)
            chargeMinigame.Activate();
    }
}

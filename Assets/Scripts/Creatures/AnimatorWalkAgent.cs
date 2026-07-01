using UnityEngine;

/// <summary>
/// 沿 waypoints 行走的通用行动代理（嗅嗅、独角兽用）。
/// loopPath=false 用于逃跑路线。
/// </summary>
public class AnimatorWalkAgent : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float waypointThreshold = 0.3f;
    [SerializeField] private bool loopPath;

    private int currentIndex;
    private bool isWalking;

    public bool IsWalking => isWalking;

    public void StartWalking()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("[AnimatorWalkAgent] No waypoints", this);
            return;
        }

        isWalking = true;
        currentIndex = 0;
        transform.position = waypoints[0].position;
        currentIndex = 1;
    }

    public void StopWalking()
    {
        isWalking = false;
    }

    private void Update()
    {
        if (!isWalking) return;
        if (currentIndex >= waypoints.Length)
        {
            if (loopPath)
                currentIndex = 0;
            else
            {
                isWalking = false;
                return;
            }
        }

        var target = waypoints[currentIndex].position;
        var direction = (target - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            var lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target) < waypointThreshold)
            currentIndex++;
    }
}

using UnityEngine;

/// <summary>
/// 蒲绒绒跳跃式沿路径移动。
/// </summary>
public class HopWalker : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float hopHeight = 0.5f;
    [SerializeField] private float hopDuration = 0.4f;
    [SerializeField] private float hopInterval = 0.2f;

    private bool isHopping;
    private int currentIndex;

    public void StartHopping()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("[HopWalker] No waypoints", this);
            return;
        }

        isHopping = true;
        currentIndex = 0;
        transform.position = waypoints[0].position;
        currentIndex = 1;
        StartCoroutine(HopRoutine());
    }

    private System.Collections.IEnumerator HopRoutine()
    {
        while (isHopping && currentIndex < waypoints.Length)
        {
            var start = transform.position;
            var end = waypoints[currentIndex].position;
            var elapsed = 0f;

            // 跳跃弧线
            while (elapsed < hopDuration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / hopDuration;
                var pos = Vector3.Lerp(start, end, t);
                pos.y += Mathf.Sin(t * Mathf.PI) * hopHeight;
                transform.position = pos;
                yield return null;
            }

            transform.position = end;
            currentIndex++;

            if (currentIndex >= waypoints.Length)
                isHopping = false;

            yield return new WaitForSeconds(hopInterval);
        }
    }
}

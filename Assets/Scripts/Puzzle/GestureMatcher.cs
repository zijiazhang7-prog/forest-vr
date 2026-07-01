using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 轨迹比对器。使用简化的点对点距离累积匹配用户输入与模板。
/// </summary>
public static class GestureMatcher
{
    /// <summary>
    /// 计算用户轨迹与模板的匹配度。返回值越小越匹配。
    /// </summary>
    public static float Match(List<Vector2> userInput, Vector2[] template, int resampleCount = 32)
    {
        if (userInput == null || userInput.Count < 4) return float.MaxValue;
        if (template == null || template.Length < 2) return float.MaxValue;

        // 重采样用户轨迹到与模板相同的采样点数
        var resampled = Resample(userInput, resampleCount);

        // 归一化用户轨迹
        var normalized = Normalize(resampled);

        // 计算累积点对点距离
        var totalDistance = 0f;
        for (var i = 0; i < resampleCount && i < template.Length; i++)
        {
            totalDistance += Vector2.Distance(normalized[i], template[i]);
        }

        return totalDistance / resampleCount;
    }

    private static List<Vector2> Resample(List<Vector2> points, int targetCount)
    {
        if (points.Count < 2) return new List<Vector2>(points);

        // 计算路径总长度
        var totalLength = 0f;
        for (var i = 1; i < points.Count; i++)
            totalLength += Vector2.Distance(points[i - 1], points[i]);

        var interval = totalLength / (targetCount - 1);
        var result = new List<Vector2> { points[0] };

        var accumulatedDistance = 0f;
        var pointIndex = 0;

        for (var i = 1; i < targetCount - 1; i++)
        {
            var targetDistance = i * interval;

            while (pointIndex < points.Count - 1)
            {
                var segLength = Vector2.Distance(points[pointIndex], points[pointIndex + 1]);
                if (accumulatedDistance + segLength >= targetDistance)
                {
                    var t = (targetDistance - accumulatedDistance) / segLength;
                    result.Add(Vector2.Lerp(points[pointIndex], points[pointIndex + 1], t));
                    break;
                }
                accumulatedDistance += segLength;
                pointIndex++;
            }
        }

        result.Add(points[points.Count - 1]);
        return result;
    }

    private static Vector2[] Normalize(List<Vector2> points)
    {
        if (points.Count == 0) return new Vector2[0];

        // 减去起点
        var origin = points[0];
        var translated = new List<Vector2>();
        foreach (var p in points)
            translated.Add(p - origin);

        // 缩放到单位包围盒
        var maxMagnitude = 0f;
        foreach (var p in translated)
        {
            var m = Mathf.Max(Mathf.Abs(p.x), Mathf.Abs(p.y));
            if (m > maxMagnitude) maxMagnitude = m;
        }

        var result = new Vector2[translated.Count];
        var scale = maxMagnitude > 0.001f ? 1f / maxMagnitude : 1f;
        for (var i = 0; i < translated.Count; i++)
            result[i] = translated[i] * scale;

        return result;
    }
}

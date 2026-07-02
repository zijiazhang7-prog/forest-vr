using UnityEngine;

/// <summary>
/// 手势模板数据（ScriptableObject 或代码内嵌）。
/// 预定义直线、波浪线、折线的归一化采样点。
/// </summary>
[CreateAssetMenu(menuName = "ForestVR/Gesture Template")]
public class GestureTemplates : ScriptableObject
{
    public GestureKind kind;
    public Vector2[] samplePoints; // 32 个归一化采样点
}

/// <summary>
/// 运行时手势模板提供器。不依赖 ScriptableObject 也可用内置模板。
/// </summary>
public static class GestureTemplateData
{
    private static Vector2[] cachedStraight;
    private static Vector2[] cachedWave;
    private static Vector2[] cachedZigzag;

    public static Vector2[] GetTemplate(GestureKind kind)
    {
        return kind switch
        {
            GestureKind.StraightLine => cachedStraight ??= GenerateStraightLine(32),
            GestureKind.WaveLine => cachedWave ??= GenerateWaveLine(32),
            GestureKind.ZigzagLine => cachedZigzag ??= GenerateZigzagLine(32),
            _ => cachedStraight ??= GenerateStraightLine(32),
        };
    }

    private static Vector2[] GenerateStraightLine(int count)
    {
        var points = new Vector2[count];
        for (var i = 0; i < count; i++)
        {
            var t = (float)i / (count - 1);
            points[i] = new Vector2(t * 2f - 1f, 0); // 从左到右的水平线
        }
        return points;
    }

    private static Vector2[] GenerateWaveLine(int count)
    {
        var points = new Vector2[count];
        for (var i = 0; i < count; i++)
        {
            var t = (float)i / (count - 1);
            var x = t * 2f - 1f;
            var y = Mathf.Sin(t * Mathf.PI * 2f) * 0.6f;
            points[i] = new Vector2(x, y);
        }
        return points;
    }

    private static Vector2[] GenerateZigzagLine(int count)
    {
        var points = new Vector2[count];
        var segments = 3; // 三段折线
        for (var i = 0; i < count; i++)
        {
            var t = (float)i / (count - 1);
            var segmentT = t * segments;
            var segmentIndex = Mathf.FloorToInt(segmentT);
            var localT = segmentT - segmentIndex;

            var x = t * 2f - 1f;
            var y = (segmentIndex % 2 == 0) ? (localT - 0.5f) * 1.2f : (0.5f - localT) * 1.2f;
            points[i] = new Vector2(x, y);
        }
        return points;
    }
}

using UnityEngine;

/// <summary>
/// 可拾取/生成的魔法核心物体。
/// </summary>
public class MagicCore : MonoBehaviour
{
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotationSpeed = 90f;

    private Vector3 startPos;
    public bool IsCollected { get; private set; }

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (IsCollected) return;

        var offset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = startPos + Vector3.up * offset;
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void Collect()
    {
        IsCollected = true;
        MagicCoreInventory.Instance?.Add(1);
        Destroy(gameObject);
    }
}

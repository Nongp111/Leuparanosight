using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    public LineRenderer linePrefab;   // Prefab ของ LineRenderer
    public float duration = 0.05f;    // เวลาแสดงเส้น
    public Color tracerColor = Color.yellow;

    public void CreateTracer(Vector3 start, Vector3 end)
    {
        LineRenderer line = Instantiate(linePrefab, start, Quaternion.identity);
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.startColor = tracerColor;
        line.endColor = tracerColor;

        Destroy(line.gameObject, duration);
    }
}

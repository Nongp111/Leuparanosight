using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    public static event Action<Vector3, float> OnNoiseEmitted;

    public float walkNoiseRadius = 6f;
    public float runNoiseRadius = 12f;
    public float noiseInterval = 0.2f;

    private float timer;
    private Vector3 lastPos;

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;

        float speed = (transform.position - lastPos).magnitude / Time.deltaTime;

        // ถ้าขยับจริง
        if (speed > 0.2f)
        {
            if (timer >= noiseInterval)
            {
                float radius = speed > 3f ? runNoiseRadius : walkNoiseRadius;
                EmitNoise(radius);
                timer = 0f;
            }
        }

        lastPos = transform.position;
    }

    private void EmitNoise(float radius)
    {
        Debug.Log("NOISE EMITTED radius: " + radius);
        OnNoiseEmitted?.Invoke(transform.position, radius);
    }
}
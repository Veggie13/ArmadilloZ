using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaveField {
    public static float GetAmplitude(float x, float z)
    {
        float s = Mathf.Sqrt(x * x + z * z);
        float y = amplitude * Mathf.Cos(kmultiplier * s - (Time.fixedTime));
    }
}

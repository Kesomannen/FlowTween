using System;
using UnityEngine;

namespace FlowTween {

public static class Easing {
    const float HalfPI = Mathf.PI / 2;
    
    public static float Linear(float t) => t;
    
    public static float SineIn(float t) => 1 - Mathf.Cos(t * HalfPI);
    public static float SineOut(float t) => Mathf.Sin(t * HalfPI);
    public static float SineInOut(float t) => -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
    
    public static float QuadIn(float t) => PolyIn(t, 2);
    public static float QuadOut(float t) => PolyOut(t, 2);
    public static float QuadInOut(float t) => PolyInOut(t, 2);
    
    public static float CubicIn(float t) => PolyIn(t, 3);
    public static float CubicOut(float t) => PolyOut(t, 3);
    public static float CubicInOut(float t) => PolyInOut(t, 3);
    
    public static float QuartIn(float t) => PolyIn(t, 4);
    public static float QuartOut(float t) => PolyOut(t, 4);
    public static float QuartInOut(float t) => PolyInOut(t, 4);
    
    public static float QuintIn(float t) => PolyIn(t, 5);
    public static float QuintOut(float t) => PolyOut(t, 5);
    public static float QuintInOut(float t) => PolyInOut(t, 5);
    
    public static float ExpoIn(float t) => t == 0 ? 0 : Mathf.Pow(2, 10 * t - 10);
    public static float ExpoOut(float t) => Equal(t, 1) ? 1 : 1 - Mathf.Pow(2, -10 * t);
    public static float ExpoInOut(float t) => t == 0
        ? 0
        : Equal(t, 1) 
        ? 1
        : t < 0.5f ? Mathf.Pow(2, 20 * t - 10) / 2
        : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
    
    public static float CircIn(float t) => 1 - Mathf.Sqrt(1 - Mathf.Pow(t, 2));
    public static float CircOut(float t) => Mathf.Sqrt(1 - Mathf.Pow(t - 1, 2));
    public static float CircInOut(float t) => t < 0.5f
        ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) / 2
        : (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) / 2;
    
    const float C1 = 1.70158f;
    const float C2 = C1 * 1.525f;
    const float C3 = C1 + 1;
    const float C4 = 2 * Mathf.PI / 3;
    
    public static float BackIn(float t) => C3 * t * t * t - C1 * t * t;
    public static float BackOut(float t) => 1 + C3 * Mathf.Pow(t - 1, 3) + C1 * Mathf.Pow(t - 1, 2);
    public static float BackInOut(float t) => t < 0.5f
        ? Mathf.Pow(2 * t, 2) * ((C2 + 1) * 2 * t - C2) / 2
        : (Mathf.Pow(2 * t - 2, 2) * ((C2 + 1) * (t * 2 - 2) + C2) + 2) / 2;
    
    public static float ElasticIn(float t) => Equal(t, 0)
        ? 0
        : Equal(t, 1)
        ? 1
        : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * C4);
    
    public static float ElasticOut(float t) => Equal(t, 0)
        ? 0
        : Equal(t, 1)
        ? 1
        : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * C4) + 1;
    
    public static float ElasticInOut(float t) => Equal(t, 0)
        ? 0
        : Equal(t, 1)
        ? 1
        : t < 0.5f
        ? -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * C4)) / 2
        : Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * C4) / 2 + 1;
    
    const float N1 = 7.5625f;
    const float D1 = 2.75f;
    
    public static float BounceIn(float t) => 1 - BounceOut(1 - t);
    
    public static float BounceOut(float t) {
        switch (t) {
            case < 1 / D1:
                return N1 * t * t;
            case < 2 / D1:
                return N1 * (t -= 1.5f / D1) * t + 0.75f;
        }

        if (t < 2.5 / D1) {
            return N1 * (t -= 2.25f / D1) * t + 0.9375f;
        }
        
        return N1 * (t -= 2.625f / D1) * t + 0.984375f;
    }
    
    public static float BounceInOut(float t) => t < 0.5f
        ? (1 - BounceOut(1 - 2 * t)) / 2
        : (1 + BounceOut(2 * t - 1)) / 2;
    
    public static float PolyIn(float t, int power) => Mathf.Pow(t, power);
    public static float PolyOut(float t, int power) => 1 - Mathf.Pow(1 - t, power);
    public static float PolyInOut(float t, int power) => t < 0.5f ? Mathf.Pow(2 * t, power) / 2 : 1 - Mathf.Pow(-2 * t + 2, power) / 2;

    static bool Equal(float a, float b) => Math.Abs(a - b) < 0.0001f;
}

}
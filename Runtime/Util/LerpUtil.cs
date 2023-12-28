using UnityEngine;

namespace FlowTween {

public static class LerpUtil {
    public static float Lerp(float a, float b, float t) => a + (b - a) * t;
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => a + (b - a) * t;
    public static Vector3 Lerp(Vector3 a, Vector3 b, float t) => a + (b - a) * t;
    public static Vector4 Lerp(Vector4 a, Vector4 b, float t) => a + (b - a) * t;

    public static Quaternion Lerp(Quaternion a, Quaternion b, float t) {
        return Quaternion.Lerp(a, b, t);
    }
    
    public static Color Lerp(Color a, Color b, float t) {
        return new Color(
            Lerp(a.r, b.r, t),
            Lerp(a.g, b.g, t),
            Lerp(a.b, b.b, t),
            Lerp(a.a, b.a, t)
        );
    }
}

}
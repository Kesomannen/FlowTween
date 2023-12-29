using System;
using UnityEngine;

namespace FlowTween {

public static class TransformTweens {
    public static Vector3TweenFactory<Transform> Position { get; } = new(t => t.position, (t, v) => t.position = v);
    public static Vector3TweenFactory<Transform> LocalPosition { get; } = new(t => t.localPosition, (t, v) => t.localPosition = v);
    public static Vector3TweenFactory<Transform> Scale { get; } = new(t => t.localScale, (t, v) => t.localScale = v);
    public static FloatTweenFactory<Transform> UniformScale { get; } = new(t => t.localScale.x, (t, v) => t.localScale = Vector3.one * v);
    public static QuaternionTweenFactory<Transform> Rotation { get; } = new(t => t.rotation, (t, v) => t.rotation = v);
    public static QuaternionTweenFactory<Transform> LocalRotation { get; } = new(t => t.localRotation, (t, v) => t.localRotation = v);
    public static Vector3TweenFactory<Transform> EulerAngles { get; } = new(t => t.eulerAngles, (t, v) => t.eulerAngles = v);
    public static Vector3TweenFactory<Transform> LocalEulerAngles { get; } = new(t => t.localEulerAngles, (t, v) => t.localEulerAngles = v);

    static readonly Axis[] _2d = { Axis.X, Axis.Y };
    
    public static Tween<Vector3> TweenPosition(this Transform transform, Vector3 position) => transform.Tween(Position, position);
    public static Tween<Vector3> TweenPosition(this Transform transform, Vector2 position) => transform.Tween(Position, _2d, position);
    public static Tween<float> TweenX(this Transform transform, float x) => transform.Tween(Position, Axis.X, x);
    public static Tween<float> TweenY(this Transform transform, float y) => transform.Tween(Position, Axis.Y, y);
    public static Tween<float> TweenZ(this Transform transform, float z) => transform.Tween(Position, Axis.Z, z);
    
    public static Tween<Vector3> TweenLocalPosition(this Transform transform, Vector3 position) => transform.Tween(LocalPosition, position);
    public static Tween<Vector3> TweenLocalPosition(this Transform transform, Vector2 position) => transform.Tween(LocalPosition, _2d, position);
    public static Tween<float> TweenLocalX(this Transform transform, float x) => transform.Tween(LocalPosition, Axis.X, x);
    public static Tween<float> TweenLocalY(this Transform transform, float y) => transform.Tween(LocalPosition, Axis.Y, y);
    public static Tween<float> TweenLocalZ(this Transform transform, float z) => transform.Tween(LocalPosition, Axis.Z, z);
    
    public static Tween<Vector3> TweenScale(this Transform transform, Vector3 scale) => transform.Tween(Scale, scale);
    public static Tween<Vector3> TweenScale(this Transform transform, Vector2 scale) => transform.Tween(Scale, _2d, scale);
    public static Tween<float> TweenScaleX(this Transform transform, float x) => transform.Tween(Scale, Axis.X, x);
    public static Tween<float> TweenScaleY(this Transform transform, float y) => transform.Tween(Scale, Axis.Y, y);
    public static Tween<float> TweenScaleZ(this Transform transform, float z) => transform.Tween(Scale, Axis.Z, z);
    
    public static Tween<float> TweenScaleUniform(this Transform transform, float scale) => transform.Tween(UniformScale, scale);
    
    public static Tween<Quaternion> TweenRotation(this Transform transform, Vector3 rotation) => transform.Tween(Rotation, Quaternion.Euler(rotation));
    public static Tween<float> TweenRotationX(this Transform transform, float x) => transform.Tween(EulerAngles, Axis.X, x);
    public static Tween<float> TweenRotationY(this Transform transform, float y) => transform.Tween(EulerAngles, Axis.Y, y);
    public static Tween<float> TweenRotationZ(this Transform transform, float z) => transform.Tween(EulerAngles, Axis.Z, z);
    
    public static Tween<Quaternion> TweenLocalRotation(this Transform transform, Vector3 rotation) => transform.Tween(LocalRotation, Quaternion.Euler(rotation));
    public static Tween<float> TweenLocalRotationX(this Transform transform, float x) => transform.Tween(LocalEulerAngles, Axis.X, x);
    public static Tween<float> TweenLocalRotationY(this Transform transform, float y) => transform.Tween(LocalEulerAngles, Axis.Y, y);
    public static Tween<float> TweenLocalRotationZ(this Transform transform, float z) => transform.Tween(LocalEulerAngles, Axis.Z, z);
}

}
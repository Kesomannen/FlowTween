using System;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// A factory for creating tweens that animate <see cref="Quaternion"/> properties.
/// Also implements a composite interface for animating individual parts of
/// the <see cref="Quaternion"/>, or Euler angles.
/// </summary>
/// <typeparam name="T">The type of the object that holds the property. Most commonly an <see cref="UnityEngine.Object"/>.</typeparam>
public class QuaternionTweenFactory<T> : 
    TweenFactory<Quaternion, T>,
    ICompositeTweenFactory<Quaternion, T, float, Axis4>,
    ICompositeTweenFactory<Vector3, T, float, Axis>
{
    public QuaternionTweenFactory(Func<T, Quaternion> getter, Action<T, Quaternion> setter) 
        : base(getter, setter, Quaternion.LerpUnclamped) { }

    public void SetPart(ref Quaternion composite, Axis4 part, float value) => composite[(int)part] = value;
    public float GetPart(Quaternion composite, Axis4 part) => composite[(int)part];

    public void SetPart(ref Vector3 composite, Axis part, float value) => composite[(int)part] = value;
    public float GetPart(Vector3 composite, Axis part) => composite[(int)part];
    
    public float Lerp(float from, float to, float t) => Mathf.LerpUnclamped(from, to, t);

    void ITweenFactory<Vector3, T>.Set(T holder, Vector3 value) => Set(holder, Quaternion.Euler(value));
    Vector3 ITweenFactory<Vector3, T>.Get(T holder) => Get(holder).eulerAngles;
    Vector3 ITweenFactory<Vector3, T>.Lerp(Vector3 from, Vector3 to, float t) => Vector3.LerpUnclamped(from, to, t);
}

public enum Axis4 {
    X, Y, Z, W
}

}
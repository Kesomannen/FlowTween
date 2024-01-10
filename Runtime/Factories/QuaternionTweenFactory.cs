using System;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// A factory for creating tweens that animate <see cref="Quaternion"/> properties.
/// Also implements a composite interface for animating individual components of
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

    /// <summary>
    /// Creates a factory that animates a Quaternion property with the given name,
    /// using <see cref="ReflectionTweenFactory"/>.
    /// </summary>
    /// <remarks>
    /// Since this uses reflection under the hood, the performance is significantly worse
    /// than defining the getter and setter yourself. If performance is a concern, you should
    /// use <see cref="QuaternionTweenFactory{T}(System.Func{T,Quaternion},System.Action{T,Quaternion})"/> instead.
    /// </remarks>
    public static QuaternionTweenFactory<T> From(string propertyName) {
        return ReflectionTweenFactory.Create<QuaternionTweenFactory<T>>(propertyName);
    }
    
    public void SetComponent(ref Quaternion composite, Axis4 component, float value) => composite[(int)component] = value;
    public float GetComponent(Quaternion composite, Axis4 component) => composite[(int)component];

    public void SetComponent(ref Vector3 composite, Axis component, float value) => composite[(int)component] = value;
    public float GetComponent(Vector3 composite, Axis component) => composite[(int)component];
    
    public float Lerp(float from, float to, float t) => Mathf.LerpUnclamped(from, to, t);

    void ITweenFactory<Vector3, T>.Set(T holder, Vector3 value) => Set(holder, Quaternion.Euler(value));
    Vector3 ITweenFactory<Vector3, T>.Get(T holder) => Get(holder).eulerAngles;
    Vector3 ITweenFactory<Vector3, T>.Lerp(Vector3 from, Vector3 to, float t) => Vector3.LerpUnclamped(from, to, t);
}

public enum Axis4 {
    X, Y, Z, W
}

}
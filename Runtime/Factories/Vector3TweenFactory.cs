using System;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// A factory for creating tweens that animate <see cref="Vector3"/> properties.
/// Also implements a composite interface for animating individual parts of
/// the <see cref="Vector3"/> by specifying one or more <see cref="Axis"/>.
/// </summary>
/// <typeparam name="T">The type of the object that holds the property. Most commonly an <see cref="UnityEngine.Object"/>.</typeparam>
public class Vector3TweenFactory<T> : 
    TweenFactory<Vector3, T>,
    ICompositeTweenFactory<Vector3, T, float, Axis>
{
    public Vector3TweenFactory(Func<T, Vector3> getter, Action<T, Vector3> setter) 
        : base(getter, setter, Vector3.LerpUnclamped) { }

    /// <summary>
    /// Creates a factory that animates a Vector3 property with the given name,
    /// using <see cref="ReflectionTweenFactory"/>.
    /// </summary>
    /// <remarks>
    /// Since this uses reflection under the hood, the performance is significantly worse
    /// than defining the getter and setter yourself. If performance is a concern, you should
    /// use <see cref="Vector3TweenFactory{T}(System.Func{T,Vector3},System.Action{T,Vector3})"/> instead.
    /// </remarks>
    public static Vector3TweenFactory<T> Create(string propertyName) {
        return ReflectionTweenFactory.Create<Vector3TweenFactory<T>>(propertyName);
    }

    public void SetPart(ref Vector3 composite, Axis part, float value) => composite[(int)part] = value;
    public float GetPart(Vector3 composite, Axis part) => composite[(int)part];

    public float Lerp(float from, float to, float t) => Mathf.LerpUnclamped(from, to, t);
    
    public ICompositeTweenFactory<Vector3, T, float, Axis> Composite => this;
}

public enum Axis {
    X, Y, Z
}

}
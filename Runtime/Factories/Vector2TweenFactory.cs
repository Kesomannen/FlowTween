﻿using System;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// A factory for creating tweens that animate <see cref="Vector2"/> properties.
/// Also implements a composite interface for animating individual components of
/// the <see cref="Vector2"/> by specifying an <see cref="Axis2"/>.
/// </summary>
/// <typeparam name="T">The type of the object that holds the property. Most commonly an <see cref="UnityEngine.Object"/>.</typeparam>
public class Vector2TweenFactory<T> : 
    TweenFactory<Vector2, T>,
    ICompositeTweenFactory<Vector2, T, float, Axis2> 
{
    public Vector2TweenFactory(Func<T, Vector2> getter, Action<T, Vector2> setter) 
        : base(getter, setter, Vector2.LerpUnclamped) { }

    /// <summary>
    /// Creates a factory that animates a Vector2 property with the given name,
    /// using <see cref="ReflectionTweenFactory"/>.
    /// </summary>
    /// <remarks>
    /// Since this uses reflection under the hood, the performance is significantly worse
    /// than defining the getter and setter yourself. If performance is a concern, you should
    /// use <see cref="Vector2TweenFactory{T}(System.Func{T,Vector2},System.Action{T,Vector2})"/> instead.
    /// </remarks>
    public static Vector2TweenFactory<T> From(string propertyName) {
        return ReflectionTweenFactory.Create<Vector2TweenFactory<T>>(propertyName);
    }
    
    public void SetComponent(ref Vector2 composite, Axis2 component, float value) => composite[(int)component] = value;
    public float GetComponent(Vector2 composite, Axis2 component) => composite[(int)component];

    public float Lerp(float from, float to, float t) => Mathf.LerpUnclamped(from, to, t);
}

/// <summary>
/// Components of a Vector2.
/// </summary>
public enum Axis2 {
    X, Y
}

}
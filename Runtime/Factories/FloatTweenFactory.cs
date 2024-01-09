using System;
using UnityEngine;

namespace FlowTween {
    
/// <summary>
/// A factory for creating tweens that animate float properties.
/// </summary>
/// <typeparam name="T">The type of the object that holds the property. Most commonly an <see cref="UnityEngine.Object"/>.</typeparam>
public class FloatTweenFactory<T> : TweenFactory<float, T> {
    public FloatTweenFactory(Func<T, float> getter, Action<T, float> setter) : base(getter, setter, Mathf.LerpUnclamped) { }
    
    /// <summary>
    /// Creates a factory that animates a float property with the given name,
    /// using <see cref="ReflectionTweenFactory"/>.
    /// </summary>
    /// <remarks>
    /// Since this uses reflection under the hood, the performance is significantly worse
    /// than defining the getter and setter yourself. If performance is a concern, you should
    /// use <see cref="FloatTweenFactory{T}(System.Func{T,float},System.Action{T,float})"/> instead.
    /// </remarks>
    public static FloatTweenFactory<T> Create(string propertyName) {
        return ReflectionTweenFactory.Create<FloatTweenFactory<T>>(propertyName);
    }
}

}
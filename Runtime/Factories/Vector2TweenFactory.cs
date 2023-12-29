using System;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// A factory for creating tweens that animate <see cref="Vector2"/> properties.
/// Also implements a composite interface for animating individual parts of
/// the <see cref="Vector2"/> by specifying an <see cref="Axis2"/>.
/// </summary>
/// <typeparam name="T">The type of the object that holds the property. Most commonly an <see cref="UnityEngine.Object"/>.</typeparam>
public class Vector2TweenFactory<T> : 
    TweenFactory<Vector2, T>,
    ICompositeTweenFactory<Vector2, T, float, Axis2> 
{
    public Vector2TweenFactory(Func<T, Vector2> getter, Action<T, Vector2> setter) 
        : base(getter, setter, Vector2.LerpUnclamped) { }

    public void SetPart(ref Vector2 composite, Axis2 part, float value) => composite[(int)part] = value;
    public float GetPart(Vector2 composite, Axis2 part) => composite[(int)part];

    public float Lerp(float from, float to, float t) => Mathf.LerpUnclamped(from, to, t);
}

/// <summary>
/// Parts of a Vector2.
/// </summary>
public enum Axis2 {
    X, Y
}

}
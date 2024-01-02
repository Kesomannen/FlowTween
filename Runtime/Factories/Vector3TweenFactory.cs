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

    public void SetPart(ref Vector3 composite, Axis part, float value) => composite[(int)part] = value;
    public float GetPart(Vector3 composite, Axis part) => composite[(int)part];

    public float Lerp(float from, float to, float t) => Mathf.LerpUnclamped(from, to, t);
    
    public ICompositeTweenFactory<Vector3, T, float, Axis> Composite => this;
}

public enum Axis {
    X, Y, Z
}

}
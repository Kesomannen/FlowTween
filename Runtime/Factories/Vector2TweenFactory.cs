using System;
using UnityEngine;

namespace FlowTween {

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

public enum Axis2 {
    X, Y
}

}
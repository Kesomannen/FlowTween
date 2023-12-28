using System;
using UnityEngine;

namespace FlowTween {

public class Vector3TweenFactory<T> : 
    TweenFactory<Vector3, T>,
    ICompositeTweenFactory<Vector3, T, float, Axis>
{
    public Vector3TweenFactory(Func<T, Vector3> getter, Action<T, Vector3> setter) 
        : base(getter, setter, LerpUtil.Lerp) { }

    public void SetPart(ref Vector3 composite, Axis part, float value) => composite[(int)part] = value;
    public float GetPart(Vector3 composite, Axis part) => composite[(int)part];

    public float Lerp(float from, float to, float t) => LerpUtil.Lerp(from, to, t);
}

public enum Axis {
    X, Y, Z
}

}
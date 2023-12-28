using System;
using UnityEngine;

namespace FlowTween {

public class QuaternionTweenFactory<T> : 
    TweenFactory<Quaternion, T>,
    ICompositeTweenFactory<Quaternion, T, float, Axis4>
{
    public QuaternionTweenFactory(Func<T, Quaternion> getter, Action<T, Quaternion> setter) 
        : base(getter, setter, LerpUtil.Lerp) { }

    public void SetPart(ref Quaternion composite, Axis4 part, float value) => composite[(int)part] = value;
    public float GetPart(Quaternion composite, Axis4 part) => composite[(int)part];
    
    public float Lerp(float from, float to, float t) => LerpUtil.Lerp(from, to, t);
}

public enum Axis4 {
    X, Y, Z, W
}

}
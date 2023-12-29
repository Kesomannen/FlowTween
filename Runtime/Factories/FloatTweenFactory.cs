using System;
using UnityEngine;

namespace FlowTween {
    
/// <summary>
/// A factory for creating tweens that animate float properties.
/// </summary>
/// <typeparam name="T">The type of the object that holds the property. Most commonly an <see cref="UnityEngine.Object"/>.</typeparam>
public class FloatTweenFactory<T> : TweenFactory<float, T> {
    public FloatTweenFactory(Func<T, float> getter, Action<T, float> setter) : base(getter, setter, Mathf.LerpUnclamped) { }
}

}
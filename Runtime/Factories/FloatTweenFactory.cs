using System;
using UnityEngine;

namespace FlowTween {

public class FloatTweenFactory<T> : TweenFactory<float, T> {
    public FloatTweenFactory(Func<T, float> getter, Action<T, float> setter) : base(getter, setter, Mathf.LerpUnclamped) { }
}

}
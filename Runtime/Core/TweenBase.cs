using System;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// Non-generic base class for <see cref="Tween{T}"/>.
/// </summary>
public abstract class TweenBase : Runnable {
    float _duration;

    public override float Duration => _duration;
    
    Func<float, float> _easeFunction;
    
    /// <summary>
    /// The tween's easing function. Input is the raw progress of the tween as it plays,
    /// between 0 and 1. The output can be outside of the 0-1 range, but it's recommended
    /// to keep it close. Should output 0 when the input is 0 and 1 when the input is 1.
    /// <br/><br/>Defaults to a linear function.
    /// </summary>
    public Func<float, float> EaseFunction {
        get => _easeFunction ?? Easings.Linear;
        set => _easeFunction = value;
    }

    /// <summary>
    /// Resets the tween to be recycled.
    /// <b>Only</b> use this if you're running the tween manually,
    /// otherwise the <see cref="TweenManager"/> will call this
    /// automatically.
    /// </summary>
    public override void Reset() {
        base.Reset();
        EaseFunction = null;
        _duration = 0;
    }

    protected override float GetProgress(float time) {
        return EaseFunction(base.GetProgress(time));
    }
    
    internal void SetDurationInternal(float duration) => _duration = duration;
}

}
using System;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// Non-generic base class for <see cref="Tween{T}"/>.
/// </summary>
public abstract class TweenBase : Runnable {
    float _duration;

    public override float Duration => _duration;

    /// <summary>
    /// The loop mode to use. See <see cref="LoopMode"/>.
    /// </summary>
    public LoopMode LoopMode { get; set; }
    
    Func<float, float> _easeFunction;
    
    /// <summary>
    /// The tween's easing function. Input is the raw progress of the tween,
    /// between 0 and 1. The output can be outside of the 0-1 range, but it's recommended
    /// to keep it close. Should output 0 when the input is 0 and 1 when the input is 1.
    /// <br/><br/>Defaults to a linear function.
    /// </summary>
    public Func<float, float> EaseFunction {
        get => _easeFunction ?? Easing.Linear;
        set => _easeFunction = value;
    }

    /// <summary>
    /// Has this tween cancelled or run for the full duration?
    /// (only applicable when <see cref="LoopMode"/> is <see cref="LoopMode.None"/>).
    /// </summary>
    public override bool IsComplete => IsCancelled || LoopMode == LoopMode.None && _time >= TotalDuration;

    protected TweenBase() {
        EaseFunction = x => x;
    }

    public abstract void Reverse();

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
        var rawProgress = base.GetProgress(time);
        return EaseFunction(LoopMode switch {
            LoopMode.None => rawProgress,
            LoopMode.Loop => rawProgress % 1,
            LoopMode.PingPong => Mathf.PingPong(rawProgress, 1),
            _ => throw new ArgumentOutOfRangeException()
        });
    }
    
    internal void SetDurationInternal(float duration) => _duration = duration;
}

}
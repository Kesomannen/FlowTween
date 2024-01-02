using System;
using System.Collections;
using UnityEngine;

namespace FlowTween {
    
/// <summary>
/// The base class for all tween-like objects.
/// 
/// <br/><br/>Any subclass of this can be ran at runtime and automatically pooled
/// by the <see cref="TweenManager"/> (see <see cref="TweenManager.Get"/>).
/// You can also run them manually by not adding them to the manager
/// and calling <see cref="Update"/> yourself. For example, the editor tween preview
/// runs tweens using the editor update loop.
///
/// <br/><br/>When running manually at runtime, you have to create tweens
/// with the <c>new</c> keyword, as all of the extension methods for creating
/// tweens (like <c>transform.TweenPosition(...)</c>) automatically add them
/// to the <see cref="TweenManager"/>.
/// 
/// <br/><br/>You can define your own sub-classes for advanced use cases,
/// but usually you'll want to use <see cref="Tween{T}"/> and it's
/// associated methods instead.
/// </summary>
public abstract class TweenBase : Runnable {
    public new float Duration { get; set; }

    /// <summary>
    /// The loop mode to use. See <see cref="LoopMode"/>.
    /// </summary>
    public LoopMode LoopMode { get; set; }
    
    Func<float, float> _easeFunction;
    
    /// <summary>
    /// The tween's easing function. Input is the <see cref="TweenBase.RawProgress"/>,
    /// between 0 and 1. The output can be outside of the 0-1 range, but it's recommended
    /// to keep it close. Should output 0 when the input is 0 and 1 when the input is 1.
    /// <br/><br/>Defaults to a linear function.
    /// </summary>
    public Func<float, float> EaseFunction {
        get => _easeFunction ?? Easing.Linear;
        set => _easeFunction = value;
    }

    /// <summary>
    /// Has this tween cancelled or run for the full duration
    /// (only applicable when <see cref="LoopMode"/> is <see cref="LoopMode.None"/>).
    /// </summary>
    public override bool IsComplete => base.IsComplete || LoopMode == LoopMode.None && _time >= Duration;

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
        EaseFunction = null;
        Duration = 0;
    }

    protected override float GetProgress(float time) {
        var progress = time / Duration;
        return EaseFunction(LoopMode switch {
            LoopMode.None => progress,
            LoopMode.Loop => progress % 1,
            LoopMode.PingPong => Mathf.PingPong(progress, 1),
            _ => throw new ArgumentOutOfRangeException()
        });
    }
}

}
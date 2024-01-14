using System;
using System.Collections;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// Base class for tweens and sequences. Can be awaited in coroutines with <c>yield</c>.
///
/// <br/><br/>Any subclass of this can be ran at runtime and automatically pooled
/// by the <see cref="TweenManager"/> (see <see cref="TweenManager.Get{T}"/>).
/// You can also run them manually by calling <see cref="Update"/> yourself.
/// For example, the editor tween preview runs tweens using the editor update loop.
///
/// <br/><br/>When running manually at runtime, you can uncheck the "Enabled" checkbox in the
/// <see cref="TweenManager"/> to disable it completely. If you want to run some runnables yourself
/// and some automatically, you have create them with the <c>new</c> keyword, as all of the
/// extension methods for creating tweens and sequences (like <c>transform.TweenPosition(...)</c>)
/// automatically add them to the <see cref="TweenManager"/> (if it's enabled).
/// 
/// <br/><br/>You can also define your own sub-classes for advanced use cases.
/// </summary>
public abstract class Runnable : IEnumerator {
    /// <summary>
    /// Has this runnable been cancelled?
    /// </summary>
    public bool IsCancelled { get; set; }
    
    /// <summary>
    /// Is this runnable paused?
    /// </summary>
    public bool IsPaused { get; set; }
    
    /// <summary>
    /// An action to invoke when this runnable completes or is cancelled.
    /// When cancelled through <see cref="TweenManager"/>, <c>callOnComplete</c>
    /// has to be true to invoke this.
    /// </summary>
    public Action CompleteAction { get; set; }
    
    /// <summary>
    /// The <see cref="LoopMode"/> of this runnable.
    /// </summary>
    public LoopMode LoopMode { get; set; }
    
    /// <summary>
    /// The number of times this runnable should loop. Set to null to loop infinitely.
    /// Not applicable if <see cref="LoopMode"/> is <see cref="LoopMode.None"/>.
    /// </summary>
    public int? Loops { get; set; }
    
    /// <summary>
    /// The duration of this runnable, in seconds.
    /// Note that this does not include any <see cref="Delay"/> before the runnable starts
    /// or the <see cref="LoopMode"/> the runnable might use.
    /// </summary>
    /// <seealso cref="TotalDuration"/>
    public virtual float Duration { get; }

    /// <summary>
    /// The total duration of this runnable (including <see cref="Delay"/> and <see cref="Loops"/>), in seconds.
    /// </summary>
    public float TotalDuration => Duration * (LoopMode == LoopMode.None || !Loops.HasValue ? 1 : Loops.Value) + Delay;
    
    /// <summary>
    /// The delay before this runnable starts, in seconds.
    /// </summary>
    public float Delay { get; set; }

    float _time;

    /// <summary>
    /// The progress of this runnable, usually between 0 and 1
    /// or close to that, unless you're using a custom easing function.
    /// Also known as the t-value.
    /// </summary>
    public float Progress => GetProgress(_time);
    
    /// <summary>
    /// Should this runnable ignore <see cref="Time.timeScale"/>?
    /// </summary>
    /// <remarks>
    /// This is not handled the runnable itself, but by the runner
    /// (at runtime, usually the <see cref="TweenManager"/>).
    /// If you're running this manually, you might want to take
    /// this property into account.
    /// </remarks>
    public bool IgnoreTimescale { get; set; }

    /// <summary>
    /// Has this runnable been cancelled or run for the full duration?
    /// </summary>
    public virtual bool IsComplete {
        get {
            if (IsCancelled) return true;
            if (LoopMode == LoopMode.None) return _time >= Duration + Delay;
            return Loops.HasValue && _time >= Duration * Loops.Value + Delay;
        }
    }

    /// <summary>
    /// Updates the runnnable.
    /// Only use this if you're running this manually, otherwise it's
    /// called automatically by the <see cref="TweenManager"/> every frame.
    /// </summary>
    /// <param name="deltaTime">The time since the last update.</param>
    public void Update(float deltaTime) {
        if (IsPaused) return;
        
        _time += deltaTime;
        
        if (_time < Delay) return;
        OnUpdate(deltaTime);
    }

    /// <summary>
    /// Cancels the runnable.
    /// <b>Only</b> use this if you're running manually,
    /// otherwise use <see cref="TweenManager.Cancel(Runnable, bool)"/>
    /// on the <see cref="TweenManager"/>.
    /// </summary>
    /// <param name="safe">
    /// Whether or not it's completely safe to still run Update.
    /// </param>
    public virtual void Cancel(bool safe) {
        IsCancelled = true;
        CompleteAction?.Invoke();
    }
    
    protected abstract void OnUpdate(float deltaTime);

    /// <summary>
    /// Resets the object to be recycled.
    /// <b>Only</b> use this if you're running manually,
    /// otherwise the <see cref="TweenManager"/> will call this
    /// automatically.
    /// </summary>
    public virtual void Reset() {
        CompleteAction = null;
        _time = 0;
        IsCancelled = false;
        IsPaused = false;
    }
    
    protected virtual float GetProgress(float time) {
        if (Duration == 0) {
            Debug.LogWarning("Tween duration is 0! Please ensure it is greater than 0 before the runnable starts running.");
            return 0;
        }
        
        var rawProgress = Mathf.Max(time - Delay, 0) / Duration;
        return LoopMode switch {
            LoopMode.None => rawProgress,
            LoopMode.Loop => rawProgress % 1,
            LoopMode.PingPong => Mathf.PingPong(rawProgress, 1),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    object IEnumerator.Current => null;
    bool IEnumerator.MoveNext() => !IsComplete;
    void IEnumerator.Reset() { }
}

}
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
public abstract class TweenBase : IEnumerator {
    /// <summary>
    /// Has this been cancelled?
    /// </summary>
    public bool IsCancelled { get; set; }
    
    /// <summary>
    /// Is this tween paused?
    /// </summary>
    public bool IsPaused { get; set; }

    /// <summary>
    /// The loop mode to use. See <see cref="LoopMode"/>.
    /// </summary>
    public LoopMode LoopMode { get; set; }
    
    /// <summary>
    /// An action to invoke when this tween completes or is cancelled.
    /// When cancelled through <see cref="TweenManager"/>, <c>callOnComplete</c>
    /// has to be true to invoke this.
    /// </summary>
    public Action CompleteAction { get; set; }
    
    /// <summary>
    /// The duration of this tween in seconds.
    /// </summary>
    public virtual float Duration { get; set; }

    float _time;
    Func<float, float> _easeFunction;

    /// <summary>
    /// The eased progress of this tween, usually between 0 and 1
    /// or close to that, unless you're using a custom easing function.
    /// Also known as the t-value.
    /// </summary>
    /// <seealso cref="RawProgress"/>
    public float Progress => EaseFunction(RawProgress);
    
    /// <summary>
    /// The tween's easing function. Input is the <see cref="TweenBase.RawProgress"/>,
    /// between 0 and 1. The output can be outside of the 0-1 range, but it's recommended
    /// to keep it close. Should output 0 when the input is 0 and 1 when the input is 1.
    /// <br/><br/>Defaults to a linear function.
    /// </summary>
    /// <exception cref="ArgumentNullException">value is null</exception>
    public Func<float, float> EaseFunction {
        get => _easeFunction;
        set => _easeFunction = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The uneased raw progress of this tween, between 0 and 1.
    /// </summary>
    /// <seealso cref="Progress"/>
    public float RawProgress {
        get {
            var progress = _time / Duration;
            return LoopMode switch {
                LoopMode.None => progress,
                LoopMode.Loop => progress % 1,
                LoopMode.PingPong => Mathf.PingPong(progress, 1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    /// <summary>
    /// Should this tween ignore <see cref="Time.timeScale"/>?
    /// </summary>
    /// <remarks>
    /// This is not used by the tween itself, but by the runner
    /// of the tween (at runtime, usually the <see cref="TweenManager"/>).
    /// If you're running a tween manually, youmight want to take
    /// this property into account.
    /// </remarks>
    public bool IgnoreTimescale { get; set; }

    /// <summary>
    /// Has this tween cancelled or run for the full duration
    /// (only applicable when <see cref="LoopMode"/> is <see cref="LoopMode.None"/>).
    /// </summary>
    public bool IsComplete => IsCancelled || LoopMode == LoopMode.None && _time >= Duration;

    protected TweenBase() {
        EaseFunction = x => x;
    }
    
    /// <summary>
    /// Updates the tween.
    /// Only use this if you're running the tween manually,
    /// otherwise it's called automatically by the <see cref="TweenManager"/>
    /// every frame.
    /// </summary>
    /// <param name="deltaTime">The time since the last update.</param>
    public void Update(float deltaTime) {
        if (IsPaused) return;
        
        _time += deltaTime;
        OnUpdate(deltaTime);
    }

    /// <summary>
    /// Cancels the tween.
    /// <b>Only</b> use this if you're running the tween manually,
    /// otherwise use <see cref="TweenManager.Cancel(TweenBase, bool)"/>
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
    
    public abstract void Reverse();

    /// <summary>
    /// Resets the tween to be recycled.
    /// <b>Only</b> use this if you're running the tween manually,
    /// otherwise the <see cref="TweenManager"/> will call this
    /// automatically.
    /// </summary>
    public virtual void Reset() {
        CompleteAction = null;
        _time = 0;
        EaseFunction = x => x;
        IsCancelled = false;
        IsPaused = false;
        Duration = 0;
    }

    object IEnumerator.Current => null;
    bool IEnumerator.MoveNext() => !IsComplete;
    void IEnumerator.Reset() { }
}

}
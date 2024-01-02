using System;
using System.Collections;
using UnityEngine;

namespace FlowTween {

public abstract class Runnable : IEnumerator {
    /// <summary>
    /// Has this been cancelled?
    /// </summary>
    public bool IsCancelled { get; set; }
    
    /// <summary>
    /// Is this tween paused?
    /// </summary>
    public bool IsPaused { get; set; }
    
    /// <summary>
    /// An action to invoke when this tween completes or is cancelled.
    /// When cancelled through <see cref="TweenManager"/>, <c>callOnComplete</c>
    /// has to be true to invoke this.
    /// </summary>
    public Action CompleteAction { get; set; }
    
    /// <summary>
    /// The duration of this tween in seconds.
    /// </summary>
    public virtual float Duration { get; }
    
    public float Delay { get; set; }

    protected float _time;

    /// <summary>
    /// The eased progress of this tween, usually between 0 and 1
    /// or close to that, unless you're using a custom easing function.
    /// Also known as the t-value.
    /// </summary>
    public float Progress => GetProgress(_time);
    
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
    public virtual bool IsComplete => IsCancelled;
    
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
        
        if (_time < Delay) return;
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
        return Mathf.Max(time - Delay, 0) / Duration;
    }
    
    object IEnumerator.Current => null;
    bool IEnumerator.MoveNext() => !IsComplete;
    void IEnumerator.Reset() { }
}

}
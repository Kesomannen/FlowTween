﻿using System;
using System.Collections;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// The base class for all tween-like objects.
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
/// <br/><br/>You can define your own sub-classes for advanced use cases,
/// but usually you'll want to use <see cref="Tween{T}"/> or <see cref="Sequence"/> and their
/// associated methods instead.
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
    /// The duration of this runnable, in seconds.
    /// Note that this does not include any <see cref="Delay"/> before the runnable starts.
    /// </summary>
    /// <seealso cref="TotalDuration"/>
    public virtual float Duration { get; }
    
    /// <summary>
    /// The total duration of this runnable (including <see cref="Delay"/>), in seconds.
    /// </summary>
    public float TotalDuration => Duration + Delay;
    
    /// <summary>
    /// The delay before this runnable starts, in seconds.
    /// </summary>
    public float Delay { get; set; }

    protected float _time;

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
    public virtual bool IsComplete => IsCancelled || _time >= TotalDuration;
    
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
        
        return Mathf.Max(time - Delay, 0) / Duration;
    }
    
    object IEnumerator.Current => null;
    bool IEnumerator.MoveNext() => !IsComplete;
    void IEnumerator.Reset() { }
}

}
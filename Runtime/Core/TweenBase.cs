using System;
using System.Collections;
using UnityEngine;

namespace FlowTween {

public abstract class TweenBase : IEnumerator {
    public bool IsCancelled { get; set; }
    public bool IsPaused { get; set; }

    public LoopMode LoopMode { get; set; }
    public Action CompleteAction { get; set; }
    public virtual float Duration { get; set; }

    float _time;
    Func<float, float> _easeFunction;

    public float Progress => EaseFunction(RawProgress);
    
    public Func<float, float> EaseFunction {
        get => _easeFunction;
        set => _easeFunction = value ?? throw new ArgumentNullException(nameof(value));
    }

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
    
    public bool IgnoreTimescale { get; set; }

    public bool IsComplete => IsCancelled || (LoopMode == LoopMode.None && _time >= Duration);

    protected TweenBase() {
        EaseFunction = x => x;
    }
    
    public void Update(float deltaTime) {
        if (IsPaused) return;
        
        _time += deltaTime;
        OnUpdate(deltaTime);
    }

    public virtual void OnComplete(bool cancelled) {
        IsCancelled = true;
        CompleteAction?.Invoke();
    }
    
    protected abstract void OnUpdate(float deltaTime);
    
    public abstract void Reverse();

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
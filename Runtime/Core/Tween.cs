using System;

namespace FlowTween {

public class Tween<T> : TweenBase {
    public T Start { get; set; }
    public T End { get; set; }

    public Action<T> UpdateAction { get; set; }
    public Func<T, T, float, T> LerpFunction { get; set; }

    public T Value => LerpFunction(Start, End, Progress);
    
    protected override void OnUpdate(float deltaTime) {
        UpdateAction?.Invoke(Value);
    }

    public override void OnComplete(bool cancelled) {
        base.OnComplete(cancelled);
        if (!cancelled) {
            UpdateAction?.Invoke(End);
        }
    }

    public override void Reverse() {
        (Start, End) = (End, Start);
    }

    public override void Reset() {
        base.Reset();
        UpdateAction = null;
        Start = default;
        End = default;
    }
    
    public Tween<T> OnUpdate(Action<T> onUpdate) {
        UpdateAction = onUpdate;
        return this;
    }
    
    public Tween<T> From(T start) {
        Start = start;
        return this;
    }
    
    public Tween<T> To(T end) {
        End = end;
        return this;
    }

    public Tween<T> Lerp(Func<T, T, float, T> lerp) {
        LerpFunction = lerp;
        return this;
    }
}

}
using System;

namespace FlowTween {

/// <summary>
/// The main tween class.
/// </summary>
public class Tween<T> : TweenBase {
    /// <summary>
    /// Start value of this tween.
    /// </summary>
    public T Start { get; set; }
    
    /// <summary>
    /// End value of this tween.
    /// </summary>
    public T End { get; set; }

    /// <summary>
    /// Invoked every update with the current value.
    /// </summary>
    public Action<T> UpdateAction { get; set; }
    
    /// <summary>
    /// Function to linearly interpolate between <see cref="Start"/> and <see cref="End"/>.
    /// Make sure this isn't null when the tween starts (usually the next frame).
    /// </summary>
    public LerpFunction<T> LerpFunction { get; set; }
    
    /// <summary>
    /// The current value of the tween.
    /// Usually between <see cref="Start"/> and <see cref="End"/>,
    /// but custom easings can make this go outside of that range.
    /// </summary>
    public T Value => LerpFunction(Start, End, Progress);
    
    protected override void OnUpdate(float deltaTime) {
        UpdateAction?.Invoke(Value);
    }

    public override void Cancel(bool safe) {
        base.Cancel(safe);
        if (!safe) {
            UpdateAction?.Invoke(End);
        }
    }

    /// <summary>
    /// Reverses the start and end values.
    /// </summary>
    public override void Reverse() {
        (Start, End) = (End, Start);
    }

    public override void Reset() {
        base.Reset();
        UpdateAction = null;
        Start = default;
        End = default;
    }
    
    /// <summary>
    /// Adds an action to <see cref="UpdateAction"/>.
    /// </summary>
    /// <param name="onUpdate">The action to invoke every update with the current value.</param>
    public Tween<T> OnUpdate(Action<T> onUpdate) {
        UpdateAction = onUpdate;
        return this;
    }
    
    /// <summary>
    /// Sets the start value of this tween.
    /// </summary>
    public Tween<T> From(T start) {
        Start = start;
        return this;
    }
    
    /// <summary>
    /// Sets the end value of this tween.
    /// </summary>
    public Tween<T> To(T end) {
        End = end;
        return this;
    }

    /// <summary>
    /// Sets <see cref="LerpFunction"/>.
    /// </summary>
    public Tween<T> Lerp(LerpFunction<T> lerp) {
        LerpFunction = lerp;
        return this;
    }
}

/// <summary>
/// A function that linearly interpolates between two values.
/// </summary>
public delegate T LerpFunction<T>(T from, T to, float t);

}
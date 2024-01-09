using System;
using FlowTween.Templates;
using UnityEngine;

namespace FlowTween {
    
/// <summary>
/// Provides extension methods shared by all <see cref="TweenBase"/> subclasses,
/// mostly for setting properties in a builder-like fashion.
/// </summary>
public static class TweenBaseExtensions {
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.EaseFunction"/>.
    /// </summary>
    public static T Ease<T>(this T tween, Func<float, float> ease) where T : TweenBase {
        tween.EaseFunction = ease;
        return tween;
    }
    
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.EaseFunction"/> to a function
    /// represented by the given <see cref="Ease"/>.
    /// </summary>
    public static T Ease<T>(this T tween, Ease ease) where T : TweenBase {
        return tween.Ease(ease.GetFunction());
    }
    
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.EaseFunction"/> to a function
    /// represented by the given <see cref="EaseType"/> and <see cref="EaseDirection"/>.
    /// </summary>
    public static T Ease<T>(this T tween, EaseType type, EaseDirection direction) where T : TweenBase {
        return tween.Ease(new Ease {
            Type = type,
            Direction = direction
        });
    }
    
    /// <summary>
    /// Sets the tween to ease in with the given <see cref="EaseType"/>.
    /// </summary>
    public static T EaseIn<T>(this T tween, EaseType type) where T : TweenBase {
        return tween.Ease(type, EaseDirection.In);
    }
    
    /// <summary>
    /// Sets the tween to ease out with the given <see cref="EaseType"/>.
    /// </summary>
    public static T EaseOut<T>(this T tween, EaseType type) where T : TweenBase {
        return tween.Ease(type, EaseDirection.Out);
    }
    
    /// <summary>
    /// Sets the tween to ease in and out with the given <see cref="EaseType"/>.
    /// </summary>
    public static T EaseInOut<T>(this T tween, EaseType type) where T : TweenBase {
        return tween.Ease(type, EaseDirection.InOut);
    }
    
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.EaseFunction"/> to follow an <see cref="AnimationCurve"/>.
    /// The tween starts at <c>time=0</c> and ends at <c>time=1</c>.
    /// The curve's value is unconstrained, but it's recommended to keep it close to the 0-1 range.
    /// </summary>
    public static T Ease<T>(this T tween, AnimationCurve curve) where T : TweenBase {
        tween.EaseFunction = curve.Evaluate;
        return tween;
    }
    
    /// <summary>
    /// Applies the given <see cref="TweenSettings"/> to the tween.
    /// </summary>
    /// <seealso cref="TweenSettings.Apply"/>
    public static T Apply<T>(this T tween, TweenSettings settings) where T : TweenBase {
        settings.Apply(tween);
        return tween;
    }

    /// <summary>
    /// Sets the tween's <see cref="TweenBase.LoopMode"/>.
    /// </summary>
    public static T Loop<T>(this T tween, LoopMode mode = LoopMode.Loop) where T : TweenBase {
        tween.LoopMode = mode;
        return tween;
    }
    
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.Duration"/>.
    /// </summary>
    public static T SetDuration<T>(this T tween, float duration) where T : TweenBase {
        tween.SetDurationInternal(duration);
        return tween;
    }
    
    public static T SetDelay<T>(this T tween, float delay) where T : TweenBase {
        tween.Delay = delay;
        return tween;
    }
    
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.IgnoreTimescale"/>.
    /// </summary>
    public static T SetIgnoreTimescale<T>(this T tween, bool ignoreTimeScale = true) where T : TweenBase {
        tween.IgnoreTimescale = ignoreTimeScale;
        return tween;
    }
}

}
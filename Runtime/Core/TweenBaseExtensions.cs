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
    /// <param name="ease">
    /// The easing function. Input is the <see cref="TweenBase.RawProgress"/> of the tween,
    /// between 0 and 1. The output can be outside of the 0-1 range, but it's recommended
    /// to keep it close. The function should output 0 when the input is 0 and 1 when the input is 1.
    /// </param>
    public static T Ease<T>(this T tween, Func<float, float> ease) where T : TweenBase {
        tween.EaseFunction = ease;
        return tween;
    }
    
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.EaseFunction"/> to a function
    /// representing the given <see cref="EaseType"/>.
    /// </summary>
    public static T Ease<T>(this T tween, EaseType ease) where T : TweenBase {
        tween.EaseFunction = EaseUtil.GetFunction(ease);
        return tween;
    }
    
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.EaseFunction"/> to follow an <see cref="AnimationCurve"/>.
    /// The tween starts at <c>time=0</c> and ends at <c>time=1</c>.
    /// The curve's value is unconstrained, but it's recommended to keep it close to the 0-1 range.
    /// </summary>
    public static T Ease<T>(this T tween, AnimationCurve ease) where T : TweenBase {
        tween.EaseFunction = ease.Evaluate;
        return tween;
    }
    
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.CompleteAction"/>.
    /// </summary>
    public static T OnComplete<T>(this T runnable, Action action) where T : Runnable {
        runnable.CompleteAction += action;
        return runnable;
    }
    
    /// <summary>
    /// Applies the given <see cref="TweenSettings"/> to the tween.
    /// </summary>
    /// <seealso cref="TweenSettings.Apply"/>
    public static T Apply<T>(this T tween, TweenSettings settings) where T : TweenBase {
        settings.Apply(tween);
        return tween;
    }
    
    /// <inheritdoc cref="Apply{T}(T, TweenSettings)"/>
    public static T Apply<T>(this T tween, TweenSettingsProperty settings) where T : TweenBase {
        return tween.Apply(settings.Value);
    }
    
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.LoopMode"/>.
    /// </summary>
    public static T Loop<T>(this T tween, LoopMode mode = LoopMode.Loop) where T : TweenBase {
        tween.LoopMode = mode;
        return tween;
    }
    
    /// <summary>
    /// Pauses the tween.
    /// </summary>
    /// <seealso cref="Runnable.IsPaused"/>
    public static T Pause<T>(this T tween) where T : Runnable {
        tween.IsPaused = true;
        return tween;
    }
    
    /// <summary>
    /// Resumes the tween.
    /// </summary>
    /// <seealso cref="Runnable.IsPaused"/>
    public static T Resume<T>(this T tween) where T : Runnable {
        tween.IsPaused = false;
        return tween;
    }
    
    /// <summary>
    /// Sets the tween's <see cref="TweenBase.Duration"/>.
    /// </summary>
    public static T SetDuration<T>(this T tween, float duration) where T : TweenBase {
        tween.Duration = duration;
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
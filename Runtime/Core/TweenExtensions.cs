using System;
using UnityEngine;

namespace FlowTween {

public static class TweenExtensions {
    public static T Ease<T>(this T tween, Func<float, float> ease) where T : TweenBase {
        tween.EaseFunction = ease;
        return tween;
    }
    
    public static T Ease<T>(this T tween, PresetEaseType ease) where T : TweenBase {
        tween.EaseFunction = PresetEaseUtil.GetFunction(ease);
        return tween;
    }
    
    public static T Ease<T>(this T tween, AnimationCurve ease) where T : TweenBase {
        tween.EaseFunction = ease.Evaluate;
        return tween;
    }
    
    public static T OnComplete<T>(this T tween, Action action) where T : TweenBase {
        tween.CompleteAction += action;
        return tween;
    }
    
    public static T Apply<T>(this T tween, TweenSettings settings) where T : TweenBase {
        settings.Apply(tween);
        return tween;
    }
    
    public static T Loop<T>(this T tween, LoopMode mode = LoopMode.Loop) where T : TweenBase {
        tween.LoopMode = mode;
        return tween;
    }
    
    public static T Pause<T>(this T tween) where T : TweenBase {
        tween.IsPaused = true;
        return tween;
    }
    
    public static T Resume<T>(this T tween) where T : TweenBase {
        tween.IsPaused = false;
        return tween;
    }
    
    public static T SetDuration<T>(this T tween, float duration) where T : TweenBase {
        tween.Duration = duration;
        return tween;
    }

    public static T SetIgnoreTimescale<T>(this T tween, bool ignoreTimeScale = true) where T : TweenBase {
        tween.IgnoreTimescale = ignoreTimeScale;
        return tween;
    }
}

}
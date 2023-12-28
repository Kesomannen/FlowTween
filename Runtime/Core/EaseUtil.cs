using System;

namespace FlowTween {

public static class EaseUtil {
    /// <summary>
    /// Gets the corresponding easing function for a preset ease type.
    /// </summary>
    public static Func<float, float> GetFunction(EaseType presetEase) {
        return presetEase switch {
            EaseType.Linear => Easing.Linear,
            EaseType.SineIn => Easing.SineIn,
            EaseType.SineOut => Easing.SineOut,
            EaseType.SineInOut => Easing.SineInOut,
            EaseType.QuadIn => Easing.QuadIn,
            EaseType.QuadOut => Easing.QuadOut,
            EaseType.QuadInOut => Easing.QuadInOut,
            EaseType.CubicIn => Easing.CubicIn,
            EaseType.CubicOut => Easing.CubicOut,
            EaseType.CubicInOut => Easing.CubicInOut,
            EaseType.QuartIn => Easing.QuartIn,
            EaseType.QuartOut => Easing.QuartOut,
            EaseType.QuartInOut => Easing.QuartInOut,
            EaseType.QuintIn => Easing.QuintIn,
            EaseType.QuintOut => Easing.QuintOut,
            EaseType.QuintInOut => Easing.QuintInOut,
            EaseType.ExpoIn => Easing.ExpoIn,
            EaseType.ExpoOut => Easing.ExpoOut,
            EaseType.ExpoInOut => Easing.ExpoInOut,
            EaseType.CircIn => Easing.CircIn,
            EaseType.CircOut => Easing.CircOut,
            EaseType.CircInOut => Easing.CircInOut,
            EaseType.BackIn => Easing.BackIn,
            EaseType.BackOut => Easing.BackOut,
            EaseType.BackInOut => Easing.BackInOut,
            EaseType.ElasticIn => Easing.ElasticIn,
            EaseType.ElasticOut => Easing.ElasticOut,
            EaseType.ElasticInOut => Easing.ElasticInOut,
            EaseType.BounceIn => Easing.BounceIn,
            EaseType.BounceOut => Easing.BounceOut,
            EaseType.BounceInOut => Easing.BounceInOut,
            _ => throw new ArgumentOutOfRangeException(nameof(presetEase), presetEase, null)
        };
    }
}

}
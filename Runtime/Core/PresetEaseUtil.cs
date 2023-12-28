using System;

namespace FlowTween {

internal static class PresetEaseUtil {
    public static Func<float, float> GetFunction(PresetEaseType presetEase) {
        return presetEase switch {
            PresetEaseType.Linear => Easing.Linear,
            PresetEaseType.SineIn => Easing.SineIn,
            PresetEaseType.SineOut => Easing.SineOut,
            PresetEaseType.SineInOut => Easing.SineInOut,
            PresetEaseType.QuadIn => Easing.QuadIn,
            PresetEaseType.QuadOut => Easing.QuadOut,
            PresetEaseType.QuadInOut => Easing.QuadInOut,
            PresetEaseType.CubicIn => Easing.CubicIn,
            PresetEaseType.CubicOut => Easing.CubicOut,
            PresetEaseType.CubicInOut => Easing.CubicInOut,
            PresetEaseType.QuartIn => Easing.QuartIn,
            PresetEaseType.QuartOut => Easing.QuartOut,
            PresetEaseType.QuartInOut => Easing.QuartInOut,
            PresetEaseType.QuintIn => Easing.QuintIn,
            PresetEaseType.QuintOut => Easing.QuintOut,
            PresetEaseType.QuintInOut => Easing.QuintInOut,
            PresetEaseType.ExpoIn => Easing.ExpoIn,
            PresetEaseType.ExpoOut => Easing.ExpoOut,
            PresetEaseType.ExpoInOut => Easing.ExpoInOut,
            PresetEaseType.CircIn => Easing.CircIn,
            PresetEaseType.CircOut => Easing.CircOut,
            PresetEaseType.CircInOut => Easing.CircInOut,
            PresetEaseType.BackIn => Easing.BackIn,
            PresetEaseType.BackOut => Easing.BackOut,
            PresetEaseType.BackInOut => Easing.BackInOut,
            PresetEaseType.ElasticIn => Easing.ElasticIn,
            PresetEaseType.ElasticOut => Easing.ElasticOut,
            PresetEaseType.ElasticInOut => Easing.ElasticInOut,
            PresetEaseType.BounceIn => Easing.BounceIn,
            PresetEaseType.BounceOut => Easing.BounceOut,
            PresetEaseType.BounceInOut => Easing.BounceInOut,
            _ => throw new ArgumentOutOfRangeException(nameof(presetEase), presetEase, null)
        };
    }
}

}
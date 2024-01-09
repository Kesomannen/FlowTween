using System;

namespace FlowTween {

/// <summary>
/// Specifies a preset easing function.
/// You can access the functions directly in <see cref="Easing"/>.
/// </summary>
[Serializable]
public struct Ease {
    /// <summary>
    /// The type of easing function.
    /// </summary>
    public EaseType Type;
    
    /// <summary>
    /// The direction(s) of the easing function.
    /// </summary>
    public EaseDirection Direction;

    public Ease(EaseType type, EaseDirection direction) {
        Type = type;
        Direction = direction;
    }
    
    public static Ease In(EaseType type) => new Ease(type, EaseDirection.In);
    public static Ease Out(EaseType type) => new Ease(type, EaseDirection.Out);
    public static Ease InOut(EaseType type) => new Ease(type, EaseDirection.InOut);

    /// <summary>
    /// Gets an easing function based on the current <see cref="Type"/> and <see cref="Direction"/>.
    /// You can access them directly in <see cref="Easing"/>.
    /// </summary>
    public Func<float, float> GetFunction() {
        return Type switch {
            EaseType.Linear => Easing.Linear,
            EaseType.Sine => Direction switch {
                EaseDirection.In => Easing.SineIn,
                EaseDirection.Out => Easing.SineOut,
                EaseDirection.InOut => Easing.SineInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Quad => Direction switch {
                EaseDirection.In => Easing.QuadIn,
                EaseDirection.Out => Easing.QuadOut,
                EaseDirection.InOut => Easing.QuadInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Cubic => Direction switch {
                EaseDirection.In => Easing.CubicIn,
                EaseDirection.Out => Easing.CubicOut,
                EaseDirection.InOut => Easing.CubicInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Quart => Direction switch {
                EaseDirection.In => Easing.QuartIn,
                EaseDirection.Out => Easing.QuartOut,
                EaseDirection.InOut => Easing.QuartInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Quint => Direction switch {
                EaseDirection.In => Easing.QuintIn,
                EaseDirection.Out => Easing.QuintOut,
                EaseDirection.InOut => Easing.QuintInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Expo => Direction switch {
                EaseDirection.In => Easing.ExpoIn,
                EaseDirection.Out => Easing.ExpoOut,
                EaseDirection.InOut => Easing.ExpoInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Circ => Direction switch {
                EaseDirection.In => Easing.CircIn,
                EaseDirection.Out => Easing.CircOut,
                EaseDirection.InOut => Easing.CircInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Back => Direction switch {
                EaseDirection.In => Easing.BackIn,
                EaseDirection.Out => Easing.BackOut,
                EaseDirection.InOut => Easing.BackInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Elastic => Direction switch {
                EaseDirection.In => Easing.ElasticIn,
                EaseDirection.Out => Easing.ElasticOut,
                EaseDirection.InOut => Easing.ElasticInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Bounce => Direction switch {
                EaseDirection.In => Easing.BounceIn,
                EaseDirection.Out => Easing.BounceOut,
                EaseDirection.InOut => Easing.BounceInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

}
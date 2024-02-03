using System;

namespace FlowTween {

/// <summary>
/// Specifies a preset easing function.
/// You can access the functions directly in <see cref="Easings"/>.
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
    /// You can access them directly in <see cref="Easings"/>.
    /// </summary>
    public Func<float, float> GetFunction() {
        return Type switch {
            EaseType.Linear => Easings.Linear,
            EaseType.Sine => Direction switch {
                EaseDirection.In => Easings.SineIn,
                EaseDirection.Out => Easings.SineOut,
                EaseDirection.InOut => Easings.SineInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Quad => Direction switch {
                EaseDirection.In => Easings.QuadIn,
                EaseDirection.Out => Easings.QuadOut,
                EaseDirection.InOut => Easings.QuadInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Cubic => Direction switch {
                EaseDirection.In => Easings.CubicIn,
                EaseDirection.Out => Easings.CubicOut,
                EaseDirection.InOut => Easings.CubicInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Quart => Direction switch {
                EaseDirection.In => Easings.QuartIn,
                EaseDirection.Out => Easings.QuartOut,
                EaseDirection.InOut => Easings.QuartInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Quint => Direction switch {
                EaseDirection.In => Easings.QuintIn,
                EaseDirection.Out => Easings.QuintOut,
                EaseDirection.InOut => Easings.QuintInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Expo => Direction switch {
                EaseDirection.In => Easings.ExpoIn,
                EaseDirection.Out => Easings.ExpoOut,
                EaseDirection.InOut => Easings.ExpoInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Circ => Direction switch {
                EaseDirection.In => Easings.CircIn,
                EaseDirection.Out => Easings.CircOut,
                EaseDirection.InOut => Easings.CircInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Back => Direction switch {
                EaseDirection.In => Easings.BackIn,
                EaseDirection.Out => Easings.BackOut,
                EaseDirection.InOut => Easings.BackInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Elastic => Direction switch {
                EaseDirection.In => Easings.ElasticIn,
                EaseDirection.Out => Easings.ElasticOut,
                EaseDirection.InOut => Easings.ElasticInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            EaseType.Bounce => Direction switch {
                EaseDirection.In => Easings.BounceIn,
                EaseDirection.Out => Easings.BounceOut,
                EaseDirection.InOut => Easings.BounceInOut,
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

}
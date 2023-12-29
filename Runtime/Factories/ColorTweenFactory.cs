using System;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// A factory for creating tweens that animate <see cref="Color"/> properties.
/// Also implements composite interfaces for animating RGBA and HSV parts of
/// the <see cref="Color"/>.
/// </summary>
/// <typeparam name="T">The type of the object that holds the property. Most commonly an <see cref="UnityEngine.Object"/>.</typeparam>
/// <example>
/// <code>
/// var factory = new ColorTweenFactory&lt;Material&gt;(
///     material => material.color,
///     (material, color) => material.color = color
/// );
/// 
/// // Tween the material's color to blue over 1 second
/// material
///     .Tween(factory, Color.blue)
///     .SetDuration(1);
///
/// 
/// // Tween the material's saturation to 0.5 over 1 second with an ease
/// material
///     .Tween(factory.AsHSV(), HSV.S, 0.5f)
///     .SetDuration(1)
///     .Ease(EaseType.CubicInOut);
/// </code>
/// </example>
public class ColorTweenFactory<T> : 
    TweenFactory<Color, T>, 
    ICompositeTweenFactory<Color, T, float, RGBA>,
    ICompositeTweenFactory<Color, T, float, HSV>
{
    public ColorTweenFactory(Func<T, Color> getter, Action<T, Color> setter) 
        : base(getter, setter, Color.LerpUnclamped) { }

    public void SetPart(ref Color composite, RGBA part, float value) => composite[(int)part] = value;
    public float GetPart(Color composite, RGBA part) => composite[(int)part];

    public void SetPart(ref Color composite, HSV part, float value) {
        Color.RGBToHSV(composite, out var h, out var s, out var v);
        switch (part) {
            case HSV.H:
                h = value; break;
            case HSV.S:
                s = value; break;
            case HSV.V:
                v = value; break;
            default: throw new ArgumentOutOfRangeException(nameof(part), part, null);
        }
        composite = Color.HSVToRGB(h, s, v);
    }

    public float GetPart(Color composite, HSV part) {
        Color.RGBToHSV(composite, out var h, out var s, out var v);
        return part switch {
            HSV.H => h,
            HSV.S => s,
            HSV.V => v,
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
        };
    }

    public float Lerp(float from, float to, float t) => Mathf.LerpUnclamped(from, to, t);

    /// <summary>
    /// Casts this factory to a composite factory for animating RGBA parts of the <see cref="Color"/>.
    /// </summary>
    public ICompositeTweenFactory<Color, T, float, RGBA> AsRGBA() => this;
    
    /// <summary>
    /// Casts this factory to a composite factory for animating HSV parts of the <see cref="Color"/>.
    /// </summary>
    public ICompositeTweenFactory<Color, T, float, HSV> AsHSV() => this;
}

public enum RGBA {
    R, G, B, A
}

public enum HSV {
    H, S, V
}

}
using System;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// A factory for creating tweens that animate <see cref="Color"/> properties.
/// Also implements composite interfaces for animating RGBA and HSV components of
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
    
    public static ColorTweenFactory<T> From(string propertyName) {
        return ReflectionTweenFactory.Create<ColorTweenFactory<T>>(propertyName);
    }
    
    public void SetComponent(ref Color composite, RGBA component, float value) => composite[(int)component] = value;
    public float GetComponent(Color composite, RGBA component) => composite[(int)component];

    public void SetComponent(ref Color composite, HSV component, float value) {
        Color.RGBToHSV(composite, out var h, out var s, out var v);
        switch (component) {
            case HSV.H:
                h = value; break;
            case HSV.S:
                s = value; break;
            case HSV.V:
                v = value; break;
            default: throw new ArgumentOutOfRangeException(nameof(component), component, null);
        }
        composite = Color.HSVToRGB(h, s, v);
    }

    public float GetComponent(Color composite, HSV component) {
        Color.RGBToHSV(composite, out var h, out var s, out var v);
        return component switch {
            HSV.H => h,
            HSV.S => s,
            HSV.V => v,
            _ => throw new ArgumentOutOfRangeException(nameof(component), component, null)
        };
    }

    public float Lerp(float from, float to, float t) => Mathf.LerpUnclamped(from, to, t);

    /// <summary>
    /// Casts this factory to a composite factory for animating <see cref="Color"/> in RGBA space.
    /// </summary>
    public ICompositeTweenFactory<Color, T, float, RGBA> AsRGBA() => this;
    
    /// <summary>
    /// Casts this factory to a composite factory for animating <see cref="Color"/> in HSV space.
    /// </summary>
    public ICompositeTweenFactory<Color, T, float, HSV> AsHSV() => this;
}

/// <summary>
/// Components of a Color.
/// </summary>
public enum RGBA {
    /// <summary>
    /// Red value (0-1).
    /// </summary>
    R,
    
    /// <summary>
    /// Green value (0-1).
    /// </summary>
    G,
    
    /// <summary>
    /// Blue value (0-1).
    /// </summary>
    B,
    
    /// <summary>
    /// Alpha value (0-1).
    /// </summary>
    A
}

/// <summary>
/// Components of a Color in HSV space.
/// </summary>
public enum HSV {
    /// <summary>
    /// Hue value (0-1).
    /// </summary>
    H,
    
    /// <summary>
    /// Saturation value (0-1).
    /// </summary>
    S, 
    
    /// <summary>
    /// Brightness value (0-1) (also known as Value).
    /// </summary>
    V
}

}
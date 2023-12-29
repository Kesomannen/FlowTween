using FlowTween.Components;
using UnityEngine;
using UnityEngine.UI;

namespace FlowTween {

public static class GraphicTweens {
    public static ColorTweenFactory<Graphic> Color { get; } = new(c => c.color, (c, color) => c.color = color);
    
    static readonly RGBA[] _rgb = { RGBA.R, RGBA.G, RGBA.B };
    
    public static Tween<Color> TweenColor(this Graphic graphic, Color color) => graphic.Tween(Color, color);
    
    public static Tween<Color> TweenRGB(this Graphic graphic, Color color) => graphic.Tween(Color.AsRGBA(), _rgb, color);
    
    public static Tween<float> TweenRed(this Graphic graphic, float value) => graphic.Tween(Color.AsRGBA(), RGBA.R, value);
    public static Tween<float> TweenGreen(this Graphic graphic, float value) => graphic.Tween(Color.AsRGBA(), RGBA.G, value);
    public static Tween<float> TweenBlue(this Graphic graphic, float value) => graphic.Tween(Color.AsRGBA(), RGBA.B, value);
    public static Tween<float> TweenAlpha(this Graphic graphic, float value) => graphic.Tween(Color.AsRGBA(), RGBA.A, value);
    
    public static Tween<float> TweenHue(this Graphic graphic, float value) => graphic.Tween(Color.AsHSV(), HSV.H, value);
    public static Tween<float> TweenSaturation(this Graphic graphic, float value) => graphic.Tween(Color.AsHSV(), HSV.S, value);
    public static Tween<float> TweenValue(this Graphic graphic, float value) => graphic.Tween(Color.AsHSV(), HSV.V, value);
    
    public static Tween<float> TweenGradient(this Graphic graphic, Gradient gradient) {
        return graphic.TweenValue(0, 1).OnUpdate(v => graphic.color = gradient.Evaluate(v));
    }
}

}
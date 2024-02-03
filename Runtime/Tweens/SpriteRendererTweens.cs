using UnityEngine;

namespace FlowTween {

public static class SpriteRendererTweens {
    public static ColorTweenFactory<SpriteRenderer> Color { get; } = new(r => r.color, (r, c) => r.color = c);

    public static Tween<Color> TweenColor(this SpriteRenderer renderer, Color to) => renderer.Tween(Color, to);

    public static Tween<float> TweenRed(this SpriteRenderer renderer, float red) => renderer.Tween(Color.AsRGBA(), RGBA.R, red);
    public static Tween<float> TweenGreen(this SpriteRenderer renderer, float green) => renderer.Tween(Color.AsRGBA(), RGBA.G, green);
    public static Tween<float> TweenBlue(this SpriteRenderer renderer, float blue) => renderer.Tween(Color.AsRGBA(), RGBA.B, blue);
    public static Tween<float> TweenAlpha(this SpriteRenderer renderer, float alpha) => renderer.Tween(Color.AsRGBA(), RGBA.A, alpha);

    public static Tween<float> TweenHue(this SpriteRenderer renderer, float hue) => renderer.Tween(Color.AsHSV(), HSV.H, hue);
    public static Tween<float> TweenSaturation(this SpriteRenderer renderer, float saturation) => renderer.Tween(Color.AsHSV(), HSV.S, saturation);
    public static Tween<float> TweenBrightness(this SpriteRenderer renderer, float value) => renderer.Tween(Color.AsHSV(), HSV.V, value);
    
    public static Tween<float> TweenGradient(this SpriteRenderer renderer, Gradient gradient) {
        return renderer.TweenValue(0, 1).OnUpdate(v => renderer.color = gradient.Evaluate(v));
    }
}

}
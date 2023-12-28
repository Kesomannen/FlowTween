using System;
using UnityEngine;

namespace FlowTween {

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

    public ICompositeTweenFactory<Color, T, float, RGBA> AsRGBA() => this;
    public ICompositeTweenFactory<Color, T, float, HSV> AsHSV() => this;
}

public enum RGBA {
    R, G, B, A
}

public enum HSV {
    H, S, V
}

}
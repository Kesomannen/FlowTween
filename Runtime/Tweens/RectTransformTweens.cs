using System;
using FlowTween.Components;
using UnityEngine;

namespace FlowTween {

public static class RectTransformTweens {
    public static Vector2TweenFactory<RectTransform> Position { get; } = new(t => t.anchoredPosition, (t, v) => t.anchoredPosition = v);
    public static Vector2TweenFactory<RectTransform> SizeDelta { get; } = new(t => t.sizeDelta, (t, v) => t.sizeDelta = v);
    
    public static Tween<Vector2> TweenPosition(this RectTransform rectTransform, Vector2 to) => rectTransform.Tween(Position, to);
    public static Tween<float> TweenX(this RectTransform rectTransform, float to) => rectTransform.Tween(Position, Axis2.X, to);
    public static Tween<float> TweenY(this RectTransform rectTransform, float to) => rectTransform.Tween(Position, Axis2.Y, to);
    
    public static Tween<Vector2> TweenSizeDelta(this RectTransform rectTransform, Vector2 to) => rectTransform.Tween(SizeDelta, to);
    public static Tween<float> TweenSizeDeltaX(this RectTransform rectTransform, float to) => rectTransform.Tween(SizeDelta, Axis2.X, to);
    public static Tween<float> TweenSizeDeltaY(this RectTransform rectTransform, float to) => rectTransform.Tween(SizeDelta, Axis2.Y, to);
}

}
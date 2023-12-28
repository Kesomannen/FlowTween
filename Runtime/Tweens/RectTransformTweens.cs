using System;
using FlowTween.Components;
using UnityEngine;

namespace FlowTween {

public static class RectTransformTweens {
    public static Vector2TweenFactory<RectTransform> AnchoredPosition { get; } = new(t => t.anchoredPosition, (t, v) => t.anchoredPosition = v);
    public static Vector2TweenFactory<RectTransform> SizeDelta { get; } = new(t => t.sizeDelta, (t, v) => t.sizeDelta = v);
    
    public static Tween<Vector2> TweenAnchoredPosition(this RectTransform rectTransform, Vector2 to) => rectTransform.Tween(AnchoredPosition, to);
    public static Tween<float> TweenAnchoredX(this RectTransform rectTransform, float to) => rectTransform.Tween(AnchoredPosition, Axis2.X, to);
    public static Tween<float> TweenAnchoredY(this RectTransform rectTransform, float to) => rectTransform.Tween(AnchoredPosition, Axis2.Y, to);
    
    public static Tween<Vector2> TweenSizeDelta(this RectTransform rectTransform, Vector2 to) => rectTransform.Tween(SizeDelta, to);
    public static Tween<float> TweenSizeDeltaX(this RectTransform rectTransform, float to) => rectTransform.Tween(SizeDelta, Axis2.X, to);
    public static Tween<float> TweenSizeDeltaY(this RectTransform rectTransform, float to) => rectTransform.Tween(SizeDelta, Axis2.Y, to);
}

}
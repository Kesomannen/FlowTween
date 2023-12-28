using FlowTween.Components;
using UnityEngine;

namespace FlowTween {

public static class CanvasGroupTweens {
    public static FloatTweenFactory<CanvasGroup> Alpha { get; } = new(c => c.alpha, (c, alpha) => c.alpha = alpha);
    
    public static Tween<float> TweenAlpha(this CanvasGroup canvasGroup, float alpha) => canvasGroup.Tween(Alpha, alpha);
}

}
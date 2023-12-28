using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

# if UNITY_EDITOR
using UnityEditor;
# endif

namespace FlowTween.Components {

public static class TweenerTargets {
    public static IReadOnlyDictionary<string, ITweenerTarget> Targets => _targets;

    static RectTransform _canvas;
    
    public static void Register(string id, ITweenerTarget target) {
        _targets[id] = target;
    }

    static readonly Dictionary<string, ITweenerTarget> _targets = new() {
        { "TransformPosition", new Vector3TweenerTarget<Transform>(TransformTweens.Position) {
            DrawGizmos = (_, start, end) => {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(start, 0.05f);
                Gizmos.DrawLine(start, end);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(end, 0.05f);
            }
        } },
        { "TransformLocalPosition", new Vector3TweenerTarget<Transform>(TransformTweens.LocalPosition) {
            DrawGizmos = (transform, start, end) => {
                var pos = transform.position;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(pos + start, 0.05f);
                Gizmos.DrawLine(pos + start, end);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(pos + end, 0.05f);
            }
        } },
        { "TransformRotation", new Vector3TweenerTarget<Transform>(TransformTweens.EulerAngles) },
        { "TransformLocalRotation", new Vector3TweenerTarget<Transform>(TransformTweens.LocalEulerAngles) },
        { "TransformScale", new Vector3TweenerTarget<Transform>(TransformTweens.Scale) },
        { "TransformUniformScale", new FloatTweenerTarget<Transform>(TransformTweens.UniformScale) },
        { "RectTransformAnchoredPosition", new Vector2TweenerTarget<RectTransform>(RectTransformTweens.AnchoredPosition) },
        { "RectTransformSizeDelta", new Vector2TweenerTarget<RectTransform>(RectTransformTweens.SizeDelta) },
        { "GraphicColor", new ColorTweenerTarget<Graphic>(GraphicTweens.Color) },
        { "GraphicAlpha", new FloatTweenerTarget<Graphic>(GraphicTweens.Color.AsRGBA().WithPart(RGBA.A)) },
        { "GraphicGradient", new GradientTweenerTarget<Graphic>(GraphicTweens.Color) },
        { "CanvasGroupAlpha", new FloatTweenerTarget<CanvasGroup>(CanvasGroupTweens.Alpha) },
        { "SpriteRendererColor", new ColorTweenerTarget<SpriteRenderer>(SpriteRendererTweens.Color) },
        { "SpriteRendererGradient", new GradientTweenerTarget<SpriteRenderer>(SpriteRendererTweens.Color) },
        { "FieldOfView", new FloatTweenerTarget<Camera>(CameraTweens.FieldOfView) }
    };
}

}
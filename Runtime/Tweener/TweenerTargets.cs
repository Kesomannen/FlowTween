using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowTween.Components {

/// <summary>
/// A registry for the available tweener targets.
/// </summary>
/// <seealso cref="Tweener"/>
/// <seealso cref="ITweenerTarget"/>
/// <seealso cref="TweenerTargetConfig"/>
public static class TweenerTargets {
    /// <summary>
    /// All the available tweener targets by id.
    /// </summary>
    public static IReadOnlyDictionary<string, ITweenerTarget> Targets => _targets;

    static RectTransform _canvas;
    
    /// <summary>
    /// Registers a target with the given id.
    /// If a target with the same id already exists, it will be overwritten.
    /// 
    /// <br/><br/>The naming scheme for ids is <c>&lt;ComponentName&gt;&lt;PropertyName&gt;</c>, e.g. <c>TransformPosition</c>.
    ///
    /// <br/><br/>Domain reloads clear the registry to the default values, so if you want your target
    /// to be available in the editor, you will need to register them with <see cref="UnityEditor.InitializeOnLoadAttribute"/>;.
    /// </summary>
    /// <example>
    /// <code>
    /// // Create a factory to define the behavior of the target
    /// var factory = new ColorTweenFactory&lt;Material&gt;(m => m.color, (m, c) => m.color = c);
    ///
    /// 
    /// // Create a target with one of the built-in target types.
    /// // The FromTo family of targets simply tween between two values,
    /// // that can either be relative or absolute
    /// var target = new ColorFromToTweenerTarget&lt;Material&gt;(factory);
    ///
    /// 
    /// // Register the target
    /// TweenerTargets.Register("MaterialColor", target);
    /// </code>
    /// </example>
    public static void Register(string id, ITweenerTarget target) {
        _targets[id] = target;
    }

    static readonly Dictionary<string, ITweenerTarget> _targets = new() {
        { "TransformPosition", new Vector3FromToTweenerTarget<Transform>(TransformTweens.Position) {
            DrawGizmos = (_, start, end) => {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(start, 0.05f);
                Gizmos.DrawLine(start, end);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(end, 0.05f);
            }
        } },
        { "TransformLocalPosition", new Vector3FromToTweenerTarget<Transform>(TransformTweens.LocalPosition) {
            DrawGizmos = (transform, start, end) => {
                var pos = transform.position;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(pos + start, 0.05f);
                Gizmos.DrawLine(pos + start, end);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(pos + end, 0.05f);
            }
        } },
        { "TransformRotation", new Vector3FromToTweenerTarget<Transform>(TransformTweens.EulerAngles) },
        { "TransformLocalRotation", new Vector3FromToTweenerTarget<Transform>(TransformTweens.LocalEulerAngles) },
        { "TransformScale", new Vector3FromToTweenerTarget<Transform>(TransformTweens.Scale) },
        { "TransformUniformScale", new FloatFromToTweenerTarget<Transform>(TransformTweens.UniformScale) },
        { "RectTransformAnchoredPosition", new Vector2FromToTweenerTarget<RectTransform>(RectTransformTweens.AnchoredPosition) },
        { "RectTransformSizeDelta", new Vector2FromToTweenerTarget<RectTransform>(RectTransformTweens.SizeDelta) },
        { "GraphicColor", new ColorFromToTweenerTarget<Graphic>(GraphicTweens.Color) },
        { "GraphicAlpha", new FloatFromToTweenerTarget<Graphic>(GraphicTweens.Color.AsRGBA().WithPart(RGBA.A)) },
        { "GraphicColor (Gradient)", new GradientTweenerTarget<Graphic>(GraphicTweens.Color) },
        { "CanvasGroupAlpha", new FloatFromToTweenerTarget<CanvasGroup>(CanvasGroupTweens.Alpha) },
        { "SpriteRendererColor", new ColorFromToTweenerTarget<SpriteRenderer>(SpriteRendererTweens.Color) },
        { "SpriteRendererColor (Gradient)", new GradientTweenerTarget<SpriteRenderer>(SpriteRendererTweens.Color) },
        { "CameraFieldOfView", new FloatFromToTweenerTarget<Camera>(CameraTweens.FieldOfView) }
    };
}

}
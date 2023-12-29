using System;
using UnityEngine;

namespace FlowTween.Templates {

/// <summary>
/// A serializable property that can either be set inline or use a
/// shared scriptable object <see cref="TemplateAsset{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the property.</typeparam>
[Serializable]
public abstract class TemplateProperty<T> {
    [SerializeField] bool _useTemplate;
    [SerializeField] TemplateAsset<T> _templateValue;
    [SerializeField] T _inlineValue;
    
    /// <summary>
    /// Whether the property is using a template or not.
    /// </summary>
    public bool UsingTemplate => _useTemplate;

    /// <summary>
    /// Gets the value of the property, either from a template or inline.
    /// </summary>
    public T Value => _useTemplate ? _templateValue.Template ?? _inlineValue : _inlineValue;

    /// <summary>
    /// Gets or sets the template.
    /// Is only used if <see cref="UsingTemplate"/> is true.
    /// When you set this to anything other than null, <see cref="UsingTemplate"/> is also set to true.
    /// </summary>
    public TemplateAsset<T> TemplateValue {
        get => _templateValue;
        set {
            _templateValue = value;
            _useTemplate = _templateValue != null;
        }
    }

    /// <summary>
    /// Gets or sets the inline value.
    /// Is only used if <see cref="UsingTemplate"/> is false.
    /// When you set this, <see cref="UsingTemplate"/> is also set to false.
    /// </summary>
    public T InlineValue {
        get => _inlineValue;
        set {
            _inlineValue = value;
            _useTemplate = false;
        }
    }
}

}
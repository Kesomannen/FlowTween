using System;
using UnityEngine;

namespace FlowTween.Templates {

/// <summary>
/// A serializable property that can either be set inline or use a
/// shared scriptable object <see cref="TweenSettingsTemplateAsset}"/>.
/// </summary>
[Serializable]
public class TweenSettingsProperty : TemplateProperty<TweenSettings> {
    public TweenSettingsProperty() { }
    public TweenSettingsProperty(TweenSettings inlineValue) : base(inlineValue) { }
    public TweenSettingsProperty(TemplateAsset<TweenSettings> templateValue) : base(templateValue) { }
    
    public static implicit operator TweenSettings(TweenSettingsProperty property) => property.Value;
    public static implicit operator TweenSettingsProperty(TweenSettings value) => new TweenSettingsProperty(value);
    public static implicit operator TweenSettingsProperty(TemplateAsset<TweenSettings> template) => new TweenSettingsProperty(template);
}

}
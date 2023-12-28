using FlowTween.Templates;
using UnityEditor;

namespace FlowTween.Editor {

[CustomPropertyDrawer(typeof(TweenSettingsProperty))]
public class TweenSettingsPropertyDrawer : TemplatePropertyDrawer<TweenSettings, TweenSettingsTemplateAsset, TweenSettingsProperty> { }

}
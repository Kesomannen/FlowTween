using FlowTween.Templates;
using UnityEditor;

namespace FlowTween.Editor {

[CustomPropertyDrawer(typeof(TweenSettingsProperty))]
internal class TweenSettingsPropertyDrawer : TemplatePropertyDrawer<TweenSettings, TweenSettingsTemplateAsset, TweenSettingsProperty> { }

}
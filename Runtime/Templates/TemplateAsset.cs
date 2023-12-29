using UnityEngine;

namespace FlowTween.Templates {

/// <summary>
/// Base class for template assets.
/// </summary>
/// <typeparam name="T">The type the template is for.</typeparam>
public abstract class TemplateAsset<T> : ScriptableObject {
    [SerializeField] T _template;

    public T Template {
        get => _template;
        set => _template = value;
    }
}

}
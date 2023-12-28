using UnityEngine;

namespace FlowTween.Templates {

public abstract class TemplateAsset<T> : ScriptableObject {
    [SerializeField] T _template;

    public T Template {
        get => _template;
        set => _template = value;
    }
}

}
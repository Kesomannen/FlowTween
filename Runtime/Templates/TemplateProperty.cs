using System;
using UnityEngine;

namespace FlowTween.Templates {

[Serializable]
public abstract class TemplateProperty<T> {
    [SerializeField] bool _useTemplate;
    [SerializeField] TemplateAsset<T> _templateValue;
    [SerializeField] T _inlineValue;
    
    public bool UsingTemplate => _useTemplate;

    public T Value => _useTemplate ? _templateValue.Template ?? _inlineValue : _inlineValue;

    public TemplateAsset<T> TemplateValue {
        get => _templateValue;
        set {
            _templateValue = value;
            _useTemplate = true;
        }
    }

    public T InlineValue {
        get => _inlineValue;
        set {
            _inlineValue = value;
            _useTemplate = false;
        }
    }
}

}
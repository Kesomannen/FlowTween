using System;
using UnityEngine;

namespace FlowTween.Components {

/// <summary>
/// A tweener target that tweens a <see cref="Color"/> property according to a <see cref="Gradient"/>.
/// </summary>
/// <typeparam name="T">The type of the component that holds the property.</typeparam>
public class GradientTweenerTarget<T> : ITweenerTarget<Color, T, GradientData> where T : Component {
    readonly Func<T, Color> _getter;
    readonly Action<T, Color> _setter;
    
    public GradientTweenerTarget(
        Func<T, Color> getter,
        Action<T, Color> setter
    ) {
        _getter = getter;
        _setter = setter;
    }

    public GradientTweenerTarget(ITweenFactory<Color, T> factory) {
        _getter = factory.Get;
        _setter = factory.Set;
    }
    
    public object GetData() {
        return new GradientData();
    }

    public Color TakeSnapshot(T holder) {
        return _getter(holder);
    }

    public void ApplySnapshot(T holder, Color snapshot) {
        _setter(holder, snapshot);
    }

    public TweenBase GetTween(T holder, GradientData data) {
        return holder.TweenValue(0, 1).OnUpdate(t => _setter(holder, data.Gradient.Evaluate(t)));
    }
}

[Serializable]
public class GradientData {
    // Gradient won't show using SerializeReference
    // so we have wrap it :/
    public Gradient Gradient;
} 

}
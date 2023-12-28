using System;
using UnityEngine;

namespace FlowTween.Components {

public class TweenerTarget<T, THolder, TData> : ITweenerTarget<T, THolder, TData>
    where THolder : Component 
    where TData : class, ITweenerTargetData<T>, new() 
{
    readonly ITweenFactory<T, THolder> _factory;
    
    public Action<THolder, T, T> DrawGizmos;
    
    public TweenerTarget(ITweenFactory<T, THolder> factory) {
        _factory = factory;
    }

    public T TakeSnapshot(THolder holder) {
        return _factory.Get(holder);
    }

    public void ApplySnapshot(THolder holder, T snapshot) {
        _factory.Set(holder, snapshot);
    }

    public virtual TweenBase GetTween(THolder holder, TData data) {
        var (start, end) = GetValues(data, holder);
        return holder.Tween(_factory, end).From(start);
    }

    public void OnDrawGizmos(THolder holder, TData data) {
        var (start, end) = GetValues(data, holder);
        DrawGizmos?.Invoke(holder, start, end);
    }

    public object GetData() {
        return new TData();
    }

    protected (T, T) GetValues(TData data, THolder holder) {
        var current = _factory.Get(holder); 
        return (
            data.GetStartValue(current),
            data.GetEndValue(current)
        );
    }
}

public class FloatTweenerTarget<T> : TweenerTarget<float, T, FloatTweenerTargetData> where T : Component {
    public FloatTweenerTarget(ITweenFactory<float, T> factory) : base(factory) { }
}

public class Vector3TweenerTarget<T> : TweenerTarget<Vector3, T, Vector3TweenerTargetData> where T : Component {
    public Vector3TweenerTarget(ITweenFactory<Vector3, T> factory) : base(factory) { }
}

public class Vector2TweenerTarget<T> : TweenerTarget<Vector2, T, Vector2TweenerTargetData> where T : Component {
    public Vector2TweenerTarget(ITweenFactory<Vector2, T> factory) : base(factory) { }
}

public class ColorTweenerTarget<T> : TweenerTarget<Color, T, ColorTweenerTargetData> where T : Component {
    public ColorTweenerTarget(ITweenFactory<Color, T> factory) : base(factory) { }
}

}
using System;
using UnityEngine;

namespace FlowTween.Components {
    
/// <summary>
/// An implementation of <see cref="ITweenerTarget{T,THolder,TData}"/> that
/// simply tweens a property between two values using a <see cref="ITweenFactory{T,THolder}"/>.
/// It relies on a data type that implements <see cref="IFromToTweenerTargetData{T}"/>.
/// </summary>
/// <seealso cref="FloatFromToTweenerTarget{T}"/>
/// <seealso cref="Vector3FromToTweenerTarget{T}"/>
/// <seealso cref="Vector2FromToTweenerTarget{T}"/>
/// <seealso cref="ColorFromToTweenerTarget{T}"/>
/// <inheritdoc cref="ITweenerTarget{T,THolder,TData}"/>
public class FromToTweenerTarget<T, THolder, TData> : ITweenerTarget<T, THolder, TData>
    where THolder : Component 
    where TData : class, IFromToTweenerTargetData<T>, new() 
{
    readonly ITweenFactory<T, THolder> _factory;
    
    public Action<THolder, T, T> DrawGizmos;
    
    public FromToTweenerTarget(ITweenFactory<T, THolder> factory) {
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

/// <summary>
/// A tweener target that tweens a float property between two values.
/// </summary>
/// <typeparam name="T">The type of the component that holds the property.</typeparam>
public class FloatFromToTweenerTarget<T> : FromToTweenerTarget<float, T, FloatTweenerTargetData> where T : Component {
    public FloatFromToTweenerTarget(ITweenFactory<float, T> factory) : base(factory) { }
}

/// <summary>
/// A tweener target that tweens a <see cref="Vector3"/> property between two values.
/// </summary>
/// <typeparam name="T">The type of the component that holds the property.</typeparam>
public class Vector3FromToTweenerTarget<T> : FromToTweenerTarget<Vector3, T, Vector3TweenerTargetData> where T : Component {
    public Vector3FromToTweenerTarget(ITweenFactory<Vector3, T> factory) : base(factory) { }
}

/// <summary>
/// A tweener target that tweens a <see cref="Vector2"/> property between two values.
/// </summary>
/// <typeparam name="T">The type of the component that holds the property.</typeparam>
public class Vector2FromToTweenerTarget<T> : FromToTweenerTarget<Vector2, T, Vector2TweenerTargetData> where T : Component {
    public Vector2FromToTweenerTarget(ITweenFactory<Vector2, T> factory) : base(factory) { }
}

/// <summary>
/// A tweener target that tweens a <see cref="Color"/> property between two values.
/// </summary>
/// <typeparam name="T">The type of the component that holds the property.</typeparam>
public class ColorFromToTweenerTarget<T> : FromToTweenerTarget<Color, T, ColorTweenerTargetData> where T : Component {
    public ColorFromToTweenerTarget(ITweenFactory<Color, T> factory) : base(factory) { }
}

}
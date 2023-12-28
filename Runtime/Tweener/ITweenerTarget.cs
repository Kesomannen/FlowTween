using System;
using UnityEngine;

namespace FlowTween.Components {

/// <summary>
/// Implement this interface to create a custom tween target type for the <see cref="Tweener"/> component.
/// For most use cases inheriting from <see cref="FromToTweenerTarget{T,THolder,TData}"/> is enough and avoids a lot of boilerplate.
/// Otherwise, the type safe <see cref="ITweenerTarget{T,THolder,TData}"/> should be used.
/// </summary>
public interface ITweenerTarget {
    /// <summary>
    /// The component type that holds the tween.
    /// </summary>
    Type ComponentType { get; }
    
    /// <summary>
    /// Takes a snapshot of the tweened values.
    /// </summary>
    object TakeSnapshot(Component component);
    
    /// <summary>
    /// Applies a snapshot of the previously captured tweened values.
    /// </summary>
    void ApplySnapshot(Component component, object snapshot);

    /// <summary>
    /// Gets a new data object for this target.
    /// </summary>
    /// <returns></returns>
    object GetData();
    
    /// <summary>
    /// Gets a tween for this target, using the given data and holder component.
    /// </summary>
    TweenBase GetTween(Component component, object data);
    
    /// <summary>
    /// Draws gizmos for this target, using the given data and holder component.
    /// <see cref="Tweener"/> calls this for selected tweens every OnDrawGizmosSelected call.
    /// </summary>
    virtual void OnDrawGizmos(Component component, object data) { }
}

/// <summary>
/// Implement this interface to create a custom tween target type for the <see cref="Tweener"/> component.
/// For targets that simply tween betwen two values, use <see cref="FromToTweenerTarget{T,THolder,TData}"/>
/// or one of its subclasses instead.
/// </summary>
/// <typeparam name="T">The tween value type.</typeparam>
/// <typeparam name="THolder">The component type that holds the tween.</typeparam>
/// <typeparam name="TData">
/// A data type that holds the configuration of the target.
/// Cannot be generic, as it is serialized using <see cref="SerializeReference"/>.
/// </typeparam>
public interface ITweenerTarget<T, in THolder, in TData> : ITweenerTarget 
    where THolder : Component
    where TData : class
{
    /// <inheritdoc cref="ITweenerTarget.TakeSnapshot"/>
    T TakeSnapshot(THolder holder);
    /// <inheritdoc cref="ITweenerTarget.ApplySnapshot"/>
    void ApplySnapshot(THolder holder, T snapshot);
    
    /// <inheritdoc cref="ITweenerTarget.GetData"/>
    TweenBase GetTween(THolder holder, TData data);
    /// <inheritdoc cref="ITweenerTarget.OnDrawGizmos"/>
    virtual void OnDrawGizmos(THolder holder, TData data) { }

    Type ITweenerTarget.ComponentType => typeof(THolder);

    void ITweenerTarget.ApplySnapshot(Component component, object snapshot) {
        ApplySnapshot(AssertComponentType(component), AssertSnapshotType(snapshot));
    }

    object ITweenerTarget.TakeSnapshot(Component component) {
        return TakeSnapshot(AssertComponentType(component));
    }

    TweenBase ITweenerTarget.GetTween(Component component, object data) {
        return GetTween(AssertComponentType(component), AssertDataType(data));
    }

    void ITweenerTarget.OnDrawGizmos(Component component, object obj) {
        OnDrawGizmos(AssertComponentType(component), AssertDataType(obj));
    }

    protected static THolder AssertComponentType(Component component) {
        if (component is THolder holder) return holder;
        throw new InvalidOperationException($"Component {component} is not of type {typeof(THolder)}");
    }

    protected static TData AssertDataType(object obj) {
        if (obj is TData data) return data;
        throw new InvalidOperationException($"Component {obj} is not of type {typeof(TData)}");
    }

    protected static T AssertSnapshotType(object obj) {
        if (obj is T snapshot) return snapshot;
        throw new InvalidOperationException($"Component {obj} is not of type {typeof(T)}");
    }
}

}
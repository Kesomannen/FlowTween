using System;
using UnityEngine;

namespace FlowTween.Components {

public interface ITweenerTarget {
    Type ComponentType { get; }
    object TakeSnapshot(Component component);
    void ApplySnapshot(Component component, object snapshot);

    object GetData();
    TweenBase GetTween(Component component, object data);
    virtual void OnDrawGizmos(Component component, object data) { }
}

public interface ITweenerTarget<T, in THolder, in TData> : ITweenerTarget 
    where THolder : Component
    where TData : class
{
    T TakeSnapshot(THolder holder);
    void ApplySnapshot(THolder holder, T snapshot);

    TweenBase GetTween(THolder holder, TData data);
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
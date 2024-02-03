using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlowTween {

/// <summary>
/// Interface for <see cref="ITweenFactory{T,THolder}"/> which value types
/// are composites, such as Vector3 or Color. This interface allows tweens
/// to only animate a single or a few components of the composite value, such as
/// only the X axis of a Vector3 property.
/// </summary>
/// <typeparam name="T">The composite type.</typeparam>
/// <typeparam name="THolder">The type of the object that holds the property. Most commonly an <see cref="Object"/>.</typeparam>
/// <typeparam name="TComponent">The type of the components of the composite value.</typeparam>
/// <typeparam name="TComponentId">A type for identifying the components of the composite value. Most commonly an enum.</typeparam>
public interface ICompositeTweenFactory<T, in THolder, TComponent, in TComponentId> : ITweenFactory<T, THolder> {
    /// <summary>
    /// Sets a component of the composite value to the given value.
    /// </summary>
    void SetComponent(ref T composite, TComponentId component, TComponent value);
    
    /// <summary>
    /// Gets a component of the composite value.
    /// </summary>
    TComponent GetComponent(T composite, TComponentId component);
    
    /// <summary>
    /// Linearly interpolates between two components of the composite value.
    /// </summary>
    TComponent Lerp(TComponent from, TComponent to, float t);
    
    /// <summary>
    /// Modifies a tween so it animates one component of the target property.
    /// Also sets the tween's <see cref="Tween{T}.Start"/> value to the current value.
    /// </summary>
    public Tween<TComponent> Apply(Tween<TComponent> tween, THolder holder, TComponentId part) {
        return tween.OnUpdate(v => {
                var current = Get(holder);
                SetComponent(ref current, part, v);
                Set(holder, current);
            })
            .Lerp(Lerp)
            .From(GetComponent(Get(holder), part));
    }

    /// <summary>
    /// Modifies a tween so it animates any number of components of the target property.
    /// Also sets the tween's <see cref="Tween{T}.Start"/> value to the current value.
    /// </summary>
    public Tween<T> Apply(Tween<T> tween, THolder holder, IReadOnlyCollection<TComponentId> parts) {
        return tween.OnUpdate(v => { 
                var current = Get(holder);
                foreach (var part in parts) {
                    SetComponent(ref current, part, GetComponent(v, part));
                }
                Set(holder, current);
            })
            .Lerp(Lerp)
            .From(Get(holder));
    }
    
    /// <summary>
    /// Creates a new <see cref="ITweenFactory{T,THolder}"/> that animates the given component of the target property.
    /// </summary>
    public ITweenFactory<TComponent, THolder> WithPart(TComponentId part) {
        return new ComponentTweenFactory<T, THolder, TComponent, TComponentId>(this, part);
    }
}

internal class ComponentTweenFactory<T, THolder, TComponent, TComponentId> : ITweenFactory<TComponent, THolder> {
    readonly ICompositeTweenFactory<T, THolder, TComponent, TComponentId> _factory;
    readonly TComponentId _part;

    public ComponentTweenFactory(ICompositeTweenFactory<T, THolder, TComponent, TComponentId> factory, TComponentId part) {
        _factory = factory;
        _part = part;
    }

    public void Set(THolder holder, TComponent value) {
        var current = _factory.Get(holder);
        _factory.SetComponent(ref current, _part, value);
        _factory.Set(holder, current);
    }
        
    public TComponent Get(THolder holder) {
        return _factory.GetComponent(_factory.Get(holder), _part);
    }
        
    public TComponent Lerp(TComponent from, TComponent to, float t) {
        return _factory.Lerp(from, to, t);
    }
}

/// <summary>
/// Provides convenience extension methods for creating tweens with <see cref="ICompositeTweenFactory{T,THolder,TPart,TPartId}"/>.
/// </summary>
public static class CompositeTweenFactoryTweenExtensions {
    /// <summary>
    /// Modifies the given tween so it animates one component of the factory's target property.
    /// </summary>
    /// <seealso cref="ICompositeTweenFactory{T,THolder,TPart,TPartId}.Apply(FlowTween.Tween{TPart},THolder,TPartId)"/>
    public static Tween<TComponent> Apply<T, THolder, TComponent, TComponentId>(
        this Tween<TComponent> tween, 
        ICompositeTweenFactory<T, THolder, TComponent, TComponentId> factory, 
        THolder holder, 
        TComponentId part
    ) {
        return factory.Apply(tween, holder, part);
    }
    
    /// <summary>
    /// Creates a tween with a composite factory that animates one component of the target property,
    /// and sets its <see cref="Tween{T}.End"/> value.
    /// </summary>
    /// <param name="holder">
    /// The target object. Also used as the tween's owner if it's a <see cref="Component"/> or <see cref="GameObject"/>. 
    /// </param>
    /// <param name="factory">The composite factory to use.</param>
    /// <param name="component">The component to animate.</param>
    /// <param name="to">The <see cref="Tween{T}.End"/> value of the tween.</param>
    /// <typeparam name="T">The tween and target property's value type.</typeparam>
    /// <typeparam name="THolder">The target object's type.</typeparam>
    /// <typeparam name="TComponent">The type of the parts of the composite value.</typeparam>
    /// <typeparam name="TComponentId">A type for identifying the components of the composite value. Most commonly an enum.</typeparam>
    /// <returns>
    /// A newly created tween. If we are at runtime and the <see cref="TweenManager"/> is
    /// enabled, the tween is automatically ran by the manager. Otherwise, you will
    /// have to run it manually by calling <see cref="Tween{T}.Update"/> yourself
    /// (see <see cref="TweenBase"/> for more details).
    /// </returns>
    public static Tween<TComponent> Tween<T, THolder, TComponent, TComponentId>(
        this THolder holder, 
        ICompositeTweenFactory<T, THolder, TComponent, TComponentId> factory, 
        TComponentId component,
        TComponent to
    ) where THolder : Object {
        return holder.Tween(to).Apply(factory, holder, component);
    }
    
    /// <summary>
    /// Modifies the given tween so it animates any number of components of the factory's target property.
    /// </summary>
    /// <seealso cref="ICompositeTweenFactory{T,THolder,TPart,TPartId}.Apply(FlowTween.Tween{T},THolder,System.Collections.Generic.IReadOnlyCollection{TPartId})"/>
    public static Tween<T> Apply<T, THolder, TPart, TPartId>(
        this Tween<T> tween, 
        ICompositeTweenFactory<T, THolder, TPart, TPartId> factory, 
        THolder holder,
        IReadOnlyCollection<TPartId> parts
    ) where THolder : Object {
        return factory.Apply(tween, holder, parts);
    }
    
    /// <summary>
    /// Creates a tween with a composite factory that animates any number of components of the target property,
    /// and sets its <see cref="Tween{T}.End"/> value.
    /// </summary>
    /// <param name="holder">
    /// The target object. Also used as the tween's owner if it's a <see cref="Component"/> or <see cref="GameObject"/>. 
    /// </param>
    /// <param name="factory">The composite factory to use.</param>
    /// <param name="parts">A collection of parts to animate. Duplicates are allowed.</param>
    /// <param name="to">
    /// The <see cref="Tween{T}.End"/> value of the tween.
    /// Any composite values not specified in <paramref name="parts"/> will be ignored.
    /// For example, if the target property is a <see cref="Vector3"/> and the <paramref name="parts"/> collection contains
    /// only <see cref="Axis.X"/> and <see cref="Axis.Y"/>, the Z component of the target property will not be animated.
    /// </param>
    /// <typeparam name="T">The tween and target property's value type.</typeparam>
    /// <typeparam name="THolder">The target object's type.</typeparam>
    /// <typeparam name="TComponent">The type of the components of the composite value.</typeparam>
    /// <typeparam name="TComponentId">A type for identifying the components of the composite value. Most commonly an enum.</typeparam>
    /// <returns>
    /// A newly created tween. If we are at runtime and the <see cref="TweenManager"/> is
    /// enabled, the tween is automatically ran by the manager. Otherwise, you will
    /// have to run it manually by calling <see cref="Tween{T}.Update"/> yourself
    /// (see <see cref="TweenBase"/> for more details).
    /// </returns>
    public static Tween<T> Tween<T, THolder, TComponent, TComponentId>(
        this THolder holder, 
        ICompositeTweenFactory<T, THolder, TComponent, TComponentId> factory, 
        IReadOnlyCollection<TComponentId> parts,
        T to
    ) where THolder : Object {
        return holder.Tween(to).Apply(factory, holder, parts);
    }
}

}
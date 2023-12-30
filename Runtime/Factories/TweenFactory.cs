using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlowTween {

/// <summary>
/// Interface for tween factories, which are wrappers for creating tweens
/// that manipulate a property of an object.
///
/// <br/><br/>Creating your own tween factory types is only necessary for
/// advanced use cases. Using one of the built-in factories,
/// like <see cref="FloatTweenFactory{T}"/> or <see cref="Vector3TweenFactory{T}"/>,
/// is usually enough.
/// </summary>
/// <typeparam name="T">The value type of the property and tween.</typeparam>
/// <typeparam name="THolder">The type of the object that holds the property. Most commonly an <see cref="Object"/>.</typeparam>
/// <seealso cref="TweenFactory{T,THolder}"/>
public interface ITweenFactory<T, in THolder> {
    /// <summary>
    /// Sets the target property to the given value.
    /// </summary>
    void Set(THolder holder, T value);
    
    /// <summary>
    /// Gets the target property of the given object.
    /// </summary>
    T Get(THolder holder);
    
    /// <summary>
    /// Linearly interpolates between two values.
    /// Should be unclamped.
    /// </summary>
    T Lerp(T from, T to, float t);
    
    /// <summary>
    /// Modifies the given tween so it animates the target property.
    /// Also sets the tween's <see cref="Tween{T}.Start"/> value to the current value of the property.
    /// </summary>
    public Tween<T> Apply(Tween<T> tween, THolder holder) {
        return tween.Lerp(Lerp)
            .OnUpdate(v => Set(holder, v))
            .From(Get(holder));
    }
}

/// <summary>
/// An implementation of <see cref="ITweenFactory{T,THolder}"/> that delegates
/// its methods to arguments passed in the constructor.
///
/// <br/><br/>Creating your own tween factory types is only necessary for
/// advanced use cases. Using one of the built-in factories,
/// like <see cref="FloatTweenFactory{T}"/> or <see cref="Vector3TweenFactory{T}"/>,
/// is usually enough.
/// </summary>
/// <inheritdoc cref="ITweenFactory{T,THolder}"/>
public class TweenFactory<T, THolder> : ITweenFactory<T, THolder> {
    readonly Func<THolder, T> _getter;
    readonly Action<THolder, T> _setter;
    readonly Func<T, T, float, T> _lerp;

    /// <summary>
    /// Creates a new tween factory.
    /// </summary>
    /// <param name="getter">Gets the target property of the given object.</param>
    /// <param name="setter">Sets the target property to the given value.</param>
    /// <param name="lerp">
    /// Linearly interpolates between two values.
    /// Should be unclamped.
    /// </param>
    public TweenFactory(
        Func<THolder, T> getter,
        Action<THolder, T> setter, 
        Func<T, T, float, T> lerp
    ) {
        _getter = getter;
        _setter = setter;
        _lerp = lerp;
    }

    public T Get(THolder holder) => _getter(holder);
    public void Set(THolder holder, T value) => _setter(holder, value);
    public T Lerp(T from, T to, float t) => _lerp(from, to, t);
}

/// <summary>
/// Provides convenience extension methods for creating tweens with <see cref="ITweenFactory{T,THolder}"/>.
/// </summary>
public static class TweenFactoryTweenExtensions {
    /// <summary>
    /// Applies the given tween factory to the tween.
    /// </summary>
    /// <seealso cref="ITweenFactory{T,THolder}.Apply"/>
    public static Tween<T> Apply<T, THolder>(this Tween<T> tween, ITweenFactory<T, THolder> factory, THolder holder) {
        return factory.Apply(tween, holder);
    }
    
    /// <summary>
    /// Creates a tween with a factory and sets its <see cref="Tween{T}.End"/> value.
    /// </summary>
    /// <param name="holder">
    /// The target object. Also used as the tween's owner (see <see cref="TweenManager.NewTween{T}"/>).
    /// </param>
    /// <param name="factory">The factory to use.</param>
    /// <param name="to">The <see cref="Tween{T}.End"/> value of the tween.</param>
    /// <typeparam name="T">The tween and target property's value type.</typeparam>
    /// <typeparam name="THolder">The target object's type.</typeparam>
    /// <returns>
    /// A newly created tween. If we are at runtime and the <see cref="TweenManager"/> is
    /// enabled, the tween is automatically ran by the manager. Otherwise, you will
    /// have to run it manually by calling <see cref="Tween{T}.Update"/> yourself
    /// (see <see cref="TweenBase"/> for more details).
    /// </returns>
    public static Tween<T> Tween<T, THolder>(this THolder holder, ITweenFactory<T, THolder> factory, T to) where THolder : Object {
        return holder.Tween(to).Apply(factory, holder);
    }
}

}
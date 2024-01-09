using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlowTween {

/// <summary>
/// Uses reflection to create tweens and tween factories for any property on any object.
/// </summary>
/// <remarks>
/// Significantly slower than using predefined <see cref="ITweenFactory{T,THolder}"/>s,
/// so if performance is a concern, look into that instead. This also doesn't offter the same
/// compile-time safety as using a factory.
/// </remarks>
public static class ReflectionTweenFactory {
    /// <inheritdoc cref="CreateTween{T,THolder}"/>
    public static Tween<T> Tween<T, THolder>(this THolder obj, string propertyName, T to) where THolder : Object {
        return CreateTween(obj, propertyName, to);
    }
    
    /// <summary>
    /// Creates a tween that animates a property on an object, using reflection.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property. The type must be float, <see cref="Vector2"/>, <see cref="Vector3"/>, <see cref="Quaternion"/> or <see cref="Color"/>.
    /// </param>
    /// <param name="to">The tween's <see cref="Tween{T}.End"/> value.</param>
    /// <typeparam name="T">The type of the resulting tween. Must be the same as the property and one of the supported types.</typeparam>
    /// <typeparam name="THolder">The target object's type.</typeparam>
    /// <returns>
    /// A newly created tween. If we are at runtime and the <see cref="TweenManager"/> is
    /// enabled, the tween is automatically ran by the manager. Otherwise, you will
    /// have to run it manually by calling <see cref="Tween{T}.Update"/> yourself
    /// (see <see cref="TweenBase"/> for more details).
    /// </returns>
    /// <remarks>
    /// This is significantly slower than using a predefined <see cref="ITweenFactory{T,THolder}"/>,
    /// so if performance is a concern, you should use those instead. It also doesn't have the same
    /// compile-time safety as using a factory.
    /// </remarks>
    public static Tween<T> CreateTween<T, THolder>(THolder holder, string propertyName, T to) where THolder : Object { 
        object factory;
        var propertyType = typeof(T);

        if (propertyType == typeof(float)) {
            factory = Create<FloatTweenFactory<THolder>>(propertyName);
        } else if (propertyType == typeof(Vector2)) {
            factory = Create<Vector2TweenFactory<THolder>>(propertyName);
        } else if (propertyType == typeof(Vector3)) {
            factory = Create<Vector3TweenFactory<THolder>>(propertyName);
        } else if (propertyType == typeof(Quaternion)) {
            factory = Create<QuaternionTweenFactory<THolder>>(propertyName);
        } else if (propertyType == typeof(Color)) {
            factory = Create<ColorTweenFactory<THolder>>(propertyName);
        } else {
            throw new ArgumentException($"Type {propertyType} is not supported");
        }
        
        return holder.Tween((ITweenFactory<T, THolder>)factory, to);
    }

    /// <summary>
    /// Creates a <see cref="TweenFactory{T,THolder}"/> that animates a property on an object, using reflection.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <typeparam name="T">The type of the property and factory.</typeparam>
    /// <typeparam name="THolder">The factory's target object's type.</typeparam>
    /// <typeparam name="TFactory">
    /// The type of factory to create. It has to have a constructor in the form of
    /// <code>ctor(Func&lt;THolder, T&gt; getter, Action&lt;THolder, T&gt; setter)</code>
    /// </typeparam>
    public static TFactory Create<T, THolder, TFactory>(string propertyName)
        where TFactory : TweenFactory<T, THolder>
    { 
        return (TFactory)CreateFactory(typeof(T), typeof(THolder), typeof(TFactory), propertyName);
    }
    
    /// <summary>
    /// Creates a <see cref="TweenFactory{T,THolder}"/> that animates a property on an object, using reflection.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <typeparam name="TFactory">
    /// The type of factory to create. It has to
    /// <list type="bullet">
    /// <item>inherit from <see cref="TweenFactory{T,THolder}"/></item>
    /// <item>have one generic argument for the holder type, which must inherit from <see cref="UnityEngine.Object"/></item>
    /// <item>have a constructor in the form of <code>ctor(Func&lt;THolder, T&gt; getter, Action&lt;THolder, T&gt; setter)</code></item>
    /// </list>
    /// </typeparam>
    public static TFactory Create<TFactory>(string propertyName) {
        var factoryType = typeof(TFactory);
        
        var baseType = factoryType.BaseType;
        if (baseType is not { IsGenericType: true } || baseType.GetGenericTypeDefinition() != typeof(TweenFactory<,>)) {
            throw new ArgumentException($"Type {factoryType} does not inherit from TweenFactory");
        }
        
        var propertyType = baseType.GetGenericArguments()[0];
        
        var genericArguments = factoryType.GetGenericArguments();
        if (genericArguments.Length != 1) {
            throw new ArgumentException($"Type {factoryType} does not have 1 generic arguments");
        }
        
        var holderType = genericArguments[0];
        if (!holderType.IsSubclassOf(typeof(Object))) {
            throw new ArgumentException($"Type {holderType} is not a subclass of UnityEngine.Object");
        }
        
        return (TFactory)CreateFactory(propertyType, holderType, factoryType, propertyName);
    }

    static object CreateFactory(Type propertyType, Type holderType, Type factoryType, string propertyName) {
        var constructor = factoryType.GetConstructor(new[] {
            GetGetterType(propertyType, holderType),
            GetSetterType(propertyType, holderType)
        });
        
        if (constructor == null) {
            throw new ArgumentException($"Type {factoryType} does not have a constructor with the required arguments");
        }
        
        var getter = CreateGetter(propertyType, holderType, propertyName);
        var setter = CreateSetter(propertyType, holderType, propertyName);
        
        return constructor.Invoke(new[] { getter, setter });
    }
    
    static object CreateSetter(Type propertyType, Type holderType, string propertyName) {
        var property = GetProperty(propertyType, holderType, propertyName);
        return Delegate.CreateDelegate(GetSetterType(propertyType, holderType), property.GetSetMethod());
    }
    
    static object CreateGetter(Type propertyType, Type holderType, string propertyName) {
        var property = GetProperty(propertyType, holderType, propertyName);
        return Delegate.CreateDelegate(GetGetterType(propertyType, holderType), property.GetGetMethod());
    }

    static PropertyInfo GetProperty(Type propertyType, Type holderType, string name) {
        var property = holderType.GetProperty(name);
        if (property == null) {
            throw new ArgumentException($"Property '{name}' not found on type {holderType}");
        }
        if (property.PropertyType != propertyType) {
            throw new ArgumentException($"Property '{name}' on type {holderType} is not of type {propertyType}");
        }
        return property;
    }
    
    static Type GetSetterType(Type propertyType, Type holderType) {
        return typeof(Action<,>).MakeGenericType(holderType, propertyType);
    }

    static Type GetGetterType(Type propertyType, Type holderType) {
        return typeof(Func<,>).MakeGenericType(holderType, propertyType);
    }
}

}
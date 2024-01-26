using System;
using System.Collections.Generic;
using System.Reflection;
using FlowTween.Components;
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
    static readonly Dictionary<Type, SupportedType> _supportedTypes = new() {
        { typeof(float), new SupportedType(typeof(FloatTweenFactory<>), typeof(FloatFromToTweenerTarget<>)) },
        { typeof(Vector2), new SupportedType(typeof(Vector2TweenFactory<>), typeof(Vector2FromToTweenerTarget<>)) },
        { typeof(Vector3), new SupportedType(typeof(Vector3TweenFactory<>), typeof(Vector3FromToTweenerTarget<>)) },
        { typeof(Quaternion), new SupportedType(typeof(QuaternionTweenFactory<>)) },
        { typeof(Color), new SupportedType(typeof(ColorTweenFactory<>), typeof(ColorFromToTweenerTarget<>)) },
    };
    
    public static ITweenerTarget CreateTweenerTarget<T, THolder>(string propertyName) where THolder : Object {
        return CreateTweenerTargetAutoTyped(GetProperty(propertyName, typeof(THolder), typeof(T)));
    }
    
    /// <inheritdoc cref="CreateTween{T,THolder}"/>
    public static Tween<T> Tween<T, THolder>(this THolder obj, string propertyName, T to) where THolder : Object {
        return CreateTween(obj, propertyName, to);
    }
    
    /// <summary>
    /// Creates a tween that animates a property on an object, using reflection.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="to">The tween's <see cref="Tween{T}.End"/> value.</param>
    /// <typeparam name="T">The type of the property. Must be float, <see cref="Vector2"/>, <see cref="Vector3"/>, <see cref="Quaternion"/> or <see cref="Color"/>.</typeparam>
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
        return holder.Tween(Create<T, THolder>(propertyName), to);
    }
    
    public static ITweenFactory<T, THolder> Create<T, THolder>(string propertyName) where THolder : Object {
        return (ITweenFactory<T, THolder>)CreateFactoryAutoTyped(GetProperty(propertyName, typeof(THolder), typeof(T)));
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
        return (TFactory)CreateFactoryWithType(propertyName, typeof(TFactory));
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
        return (TFactory)CreateFactoryWithType(GetProperty(propertyName, typeof(THolder), typeof(T)), typeof(TFactory));
    }

    internal static ITweenerTarget CreateTweenerTargetAutoTyped(PropertyInfo property) {
        var factory = CreateFactoryAutoTyped(property);
        var propertyType = property.PropertyType;
        
        if (!_supportedTypes.TryGetValue(propertyType, out var supportedType)) {
            throw new ArgumentException($"Type {propertyType} is not supported");
        }
        
        var tweenerTargetType = supportedType.TweenerTargetType.MakeGenericType(property.DeclaringType);
        return (ITweenerTarget)Activator.CreateInstance(tweenerTargetType, factory);
    }
    
    static object CreateFactoryAutoTyped(PropertyInfo property) {
        var propertyType = property.PropertyType;

        if (!_supportedTypes.TryGetValue(propertyType, out var supportedType)) {
            throw new ArgumentException($"Type {propertyType} is not supported");
        }
        
        return CreateFactoryWithType(property, supportedType.FactoryType.MakeGenericType(property.DeclaringType));
    }
    
    static object CreateFactoryWithType(string propertyName, Type factoryType) {
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
        
        return CreateFactoryWithType(GetProperty(propertyName, holderType, propertyType), factoryType);
    }

    static object CreateFactoryWithType(PropertyInfo property, Type factoryType) {
        var getterType = typeof(Func<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
        var setterType = typeof(Action<,>).MakeGenericType(property.DeclaringType, property.PropertyType);

        var constructor = factoryType.GetConstructor(new[] { getterType, setterType });
        
        if (constructor == null) {
            throw new ArgumentException($"Type {factoryType} does not have a constructor with the required arguments");
        }
        
        return constructor.Invoke(new object[] {
            Delegate.CreateDelegate(getterType, property.GetGetMethod()), 
            Delegate.CreateDelegate(setterType, property.GetSetMethod())
        });
    }
    
    static PropertyInfo GetProperty(string name, Type holderType, Type propertyType = null) {
        var property = holderType.GetProperty(name);
        if (property == null) {
            throw new ArgumentException($"Property '{name}' not found on type {holderType}");
        }
        if (propertyType != null && property.PropertyType != propertyType) {
            throw new ArgumentException($"Property '{name}' on type {holderType} is not of type {propertyType}");
        }
        return property;
    }

    struct SupportedType {
        public Type FactoryType;
        public Type TweenerTargetType;

        public SupportedType(Type factoryType = null, Type tweenerTargetType = null) {
            FactoryType = factoryType;
            TweenerTargetType = tweenerTargetType;
        }
    }
}

}
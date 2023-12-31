﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlowTween {
    
/// <summary>
/// Singleton for running and pooling <see cref="TweenBase"/> objects at runtime.
/// </summary>
public class TweenManager : MonoBehaviour {
    [SerializeField] bool _enabled = true;
    [SerializeField] bool _doNullChecks = true;
    [SerializeField] bool _hasMaxTweens = true;
    [SerializeField] int _maxTweens = 1000;
    
    static TweenManager _singleton;

    /// <summary>
    /// The singleton instance of the <see cref="TweenManager"/>.
    /// It's not recommended to use this directly, instead use <see cref="TweenManager.TryAccess"/>.
    /// </summary>
    [CanBeNull]
    public static TweenManager Singleton {
        get {
            if (!Application.isPlaying) {
                return null;
            }
            
            if (_singleton != null) return _singleton._enabled ? _singleton : null;
            _singleton = FindObjectOfType<TweenManager>();

            if (_singleton != null) return _singleton._enabled ? _singleton : null;
            _singleton = new GameObject("TweenManager").AddComponent<TweenManager>();
            DontDestroyOnLoad(_singleton);

            return _singleton;
        }
    }

    readonly Dictionary<Type, Queue<TweenBase>> _inactive = new();
    readonly List<TweenInstance> _active = new();

    /// <summary>
    /// Whether or not to do null checks on
    /// the owner of tweens every update. If a null owner
    /// is detected, the tween is cancelled automatically.
    ///
    /// <br/><br/>Defaults to true.
    /// </summary>
    public bool DoNullChecks {
        get => _doNullChecks;
        set => _doNullChecks = value;
    }

    /// <summary>
    /// The maximum number of tweens that can be created.
    /// Both inactive and active tweens count towards this number.
    /// Set to null to disable the limit.
    ///
    /// <br/><br/>Defaults to 1000.
    /// </summary>
    public int? MaxTweens {
        get => _hasMaxTweens ? _maxTweens : null;
        set {
            _hasMaxTweens = value.HasValue;
            _maxTweens = value ?? 0;
        }
    }
    
    /// <summary>
    /// The number of active tweens.
    /// </summary>
    public int ActiveTweenCount => _active.Count;
    
    /// <summary>
    /// The number of inactive tweens.
    /// </summary>
    public int InactiveTweenCount => _inactive.Values.Sum(q => q.Count);
    
    /// <summary>
    /// The total number of created tweens, both active and inactive.
    /// </summary>
    public int TotalTweenCount => ActiveTweenCount + InactiveTweenCount;

    void Update() {
        for (var i = 0; i < _active.Count; i++) {
            var instance = _active[i];
            var tween = instance.Tween;

            var isNull = DoNullChecks && instance.HasOwner && instance.Owner == null;
            if (!isNull) {
                tween.Update(tween.IgnoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime);
                if (!tween.IsComplete) continue;
            }
            
            tween.Cancel(!isNull);
            _active.RemoveAt(i);
            Return(tween);
            i--;
        }
    }

    void Return(TweenBase tween) {
        var type = tween.GetType();
        if (!_inactive.TryGetValue(type, out var queue)) {
            _inactive.Add(type, queue = new Queue<TweenBase>());
        }

        queue.Enqueue(tween);
    }

    /// <summary>
    /// Cancels a tween.
    /// </summary>
    /// <param name="tween">The tween to cancel.</param>
    /// <param name="callOnComplete">Whether or not to call the tween's <see cref="TweenBase.CompleteAction"/></param>
    public void Cancel(TweenBase tween, bool callOnComplete = false) {
        if (callOnComplete) tween.Cancel(false);
        _active.RemoveAll(t => t.Tween == tween);
        Return(tween);
    }

    /// <summary>
    /// Cancels all tweens with the given owner.
    /// </summary>
    /// <param name="obj">
    /// The owner of the tweens to cancel.
    /// Any tweens owned by components on this are also cancelled.
    /// </param>
    /// <param name="callOnComplete">Whether or not to call the tweens' <see cref="TweenBase.CompleteAction"/></param>
    public void CancelObject(GameObject obj, bool callOnComplete = false) {
        for (var i = 0; i < _active.Count; i++) {
            var instance = _active[i];
            if (instance.Owner != obj) continue;

            Cancel(instance.Tween, callOnComplete);
            i--;
        }
    }

    /// <summary>
    /// Gets a pooled instance of <typeparamref name="T"/>.
    /// Only use for advanced cases, otherwise use <see cref="NewTween{T}"/>
    /// or one of the extension methods.
    /// If there are any inactive instances of type <typeparamref name="T"/>,
    /// an instance is reset and returned.
    /// Otherwise, a new instance is created.
    /// </summary>
    /// <param name="owner">
    /// The owner of the tween, optional.
    /// If it's a <see cref="GameObject"/>, it will be used as the owner.
    /// If it's a <see cref="Component"/>, its <see cref="Component.gameObject"/>
    /// will be used as the owner.
    /// Otherwise, the tween will have no owner.
    /// </param>
    /// <typeparam name="T">A subclass of <see cref="TweenBase"/> to get.</typeparam>
    /// <exception cref="InvalidOperationException">
    /// The maximum number of tweens has been reached (see <see cref="MaxTweens"/>).
    /// </exception>
    public T Get<T>(Object owner = null) where T : TweenBase, new() {
        var gameObjectOwner = owner switch {
            GameObject o => o,
            Component c => c.gameObject,
            _ => null
        };
        
        T tween;
        var type = typeof(T);
        if (_inactive.TryGetValue(type, out var tweens) && tweens.TryDequeue(out var next)) {
            next.Reset();
            tween = (T)next;
        } else {
            if (MaxTweens.HasValue && TotalTweenCount >= MaxTweens) {
                throw new InvalidOperationException(
                    $"Max tween count reached ({MaxTweens}). This number can be changed in the TweenManager.");
            }

            tween = new T();
        }

        _active.Add(TweenInstance.From(tween, gameObjectOwner));
        return tween;
    }
    
    /// <summary>
    /// Gets a pooled instance of <see cref="Tween{T}"/>.
    /// If there are any inactive tweens of type <see cref="Tween{T}"/>,
    /// a tween is reset and returned.
    /// Otherwise, a new tween is created.
    /// </summary>
    /// <param name="owner">
    /// The owner of the tween, optional.
    /// If it's a <see cref="GameObject"/>, it will be used as the owner.
    /// If it's a <see cref="Component"/>, its <see cref="Component.gameObject"/>
    /// will be used as the owner.
    /// Otherwise, the tween will have no owner.
    /// </param>
    /// <typeparam name="T">The type of the tween to get.</typeparam>
    public Tween<T> NewTween<T>(Object owner = null) => Get<Tween<T>>(owner);
    
    /// <summary>
    /// Tries to get the singleton instance and run the given action on it.
    /// </summary>
    /// <returns>Whether or not the action was invoked.</returns>
    public static bool TryAccess(Action<TweenManager> action) {
        if (Singleton == null) return false;
        action(Singleton);
        return true;
    }
    
    /// <summary>
    /// Tries to get the singleton instance and run the given function on it.
    /// </summary>
    /// <param name="func">The function to run.</param>
    /// <param name="result">The result of the function, if it was invoked.</param>
    /// <typeparam name="T">The result type of <see cref="func"/>.</typeparam>
    /// <returns>Whether or not the function was invoked.</returns>
    public static bool TryAccess<T>(Func<TweenManager, T> func, out T result) {
        if (Singleton == null) {
            result = default;
            return false;
        }
        result = func(Singleton);
        return true;
    }

    readonly struct TweenInstance {
        public readonly TweenBase Tween;
        public readonly GameObject Owner;
        public readonly bool HasOwner;

        public TweenInstance(TweenBase tween, GameObject owner, bool hasOwner) {
            Tween = tween;
            Owner = owner;
            HasOwner = hasOwner;
        }
        
        public static TweenInstance From(TweenBase tween, GameObject owner) {
            return new TweenInstance(tween, owner, owner != null);
        }
    }
}

}
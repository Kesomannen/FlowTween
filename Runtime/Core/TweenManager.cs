using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlowTween {

public class TweenManager : MonoBehaviour {
    [SerializeField] bool _doNullChecks = true;
    [SerializeField] bool _hasMaxTweens = true;
    [SerializeField] int _maxTweens = 1000;
    
    static TweenManager _singleton;

    public static TweenManager Singleton {
        get {
            if (!Application.isPlaying) {
                return null;
            }
            
            if (_singleton != null) return _singleton;
            _singleton = FindObjectOfType<TweenManager>();

            if (_singleton != null) return _singleton;
            _singleton = new GameObject("TweenManager").AddComponent<TweenManager>();
            DontDestroyOnLoad(_singleton);

            return _singleton;
        }
    }

    readonly Dictionary<Type, Queue<TweenBase>> _inactive = new();
    readonly List<TweenInstance> _active = new();

    public bool DoNullChecks {
        get => _doNullChecks;
        set => _doNullChecks = value;
    }

    public int? MaxTweens {
        get => _hasMaxTweens ? _maxTweens : null;
        set {
            _hasMaxTweens = value.HasValue;
            _maxTweens = value ?? 0;
        }
    }
    
    public int ActiveTweenCount => _active.Count;
    public int InactiveTweenCount => _inactive.Values.Sum(q => q.Count);
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
            
            tween.OnComplete(isNull);
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

    public void Cancel(TweenBase tween, bool callOnComplete = false) {
        if (callOnComplete) tween.OnComplete(true);
        _active.RemoveAll(t => t.Tween == tween);
        Return(tween);
    }

    public void CancelObject(GameObject obj, bool callOnComplete = false) {
        for (var i = 0; i < _active.Count; i++) {
            var instance = _active[i];
            if (instance.Owner != obj) continue;

            Cancel(instance.Tween, callOnComplete);
            i--;
        }
    }

    T Get<T>(Object owner = null) where T : TweenBase, new() {
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
    
    public Tween<T> NewTween<T>(Object owner = null) => Get<Tween<T>>(owner);
    
    public static bool TryAccess(Action<TweenManager> action) {
        if (Singleton == null) return false;
        action(Singleton);
        return true;
    }
    
    public static bool TryAccess<T>(Func<TweenManager, T> action, out T result) {
        if (Singleton == null) {
            result = default;
            return false;
        }
        result = action(Singleton);
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
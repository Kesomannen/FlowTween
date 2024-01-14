using System;
using System.Linq;
using FlowTween.Templates;
using UnityEngine;
using UnityEngine.Events;

namespace FlowTween.Components {
    
/// <summary>
/// A serializable configuration for a <see cref="Tweener"/> target.
/// Used by <see cref="Tweener"/>, but can also be used standalone.
///
/// <br/><br/>If you serialize this in the inspector, make sure to call <see cref="Validate"/>
/// and <see cref="DrawGizmos"/> appropriately. You may also want to handle <see cref="PlayOnEnable"/>,
/// <see cref="PlayOnDisable"/> and <see cref="ResetOnDisable"/>, as these are not handled by the configuration itself.
/// </summary>
[Serializable]
public class TweenerTargetConfig {
    [SerializeField] string _targetId = FallbackTargetId;
    [SerializeField] string _prevTargetId = FallbackTargetId;
    [SerializeReference] object _data;
    [SerializeField] TweenSettingsProperty _settings = new();
    [SerializeField] bool _playOnEnable;
    [SerializeField] bool _playOnDisable;
    [SerializeField] bool _resetOnDisable = true;
    [SerializeField] bool _ignoreTimescale;
    [SerializeField] UnityEvent _onStart;
    [SerializeField] UnityEvent _onComplete;
    [SerializeField] bool _selected;

    /// <summary>
    /// Is this target expanded in the inspector?
    /// </summary>
    public bool IsSelected => _selected;

    /// <summary>
    /// The id of the target, used to find a <see cref="ITweenerTarget"/> in <see cref="TweenerTargets"/>.
    /// To set this, use <see cref="SetTargetId"/> or <see cref="SetTarget{T,THolder,TData}"/> instead.
    /// </summary>
    public string TargetId {
        get {
            GetTarget();
            return _targetId;
        }
    }
    
    Component _cachedComponent;
    Type _cachedComponentType;
    object _snapshot;
    
    /// <summary>
    /// The id of the fallback target, used when <see cref="TargetId"/> is null or not found in <see cref="TweenerTargets"/>.
    /// </summary>
    public const string FallbackTargetId = "TransformPosition"; 

    /// <summary>
    /// The tween settings to apply to the resulting tween.
    /// </summary>
    public TweenSettingsProperty Settings {
        get => _settings;
        set => _settings = value;
    }

    /// <summary>
    /// Whether or not to play the tween whenever OnEnable is called.
    /// </summary>
    /// <remarks>
    /// This is not handled by the configuration itself, but by the <see cref="Tweener"/> component.
    /// </remarks>
    public bool PlayOnEnable {
        get => _playOnEnable;
        set => _playOnEnable = value;
    }
    
    /// <summary>
    /// Whether or not to play the tween whenever the component or
    /// game object is disabled using the <see cref="Disabling"/> system.
    /// </summary>
    /// <remarks>
    /// This is not handled by the configuration itself, but by the <see cref="Tweener"/> component.
    /// </remarks>
    public bool PlayOnDisable {
        get => _playOnDisable;
        set => _playOnDisable = value;
    }
    
    /// <summary>
    /// Whether or not the target property should be reset to its original value
    /// after an OnDisable tween has finished. Only applies if <see cref="PlayOnDisable"/>
    /// is true.
    /// </summary>
    /// <remarks>
    /// This is not handled by the configuration itself, but by the <see cref="Tweener"/> component.
    /// </remarks>
    public bool ResetOnDisable {
        get => _resetOnDisable;
        set => _resetOnDisable = value;
    }

    public TweenerTargetConfig() {
        RefreshTarget();
    }

    public TweenerTargetConfig(string targetId, object data) {
        SetTargetId(targetId, data);
    }
    
    /// <summary>
    /// Creates a new tween from this configuration.
    /// </summary>
    public TweenBase GetTween(GameObject gameObject) {
        var target = GetTarget();
        var component = GetComponent(gameObject);
        if (component != null) {
            _onStart.Invoke();
            
            return target.GetTween(component, _data)
                .SetIgnoreTimescale(_ignoreTimescale)
                .OnComplete(_onComplete.Invoke)
                .Apply(Settings.Value);
        }
        
        Debug.LogError($"Target component of type {target.ComponentType} is missing", component);
        return null;
    }

    /// <summary>
    /// Takes a snapshot of the target property.
    /// </summary>
    public void TakeSnapshot(GameObject gameObject) {
        _snapshot = GetTarget().TakeSnapshot(GetComponent(gameObject));
    }

    /// <summary>
    /// Applies a snapshot of the previously captured value.
    /// Make sure to call <see cref="TakeSnapshot"/> before calling this.
    /// </summary>
    public void ApplySnapshot(GameObject gameObject) {
        if (_snapshot == null) return;
        GetTarget().ApplySnapshot(GetComponent(gameObject), _snapshot);
        _snapshot = null;
    }

    /// <summary>
    /// Draws gizmos for a game object, if <see cref="IsSelected"/> is true.
    /// Should be called from OnDrawGizmosSelected.
    /// </summary>
    public void DrawGizmos(GameObject gameObject) {
        if (!_selected) return;
        
        var component = GetComponent(gameObject, false);
        if (component == null) return;
        
        GetTarget().OnDrawGizmos(component, _data);
    }

    /// <summary>
    /// Validates the target component and data.
    /// Should be called from OnValidate.
    /// </summary>
    public void Validate(GameObject gameObject) {
        if (_targetId == _prevTargetId) return;
        RefreshTarget();
        GetComponent(gameObject);
    }

    /// <summary>
    /// Sets the <see cref="TargetId"/> and tween data.
    /// </summary>
    /// <param name="targetId">
    /// An id to retrieve a <see cref="ITweenerTarget"/> from <see cref="TweenerTargets"/>.
    /// If null or not found, <see cref="FallbackTargetId"/> is used instead.
    /// </param>
    /// <param name="data">
    /// The data to use for the target.
    /// This must be of the same type as the data type of the target.
    /// You can get the data type by calling <see cref="ITweenerTarget.GetData"/>
    /// and checking its type, or skip that and use the type safe <see cref="SetTarget{T,THolder,TData}"/> instead.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="data"/> was not the correct type.</exception>
    public TweenerTargetConfig SetTargetId(string targetId, object data) {
        if (targetId == _targetId) return this;
        
        _targetId = targetId;
        RefreshTarget();
        if (_data.GetType() == data.GetType()) {
            _data = data;
        } else {
            throw new ArgumentException($"Data type mismatch: {_data.GetType()} != {data.GetType()}");
        }

        return this;
    }

    /// <summary>
    /// Sets the tween data and <see cref="TargetId"/> using a target instance.
    /// </summary>
    /// <param name="target">
    /// The target to use. Must be present in <see cref="TweenerTargets"/>.
    /// </param>
    /// <param name="data">
    /// The data to use for the target.
    /// This must be of the same type as the data type of the target.
    /// You can get the data type by calling <see cref="ITweenerTarget.GetData"/>
    /// and checking its type, or skip that and use the type safe <see cref="SetTarget{T,THolder,TData}"/> instead.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="target"/> was not present in <see cref="TweenerTargets"/>.</exception>
    public TweenerTargetConfig SetTargetUntyped(ITweenerTarget target, object data) {
        var id = TweenerTargets.Targets.FirstOrDefault(pair => pair.Value == target).Key;
        if (id == null) {
            throw new ArgumentException($"Target {target} is not registered in TweenerTargets");
        }
        return SetTargetId(id, data);
    }
    
    /// <summary>
    /// Sets the tween data and <see cref="TargetId"/> using a target instance.
    /// </summary>
    /// <param name="target">The target to use. Must be present in <see cref="TweenerTargets"/>.</param>
    /// <param name="data">The data to use for the target.</param>
    /// <exception cref="ArgumentException"><paramref name="target"/> was not present in <see cref="TweenerTargets"/>.</exception>
    public TweenerTargetConfig SetTarget<T, THolder, TData>(ITweenerTarget<T, THolder, TData> target, TData data) 
        where THolder : Component 
        where TData : class
    {
        return SetTargetUntyped(target, data);
    }

    void RefreshTarget() {
        _prevTargetId = _targetId;
        _data = GetTarget().GetData();
    }

    ITweenerTarget GetTarget() {
        if (_targetId != null && TweenerTargets.Targets.TryGetValue(_targetId, out var target)) {
            return target;
        }
            
        _targetId = FallbackTargetId;
        return TweenerTargets.Targets[_targetId];
    }

    Component GetComponent(GameObject gameObject, bool logError = true) {
        var target = GetTarget();
        if (_cachedComponent != null &&
            _cachedComponentType == target.ComponentType &&
            _cachedComponent.gameObject == gameObject
        ) return _cachedComponent;
        
        _cachedComponentType = target.ComponentType;
        _cachedComponent = gameObject.GetComponent(target.ComponentType);
        if (_cachedComponent != null) return _cachedComponent;

        if (logError) {
            Debug.LogWarning($"Tween type {_targetId} requires component of type {_cachedComponentType}", gameObject);
        }
        return null;
    }
}

}
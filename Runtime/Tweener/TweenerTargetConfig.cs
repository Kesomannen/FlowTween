using System;
using System.Linq;
using FlowTween.Templates;
using UnityEngine;

namespace FlowTween.Components {
    
[Serializable]
public class TweenerTargetConfig {
    [SerializeField] string _targetId = FallbackTargetId;
    [SerializeField] string _prevTargetId = FallbackTargetId;
    [SerializeReference] object _data;
    [SerializeField] TweenSettingsProperty _settings;
    [SerializeField] bool _playOnEnable;
    [SerializeField] bool _playOnDisable;
    [SerializeField] bool _resetOnDisable = true;
    [SerializeField] bool _selected;

    public bool IsSelected => _selected;

    public string TargetId {
        get {
            GetTarget();
            return _targetId;
        }
    }
    
    Component _cachedComponent;
    Type _cachedComponentType;
    object _snapshot;
    
    const string FallbackTargetId = "TransformPosition"; 

    public TweenSettingsProperty Settings {
        get => _settings;
        set => _settings = value;
    }

    public bool PlayOnEnable {
        get => _playOnEnable;
        set => _playOnEnable = value;
    }
    
    public bool PlayOnDisable {
        get => _playOnDisable;
        set => _playOnDisable = value;
    }
    
    public bool ResetOnDisable {
        get => _resetOnDisable;
        set => _resetOnDisable = value;
    }

    public TweenBase GetTween(Component component) {
        var target = GetTarget();
        if (component != null) {
            return target.GetTween(component, _data).Apply(Settings.Value);
        }
        
        Debug.LogError($"Target component of type {target.ComponentType} is missing", component);
        return null;
    }
    
    public TweenBase GetTween(GameObject gameObject) {
        return GetTween(GetComponent(gameObject));
    }

    public void TakeSnapshot(Component component) {
        _snapshot = GetTarget().TakeSnapshot(component);
    }

    public void TakeSnapshot(GameObject gameObject) {
        TakeSnapshot(GetComponent(gameObject));
    }

    public void ApplySnapshot(Component component) {
        if (_snapshot == null) return;
        GetTarget().ApplySnapshot(component, _snapshot);
        _snapshot = null;
    }

    public void ApplySnapshot(GameObject gameObject) {
        ApplySnapshot(GetComponent(gameObject));
    }

    public void DrawGizmos(Component component) {
        if (!_selected) return;
        GetTarget().OnDrawGizmos(component, _data);
    }

    public void DrawGizmos(GameObject gameObject) {
        var component = GetComponent(gameObject, false);
        if (component == null) return;
        DrawGizmos(component);
    }

    public void Init() {
        RefreshTarget();
    }

    public void Validate(GameObject gameObject) {
        if (_targetId == _prevTargetId) return;
        RefreshTarget();
        GetComponent(gameObject);
    }

    public void SetTargetId(string targetId, object data) {
        if (targetId == _targetId) return;
        
        _targetId = targetId;
        RefreshTarget();
        if (_data.GetType() == data.GetType()) {
            _data = data;
        } else {
            throw new ArgumentException($"Data type mismatch: {_data.GetType()} != {data.GetType()}");
        }
    }

    public void SetTargetUntyped(ITweenerTarget target, object data) {
        var id = TweenerTargets.Targets.FirstOrDefault(pair => pair.Value == target).Key;
        if (id == null) {
            throw new ArgumentException($"Target {target} is not registered in TweenerTargets");
        }
        SetTargetId(id, data);
    }
    
    public void SetTarget<T, THolder, TData>(ITweenerTarget<T, THolder, TData> target, TData data) 
        where THolder : Component 
        where TData : class
    {
        SetTargetUntyped(target, data);
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
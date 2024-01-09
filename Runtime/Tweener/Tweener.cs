using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace FlowTween.Components {

/// <summary>
/// Component for creating tweens in the editor.
/// </summary>
public class Tweener : MonoBehaviour, IOnDisableRoutine {
    [SerializeField] private List<TweenerTargetConfig> _tweens = new();
    [SerializeField] bool _preventOverlapping;
    [SerializeField] bool _sequential;
    
    /// <summary>
    /// List of tween configurations in this tweener.
    /// </summary>
    public List<TweenerTargetConfig> Tweens => _tweens;
    
    /// <summary>
    /// Whether or not to prevent multiple tweens from running at the same time,
    /// unless they were started at the same time.
    /// </summary>
    public bool PreventOverlapping {
        get => _preventOverlapping;
        set => _preventOverlapping = value;
    }
    
    public bool Sequential {
        get => _sequential;
        set => _sequential = value;
    }

    Queue<TweenQueueItem> _tweenQueue;

    struct TweenQueueItem {
        public TweenerTargetConfig Config;
        public Action<TweenBase> Callback;
    }

    void OnEnable() {
        PlayTweens(_tweens.Where(tween => tween.PlayOnEnable));
    }

    void OnValidate() {
        foreach (var tween in _tweens) {
            tween.Validate(gameObject);
        }
    }

    void Reset() {
        _tweens.Clear();
        AddTween(out _);
    }

    void OnDrawGizmosSelected() {
        foreach (var tween in _tweens) {
            tween.DrawGizmos(gameObject);
        }
    }

    /// <summary>
    /// Plays the tween at the given index of the <see cref="Tweens"/> list.
    /// </summary>
    /// <remarks>
    /// Does not return anything to be compatible with <see cref="UnityEvent"/>s.
    /// If you need to get the resulting tween, use <see cref="PlayTween(int)"/> instead.
    /// </remarks>
    public void Play(int index) {
        PlayTween(index);
    }

    /// <summary>
    /// Plays all tweens in this tweener.
    /// </summary>
    /// <remarks>
    /// Does not return anything to be compatible with <see cref="UnityEvent"/>s
    /// and to avoid unnecessary allocations.
    /// </remarks>
    public void PlayAll() {
        PlayTweens(_tweens);
    }

    /// <summary>
    /// Plays the tween at the given index of the <see cref="Tweens"/> list.
    /// </summary>
    public TweenBase PlayTween(int index, Action<TweenBase> callback = null) {
        BeforePlay();
        return PlayTweenInternal(index, callback);
    }
    
    /// <summary>
    /// Plays the given tween configuration.
    /// </summary>
    /// <returns>The newly created tween, or null if <see cref="Sequential"/> is true.</returns>
    public TweenBase PlayTween(TweenerTargetConfig tween, Action<TweenBase> callback = null) {
        BeforePlay();
        return PlayTweenInternal(tween, callback);
    }

    /// <summary>
    /// Plays the given tween configurations.
    /// </summary>
    public void PlayTweens(IEnumerable<TweenerTargetConfig> tweens, Action<TweenBase> callback = null) {
        BeforePlay();
        foreach (var tween in tweens) {
            PlayTweenInternal(tween, callback);
        }
    }
    
    /// <summary>
    /// Plays the tweens at the given indices of the <see cref="Tweens"/> list.
    /// </summary>
    public void PlayTweens(IEnumerable<int> indices, Action<TweenBase> callback = null) {
        BeforePlay();
        foreach (var index in indices) {
            PlayTweenInternal(index, callback);
        }
    }

    /// <summary>
    /// Plays the given tween configurations and returns the resulting tweens.
    /// </summary>
    public TweenBase[] PlayTweensAndReturn(IEnumerable<TweenerTargetConfig> tweens, Action<TweenBase> callback = null) {
        BeforePlay();
        return tweens.Select(config => PlayTweenInternal(config, callback)).ToArray();
    }

    /// <summary>
    /// Plays the tweens at the given indices of the <see cref="Tweens"/> list
    /// and returns the resulting tweens.
    /// </summary>
    public TweenBase[] PlayTweensAndReturn(IEnumerable<int> indices, Action<TweenBase> callback = null) {
        BeforePlay();
        return indices.Select(index => PlayTweenInternal(index, callback)).ToArray();
    }

    TweenBase PlayTweenInternal(int index, Action<TweenBase> callback) {
        if (index >= 0 && index < _tweens.Count) {
            PlayTweenInternal(_tweens[index], callback);
        }
        
        Debug.LogWarning($"Tried to play tween at index out of range: {index}", this);
        return null;
    }
    
    TweenBase PlayTweenInternal(TweenerTargetConfig config, Action<TweenBase> callback) {
        if (!_tweens.Contains(config)) {
            Debug.LogWarning("Tried to play a tween that is not owned by this tweener", this);
            return null;
        }

        if (!_sequential) {
            var tween = config.GetTween(gameObject);
            callback?.Invoke(tween);
            return tween;
        }
        
        _tweenQueue ??= new Queue<TweenQueueItem>();
        _tweenQueue.Enqueue(new TweenQueueItem {
            Config = config,
            Callback = callback
        });
        if (_tweenQueue.Count == 1) {
            StartCoroutine(PlaySequentialLoop());
        }
        
        return null;
    }

    void BeforePlay() {
        if (_preventOverlapping) {
            gameObject.CancelTweens();
        }
    }

    IEnumerator PlaySequentialLoop() {
        while (_tweenQueue.TryPeek(out var next)) {
            var tween = next.Config.GetTween(gameObject);
            next.Callback?.Invoke(tween);
            yield return tween;
            _tweenQueue.Dequeue();
        }
    }

    /// <summary>
    /// Adds a tween configuration to this tweener.
    /// </summary>
    /// <returns>The index of the tween in the <see cref="Tweens"/> list.</returns>
    public int AddTween(TweenerTargetConfig config) {
        _tweens.Add(config);
        return _tweens.Count - 1;
    }

    public TweenBase AddTweenAndPlay(TweenerTargetConfig config, Action<TweenBase> callback = null) {
        return PlayTween(AddTween(config), callback);
    }
    
    /// <summary>
    /// Creates a new tween and adds it to this tweener.
    /// </summary>
    public TweenerTargetConfig AddTween(out int index) {
        var tween = new TweenerTargetConfig();
        index = AddTween(tween);
        return tween;
    }
    
    /// <summary>
    /// Removes the given tween from this tweener.
    /// </summary>
    public bool RemoveTween(TweenerTargetConfig config) {
        return _tweens.Remove(config);
    }
    
    /// <summary>
    /// Removes the tween at the given index from this tweener.
    /// </summary>
    public bool RemoveTween(int index) {
        if (index < 0 || index >= _tweens.Count)
            return false;
        
        _tweens.RemoveAt(index);
        return true;
    }

    IEnumerator IOnDisableRoutine.OnDisableRoutine() {
        if (!_tweens.Any(tween => tween.PlayOnDisable)) yield break;
        var toPlay = _tweens.Where(tween => tween.PlayOnDisable).ToArray();
        
        gameObject.CancelTweens();
        
        foreach (var config in toPlay.Where(t => t.ResetOnDisable)) {
            config.TakeSnapshot(gameObject);
        }
        
        var completed = 0;
        var tweens = PlayTweensAndReturn(toPlay);
        foreach (var tween in tweens) {
            StartCoroutine(AwaitTween(tween));
        }
        
        yield return new WaitUntil(() => completed == tweens.Length);
        yield break;

        IEnumerator AwaitTween(TweenBase tween) {
            yield return tween;
            completed++;
            
            if (completed == tweens.Length) {
                foreach (var config in toPlay.Where(t => t.ResetOnDisable)) {
                    config.ApplySnapshot(gameObject);
                }
            }
        }
    }
    
    void OnDisable() {
        _tweenQueue?.Clear();
    }
}

}
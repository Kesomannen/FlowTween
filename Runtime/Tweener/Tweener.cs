using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlowTween.Components {

public class Tweener : MonoBehaviour, IOnDisableRoutine {
    [SerializeField] private List<TweenerTargetConfig> _tweens = new();
    [SerializeField] bool _preventOverlapping;
    
    public List<TweenerTargetConfig> Tweens => _tweens;
    
    public bool PreventOverlapping {
        get => _preventOverlapping;
        set => _preventOverlapping = value;
    }

    void OnEnable() {
        if (_tweens.Any(tween => tween.PlayOnEnable)) {
            PlayTweens(_tweens.Where(tween => tween.PlayOnEnable));
        }
    }

    void OnValidate() {
        foreach (var tween in _tweens) {
            tween.Validate(gameObject);
        }
    }

    void Reset() {
        _tweens.Clear();
        AddTween();
    }

    void OnDrawGizmosSelected() {
        foreach (var tween in _tweens) {
            tween.DrawGizmos(gameObject);
        }
    }

    public void Play(int index) {
        PlayTween(index);
    }

    public void PlayAll() {
        PlayTweens(_tweens);
    }

    public TweenBase PlayTween(TweenerTargetConfig tween) {
        BeforePlay();
        return PlayTweenInternal(tween);
    }

    public TweenBase PlayTween(int index) {
        BeforePlay();
        return PlayTweenInternal(index);
    }
    
    public void PlayTweens(IEnumerable<TweenerTargetConfig> tweens) {
        BeforePlay();
        foreach (var tween in tweens) {
            PlayTweenInternal(tween);
        }
    }
    
    public void PlayTweens(IEnumerable<int> indices) {
        BeforePlay();
        foreach (var index in indices) {
            PlayTweenInternal(index);
        }
    }

    public TweenBase[] PlayTweensAndReturn(IEnumerable<TweenerTargetConfig> tweens) {
        BeforePlay();
        return tweens.Select(PlayTweenInternal).ToArray();
    }

    public TweenBase[] PlayTweensAndReturn(IEnumerable<int> indices) {
        BeforePlay();
        return indices.Select(PlayTweenInternal).ToArray();
    }

    TweenBase PlayTweenInternal(TweenerTargetConfig tween) {
        if (_tweens.Contains(tween)) return tween.GetTween(gameObject);
        
        Debug.LogWarning("Tried to play a tween that is not owned by this tweener", this);
        return null;
    }
    
    TweenBase PlayTweenInternal(int index) {
        if (index >= 0 && index < _tweens.Count) return _tweens[index].GetTween(gameObject);
        
        Debug.LogWarning($"Tried to play tween at index out of range: {index}", this);
        return null;
    }

    void BeforePlay() {
        if (_preventOverlapping) {
            gameObject.CancelTweens();
        }
    }

    public int AddTween(TweenerTargetConfig tween) {
        _tweens.Add(tween);
        tween.Init();
        return _tweens.Count - 1;
    }
    
    public TweenerTargetConfig AddTween() {
        var tween = new TweenerTargetConfig();
        AddTween(tween);
        return tween;
    }
    
    public bool RemoveTween(TweenerTargetConfig tween) {
        return _tweens.Remove(tween);
    }
    
    public bool RemoveTween(int index) {
        if (index < 0 || index >= _tweens.Count)
            return false;
        
        _tweens.RemoveAt(index);
        return true;
    }
    
    public IEnumerator OnDisableRoutine() {
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
}

}
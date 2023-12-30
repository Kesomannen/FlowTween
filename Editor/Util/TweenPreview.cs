using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace FlowTween.Editor {

/// <summary>
/// A helper class for previewing tweens in the editor.
/// </summary>
public class TweenPreview {
    TweenBase[] _tweens;
    int _playCount;
    float _lastUpdate;
    
    /// <summary>
    /// Are any tweens currently playing?
    /// </summary>
    public bool IsPlaying { get; private set; }

    static float Time => (float)EditorApplication.timeSinceStartup;

    /// <summary>
    /// Called when a tween is started or stopped.
    /// </summary>
    public event Action<TweenBase> OnTweenStoppedOrRestarted;

    /// <summary>
    /// Called when the preview starts playing.
    /// </summary>
    public event Action OnStartedPlaying;
    
    /// <summary>
    /// Called when all tweens have stopped playing.
    /// </summary>
    public event Action OnStoppedPlaying;

    public TweenPreview() { }
    
    /// <summary>
    /// Creates a new preview and attaches it to the given element,
    /// making sure to stop all tweens when the element is detached.
    /// </summary>
    public TweenPreview(VisualElement element) {
        Attach(element);
    }
    
    /// <summary>
    /// Plays the given tweens.
    /// If any tweens are already playing, they will be stopped
    /// or restarted, depending on if they are in the new list.
    /// </summary>
    public void Play(params TweenBase[] tweens) {
        _tweens = tweens; 
        _playCount = tweens.Length;
        
        if (!IsPlaying) {
            StartPlaying();
        } else {
            foreach (var tween in tweens) {
                OnTweenStoppedOrRestarted?.Invoke(tween);
            }
        }
    }

    void Update() {
        var delta = Time - _lastUpdate;
        _lastUpdate = Time;

        for (var i = 0; i < _tweens.Length; i++) {
            
            var tween = _tweens[i];
            if (tween == null) continue;

            tween.Update(delta);
            if (!tween.IsComplete) continue;
            
            Stop(i);
            if (!IsPlaying) return;
        }
    }

    /// <summary>
    /// Stops the tween at the given index.
    /// </summary>
    public void Stop(int tweenIndex) {
        if (!IsPlaying) return;
        
        _playCount--;

        try {
            OnTweenStoppedOrRestarted?.Invoke(_tweens[tweenIndex]);
        } finally {
            _tweens[tweenIndex] = null;

            if (_playCount <= 0) {
                StopPlaying();
            }
        }
    }

    /// <summary>
    /// Stops all tweens.
    /// </summary>
    public void StopAll() {
        if (!IsPlaying) return;
        foreach (var tween in _tweens) {
            OnTweenStoppedOrRestarted?.Invoke(tween);
        }
        StopPlaying();
    }

    void StartPlaying() {
        if (IsPlaying) return;
        
        IsPlaying = true;
        _lastUpdate = Time;
        EditorApplication.update += Update;
        
        OnStartedPlaying?.Invoke();
    }

    void StopPlaying() {
        if (!IsPlaying) return;
        
        IsPlaying = false;
        EditorApplication.update -= Update;
        _tweens = null;
        
        OnStoppedPlaying?.Invoke();
    }

    /// <summary>
    /// Attaches this preview to the given element,
    /// making sure to stop all tweens when the element is detached.
    /// </summary>
    public void Attach(VisualElement element) {
        element.RegisterCallback<DetachFromPanelEvent>(_ => StopAll());
    }
}

}
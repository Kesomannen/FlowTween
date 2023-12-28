using System;
using UnityEditor;

namespace FlowTween.Editor {

public class TweenPreview {
    TweenBase[] _tweens;
    int _playCount;
    float _lastUpdate;
    
    public bool IsPlaying { get; private set; }

    static float Time => (float)EditorApplication.timeSinceStartup;

    public event Action<TweenBase> OnTweenStoppedOrRestarted;
    public event Action OnStartedPlaying, OnStoppedPlaying;
    
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
}

}
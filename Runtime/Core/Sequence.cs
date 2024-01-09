using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace FlowTween {

/// <summary>
/// A sequence of actions and tweens.
/// </summary>
public class Sequence : Runnable {
    readonly List<Item> _items = new();

    int _lastItemIndex = -1;

    public override float Duration => _items.Sum(i => i.Duration);

    protected override void OnUpdate(float deltaTime) {
        if (_items.Count == 0) return;
        
        var newItemIndex = GetItemIndex(Progress);

        while (_lastItemIndex < newItemIndex) {
            _lastItemIndex++;
            _items[_lastItemIndex].Action?.Invoke();
        }
    }
    
    int GetItemIndex(float progress) {
        var totalTime = Duration;
        var t = 0f;

        for (int i = 0; i < _items.Count; i++) {
            if (_items[i].Overlay) continue;
             
            t += _items[i].Duration / totalTime;
            if (t <= progress) continue;
            
            while (i < _items.Count - 1 && _items[i + 1].Overlay) {
                i++;
            }
            return i;
        }

        return _items.Count - 1;
    }

    /// <summary>
    /// Adds an item to the sequence.
    /// </summary>
    public Sequence Add(Item item) {
        _items.Add(item);
        return this;
    }
    
    /// <summary>
    /// Adds a delay to the sequence.
    /// </summary>
    public Sequence AddDelay(float duration) {
        return Add(new Item { Duration = duration });
    }
    
    /// <summary>
    /// Adds an action to the sequence.
    /// </summary>
    public Sequence Add(Action action, bool overlay = false) {
        return Add(new Item {
            Action = action,
            Overlay = overlay
        });
    }
    
    /// <summary>
    /// Adds a runnable (a tween or another sequence) to this sequence,
    /// which is created now and then paused until it's time to play.
    /// </summary>
    /// <param name="overlay">If true, makes the runnable run at the same time as the previous item.</param>
    /// <remarks>
    /// Be cautios when using this method, as it can cause unexpected
    /// behavior if the tween's target properties are changed after
    /// the tween is created.
    /// </remarks>
    public Sequence AddNow(Runnable runnable, bool overlay = false) {
        runnable.IsPaused = true;
        return Add(new Item {
            Duration = runnable.TotalDuration, 
            Action = () => runnable.IsPaused = false,
            Overlay = overlay
        });
    }

    /// <summary>
    /// Adds a tween to the sequence, which is created from <paramref name="createTween"/>
    /// when it's time to play. The duration and delay of the tween
    /// are set automatically from the parameters.
    /// </summary>
    public Sequence Add<T>(Func<Tween<T>> createTween, float duration, float delay = 0, bool overlay = false) {
        return Add(new Item {
            Duration = duration + delay,
            Action = () => createTween().SetDuration(duration).SetDelay(delay),
            Overlay = overlay
        });
    }
    
    /// <summary>
    /// Adds a tween to the sequence, which is created from <paramref name="createTween"/>
    /// when it's time to play. The <paramref name="settings"/> are automatically applied
    /// to the tween.
    /// </summary>
    public Sequence Add<T>(Func<Tween<T>> createTween, TweenSettings settings, bool overlay = false) {
        return Add(new Item {
            Duration = settings.Duration + settings.Delay,
            Action = () => createTween().Apply(settings),
            Overlay = overlay
        });
    }

    /// <summary>
    /// An item in a sequence.
    /// </summary>
    public struct Item {
        float _duration;

        /// <summary>
        /// The duration of the item. Must be greater than or equal to 0.
        /// </summary>
        public float Duration {
            get => _duration;
            set => _duration = Mathf.Max(0, value);
        }

        /// <summary>
        /// If true, the item will run at the same time as the previous item.
        /// </summary>
        public bool Overlay;
        
        /// <summary>
        /// An action to invoke when the item starts playing.
        /// </summary>
        [CanBeNull]
        public Action Action;
    }
}

}
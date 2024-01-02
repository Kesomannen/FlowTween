using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace FlowTween {

public class Sequence : Runnable {
    readonly List<Item> _items = new();

    int _lastItemIndex = -1;

    public override float Duration => _items.Sum(i => i.Duration);

    protected override void OnUpdate(float deltaTime) {
        if (_items.Count == 0) return;
        
        var newItemIndex = GetItemIndex(Progress);

        if (_lastItemIndex == newItemIndex) return;
        
        _lastItemIndex = newItemIndex;
        _items[newItemIndex].Action?.Invoke();
    }
    
    int GetItemIndex(float progress) {
        var totalTime = Duration;
        var t = 0f;

        for (int i = 0; i < _items.Count; i++) {
            t += _items[i].Duration / totalTime;
            if (t > progress) return i;
        }

        return _items.Count - 1;
    }

    public Sequence Add(Item item) {
        _items.Add(item);
        return this;
    }
    
    public Sequence AddDelay(float duration) {
        return Add(new Item { Duration = duration });
    }
    
    public Sequence Add(Action action) {
        return Add(new Item { Action = action });
    }
    
    public Sequence AddNow<T>(Tween<T> tween) {
        tween.IsPaused = true;
        return Add(new Item {
            Duration = tween.Duration, 
            Action = () => tween.IsPaused = false
        });
    }

    public Sequence Add<T>(Func<Tween<T>> factory, float duration) {
        return Add(new Item {
            Duration = duration,
            Action = () => factory().SetDuration(duration)
        });
    }
    
    public Sequence Add<T, THolder>(
        THolder obj, 
        ITweenFactory<T, THolder> factory, 
        T to,
        float duration, 
        Action<Tween<T>> extraSetup = null
    ) where THolder : Object {
        return Add(() => {
            var tween = obj.Tween(factory, to);
            extraSetup?.Invoke(tween);
            return tween;
        }, duration);
    }
    
    public Sequence Add<T, THolder>(
        THolder obj,
        ITweenFactory<T, THolder> factory, 
        T to, 
        TweenSettings settings,
        Action<Tween<T>> extraSetup = null
    ) where THolder : Object {
        return Add(obj, factory, to, settings.Duration, tween => {
            tween.Apply(settings);
            extraSetup?.Invoke(tween);
        });
    }

    public struct Item {
        public float Duration;
        [CanBeNull]
        public Action Action;
    }
}

}
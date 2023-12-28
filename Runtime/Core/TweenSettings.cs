using System;
using UnityEngine;

namespace FlowTween {

[Serializable]
public class TweenSettings {
    [Min(0)]
    [SerializeField] float _duration = 1;
    [SerializeField] EasingType _easeType;
    [SerializeField] EaseType _presetEase;
    [SerializeField] AnimationCurve _customEase;
    [SerializeField] LoopMode _loopMode;

    public float Duration {
        get => _duration;
        set => _duration = Mathf.Max(0, value);
    }

    public EaseType PresetEase {
        get => _presetEase;
        set {
            _presetEase = value;
            _easeType = EasingType.Preset;
        }
    }
    
    public AnimationCurve CustomEase {
        get => _customEase;
        set {
            _customEase = value;
            _easeType = EasingType.Custom;
        }
    }

    public void Apply(TweenBase tween) {
        tween.SetDuration(_duration).Loop(_loopMode);

        switch (_easeType) {
            case EasingType.Preset:
                tween.Ease(_presetEase);
                break;
            case EasingType.Custom:
                tween.Ease(_customEase);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public enum EasingType {
        Preset,
        Custom
    }
}

}
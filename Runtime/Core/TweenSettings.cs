using System;
using UnityEngine;

namespace FlowTween {
    
/// <summary>
/// A helper class for setting the properties that
/// change a tween's "feel", such as easing and duration.
/// </summary>
[Serializable]
public class TweenSettings {
    [Min(0)]
    [SerializeField] float _duration = 1;
    [Min(0)]
    [SerializeField] float _delay;
    [SerializeField] EasingType _easeType;
    [SerializeField] Ease _presetEase;
    [SerializeField] AnimationCurve _customEase;
    [SerializeField] LoopMode _loopMode;
    [SerializeField] int _loopCount = -1;

    /// <summary>
    /// Duration in seconds, must be greater than 0.
    /// </summary>
    public float Duration {
        get => _duration;
        set => _duration = Mathf.Max(0, value);
    }
    
    /// <summary>
    /// Delay in seconds, must be greater than 0.
    /// </summary>
    public float Delay {
        get => _delay;
        set => _delay = Mathf.Max(0, value);
    }

    /// <summary>
    /// The preset easing type to use.
    /// Setting this also sets <see cref="EaseType"/> to <see cref="EasingType.Preset"/>.
    /// </summary>  
    public Ease PresetEase {
        get => _presetEase;
        set {
            _presetEase = value;
            _easeType = EasingType.Preset;
        }
    }
    
    /// <summary>
    /// A curve to use for easing.
    /// The tween starts at <c>time=0</c> and ends at <c>time=1</c>.
    /// The curve's value is unconstrained, but it's recommended to keep it between 0 and 1.
    /// Setting this also sets <see cref="EaseType"/> to <see cref="EasingType.Custom"/>.
    /// </summary>
    public AnimationCurve CustomEase {
        get => _customEase;
        set {
            _customEase = value;
            _easeType = EasingType.Custom;
        }
    }
    
    /// <summary>
    /// The easing type to use.
    ///
    /// You usually don't need to set this manually, as setting
    /// <see cref="CustomEase"/> or <see cref="PresetEase"/> will also
    /// change this to the appropriate value.
    /// 
    /// If you do set it to <see cref="EasingType.Custom"/> make sure
    /// <see cref="CustomEase"/> is not null.
    /// </summary>
    public EasingType EaseType {
        get => _easeType;
        set => _easeType = value;
    }
    
    /// <summary>
    /// The loop mode to use.
    /// </summary>
    public LoopMode LoopMode {
        get => _loopMode;
        set => _loopMode = value;
    }
    
    public int? LoopCount {
        get => _loopCount == -1 ? null : _loopCount;
        set => _loopCount = value ?? -1;
    }

    public TweenSettings() { }

    public TweenSettings(float duration, Ease ease) {
        Duration = duration;
        PresetEase = ease;
    }

    public TweenSettings(float duration, AnimationCurve curve) {
        Duration = duration;
        CustomEase = curve;
    }
    
    /// <summary>
    /// Applies the settings to the given tween.
    /// </summary>
    public void Apply(TweenBase tween) {
        tween.SetDuration(_duration);
        
        tween.Delay = _delay;
        tween.LoopMode = _loopMode;
        tween.Loops = LoopCount;

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

    /// <summary>
    /// The type of easing to apply.
    /// Can either be <see cref="EasingType.Preset"/> or <see cref="EasingType.Custom"/>.
    /// </summary>
    public enum EasingType {
        /// <summary>
        /// Uses one of <see cref="EaseType"/>.
        /// </summary>
        Preset,
        
        /// <summary>
        /// Uses a custom <see cref="AnimationCurve"/>.
        /// </summary>
        Custom
    }
}

}
# Changelog

## [1.1.0]

### Added

- `FromToTweenerTarget` now supports any binary operation
- Ability to set another component to use as the source for `FromToTweenerTarget`
- `SetTargetId`, `SetTargetUntyped` and `SetTarget` methods to `TweenerTargetConfig`
- Sequential mode for `Tweener`
- Added ability to delay tweens
- Extracted a base class for `Sequence` and `TweenBase` called `Runnable`
- OnComplete and OnStart UnityEvents in `TweenerTargetConfig`
- Ignore Timescale option for `TweenerTargetConfig`
- Control panel to monitor tweens in the `TweenManager`

### Changed

- The property drawer for `FromToTweenerTarget` to accommodate the new features
- Names for tweener targets:
    - "SpriteRendererGradient" is now "SpriteRendererColor (Gradient)"
    - "GraphicGradient" is now "GraphicColor (Gradient)"
    - "RectTransformPosition" is now "RectTransformAnchoredPosition"
    - "RectTransformSize" is now "RectTransformSizeDelta"
    - "TransformUniformScale" is now "TransformScale (Uniform)"
- `Easing` is now `Easings`

### Fixed

- `Sequence` actually works now

### Removed

- `PlayOnDisable (all)` and `PlayOnEnable (all)` from the `Tweener` editor (will be re-added later)

## [1.0.0] - 2023-12-29

### Added

- `tween.Apply(settings)` shorthand for `TweenSettingsProperty` (previously `tween.Apply(settings.Value)`)
- XML documentation for most public members
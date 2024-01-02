# Changelog

## [Unreleased]

### Added

- `FromToTweenerTarget` now supports any binary operation
- Ability to set another component to use as the source for `FromToTweenerTarget`
- `SetTargetId`, `SetTargetUntyped` and `SetTarget` methods to `TweenerTargetConfig`

### Changed

- Reworked the property drawer for `FromToTweenerTarget` to accommodate the new features
- Tweener target "SpriteRendererGradient" is now "SpriteRendererColor (Gradient)"
- Tweener target "GraphicGradient" is now "GraphicColor (Gradient)"

## [1.0.0] - 2023-12-29

### Added

- `tween.Apply(settings)` shorthand for `TweenSettingsProperty` (previously `tween.Apply(settings.Value)`)
- XML documentation for most public members
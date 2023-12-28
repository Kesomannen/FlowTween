# FlowTween

A modern and easy-to-use tweening library for Unity 2022.3+.
You can use FlowTween fluently like any other tweening library:

```csharp
transform
    .TweenX(10)
    .SetEase(Ease.OutCubic)
    .SetDuration(1);
```

But the real magic lies in the helper classes, like `TweenSettings`,
which let's you easily customize your tweens in the inspector:

```csharp
[SerializeField] TweenSettings _settings;

void Start()
{
    transform.TweenX(10).Apply(_settings);
}
```
![bild](https://github.com/Kesomannen/FlowTween/assets/113015915/9afc332f-8fec-4290-9835-0274fdb71f99)

Not too familiar with all the ease types? You can preview them right in the editor!

<b>Note! FlowTween is not fully compatible with IMGUI. If you have an IMGUI custom editor,
or a package like NaughtyAttributes which overrides all editors, the tween preview will not show.<b>

Prefer a no-code approach? The `Tweener` component can tween tons of properties without a single line of code!

![bild](https://github.com/Kesomannen/FlowTween/assets/113015915/e342d259-763e-4f8d-8174-b653af034cd4)

## Installation

FlowTween can be installed like a normal package in the Package Manager

1. Open Package Manager
2. Press the `+` button in the top left
3. Choose "Add package from Git URL..."
4. Add `https://github.com/Kesomannen/FlowTween.git`

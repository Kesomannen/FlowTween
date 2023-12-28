# FlowTween

A modern and easy-to-use tweening library for Unity 2022.3+.
You can use FlowTween fluently like any other tweening library:

```csharp
transform
    .TweenX(10)
    .SetEase(Ease.OutCubic)
    .SetDuration(1);
```

But the real magic is in the helper classes, like `TweenSettings`,
which let's you easily customize your tweens in the inspector:

```csharp
[SerializeField] TweenSettings _settings;

void Start()
{
    transform.TweenX(10).Apply(_settings);
}
```
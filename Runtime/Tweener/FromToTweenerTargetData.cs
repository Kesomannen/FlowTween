using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlowTween.Components {

[Serializable]
public class FromToTweenerTargetValue<T> {
    public bool Relative = true;
    public T Value;
    public string OperationName;
    public Component Source;
}

[Serializable]
public abstract class FromToTweenerTargetData<T> {
    [SerializeField] FromToTweenerTargetValue<T> _start = new();
    [SerializeField] FromToTweenerTargetValue<T> _end = new();
    [SerializeField] string _sourceTypeName;

    public FromToTweenerTargetValue<T> Start {
        get => _start;
        set => _start = value;
    }
    
    public FromToTweenerTargetValue<T> End {
        get => _end;
        set => _end = value;
    }

    public FromToTweenerTargetData() { }

    public FromToTweenerTargetData(Type componentType) {
        Init(componentType);
    }
    
    public void Init(Type componentType) {
        _sourceTypeName = componentType.AssemblyQualifiedName;
        
        var operation = GetOperations().Keys.First();
        _start.OperationName = operation;
        _end.OperationName = operation;
    }
    
    public abstract IReadOnlyDictionary<string, Func<T, T, T>> GetOperations();
}

[Serializable]
public class Vector3TweenerTargetData : FromToTweenerTargetData<Vector3> {
    public static IReadOnlyDictionary<string, Func<Vector3, Vector3, Vector3>> Operations = new Dictionary<string, Func<Vector3, Vector3, Vector3>> {
        { "Add", (a, b) => a + b },
        { "Subtract", (a, b) => a - b }
    };
    
    public override IReadOnlyDictionary<string, Func<Vector3, Vector3, Vector3>> GetOperations() => Operations;
    
    public Vector3TweenerTargetData() { }
    public Vector3TweenerTargetData(Type componentType) : base(componentType) { }
    public static Vector3TweenerTargetData Create<THolder>() where THolder : Component => new(typeof(THolder));
}

[Serializable]
public class Vector2TweenerTargetData : FromToTweenerTargetData<Vector2> {
    public static IReadOnlyDictionary<string, Func<Vector2, Vector2, Vector2>> Operations = new Dictionary<string, Func<Vector2, Vector2, Vector2>> {
        { "Add", (a, b) => a + b } ,
        { "Subtract", (a, b) => a - b } ,
        { "Multiply", (a, b) => a * b } ,
        { "Divide", (a, b) => a / b }
    };
    
    public override IReadOnlyDictionary<string, Func<Vector2, Vector2, Vector2>> GetOperations() => Operations;
    
    public Vector2TweenerTargetData() { }
    public Vector2TweenerTargetData(Type componentType) : base(componentType) { }
    public static Vector2TweenerTargetData Create<THolder>() where THolder : Component => new(typeof(THolder));
}

[Serializable]
public class ColorTweenerTargetData : FromToTweenerTargetData<Color> {
    public static IReadOnlyDictionary<string, Func<Color, Color, Color>> Operations = new Dictionary<string, Func<Color, Color, Color>> {
        { "Multiply", (a, b) => a * b },
        { "Add", (a, b) => a + b },
        { "Subtract", (a, b) => a - b }
    };
    
    public override IReadOnlyDictionary<string, Func<Color, Color, Color>> GetOperations() => Operations;
    
    public ColorTweenerTargetData() { }
    public ColorTweenerTargetData(Type componentType) : base(componentType) { }
    public static ColorTweenerTargetData Create<THolder>() where THolder : Component => new(typeof(THolder));
}

[Serializable]
public class FloatTweenerTargetData : FromToTweenerTargetData<float> {
    public static IReadOnlyDictionary<string, Func<float, float, float>> Operations = new Dictionary<string, Func<float, float, float>> {
        { "Add", (a, b) => a + b },
        { "Subtract", (a, b) => a - b },
        { "Multiply", (a, b) => a * b },
        { "Divide", (a, b) => a / b }
    };
    
    public override IReadOnlyDictionary<string, Func<float, float, float>> GetOperations() => Operations;
    
    public FloatTweenerTargetData() { }
    public FloatTweenerTargetData(Type componentType) : base(componentType) { }
    public static FloatTweenerTargetData Create<THolder>() where THolder : Component => new(typeof(THolder));
}

}
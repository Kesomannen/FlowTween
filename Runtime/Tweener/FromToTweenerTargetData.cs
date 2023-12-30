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
    
    public FromToTweenerTargetValue<T> Start => _start;
    public FromToTweenerTargetValue<T> End => _end;
    
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
}

[Serializable]
public class ColorTweenerTargetData : FromToTweenerTargetData<Color> {
    public static IReadOnlyDictionary<string, Func<Color, Color, Color>> Operations = new Dictionary<string, Func<Color, Color, Color>> {
        { "Multiply", (a, b) => a * b },
        { "Add", (a, b) => a + b },
        { "Subtract", (a, b) => a - b }
    };
    
    public override IReadOnlyDictionary<string, Func<Color, Color, Color>> GetOperations() => Operations;
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
}

}
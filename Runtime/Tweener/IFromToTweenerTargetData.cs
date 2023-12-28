using System.Collections.Generic;

namespace FlowTween.Components {

public interface IFromToTweenerTargetData<T> {
    T GetStartValue(T current);
    T GetEndValue(T current);
}

public interface ICompositeTweenerTargetData<T, out TPartId> : IFromToTweenerTargetData<T> {
    IEnumerable<TPartId> GetParts();
}

}
using System.Collections.Generic;

namespace FlowTween.Components {

public interface ITweenerTargetData<T> {
    T GetStartValue(T current);
    T GetEndValue(T current);
}

public interface ICompositeTweenerTargetData<T, out TPartId> : ITweenerTargetData<T> {
    IEnumerable<TPartId> GetParts();
}

}
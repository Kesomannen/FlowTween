using System.Collections.Generic;

namespace FlowTween.Components {

/// <summary>
/// Data type for <see cref="FromToTweenerTarget{T,THolder,TData}"/>.
/// </summary>
/// <typeparam name="T">The tween value type.</typeparam>
/// <seealso cref="FromToTweenerTarget{T,THolder,TData}"/>
public interface IFromToTweenerTargetData<T> {
    T GetStartValue(T current);
    T GetEndValue(T current);
}

}
using FlowTween.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FlowTween.Editor {

[CustomPropertyDrawer(typeof(FromToTweenerTargetData<>), true)]
internal class FromToTweenerTargetDataDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        return Resources.Load<VisualTreeAsset>("FlowTween/FromToTweenerTargetData").CloneTree();
    }
}

}
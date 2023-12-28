using FlowTween.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FlowTween.Editor {

[CustomPropertyDrawer(typeof(GradientData))]
public class GradientDataDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        return new PropertyField(property.FindPropertyRelative("Gradient"));
    }
}

}
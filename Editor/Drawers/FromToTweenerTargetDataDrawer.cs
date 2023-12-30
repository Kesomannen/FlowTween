using System;
using System.Collections.Generic;
using System.Linq;
using FlowTween.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FlowTween.Editor {

[CustomPropertyDrawer(typeof(FromToTweenerTargetData<>), true)]
internal class FromToTweenerTargetDataDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        var template = Resources.Load<VisualTreeAsset>("FlowTween/FromToTweenerTargetData");
        var root = template.CloneTree();
        
        var propertyObject = property.GetValue<object>();
        var getOperationsMethod = propertyObject.GetType().GetMethod("GetOperations");
        var operations = getOperationsMethod!.Invoke(propertyObject, null);
        var keysProperty = operations.GetType().GetProperty("Keys");
        var operationNames = ((IEnumerable<string>)keysProperty!.GetValue(operations))
            .Select(key => key.ToDisplayString()).ToList();
        
        var sourceTypeName = property.Find("_sourceTypeName");
        
        SetupValue("start");
        SetupValue("end");
        
        return root;

        void SetupValue(string path) {
            var valueElement = root.Q(path);

            var operationDropdown = valueElement.Q<DropdownField>("operation-dropdown");
            operationDropdown.choices = operationNames;
            
            var sourceField = valueElement.Q<ObjectField>("source-field");
            sourceField.objectType = Type.GetType(sourceTypeName.stringValue);

            var foldout = valueElement.Q<Foldout>();
            
            var relativeToggle = valueElement.Q<Toggle>("relative-toggle");
            relativeToggle.RegisterValueChangedCallback(evt => RelativeChanged(evt.newValue));
            RelativeChanged(relativeToggle.value);
            
            void RelativeChanged(bool value) {
                foldout.SetVisible(value);
            }
        }
    }
}

}
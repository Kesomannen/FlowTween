using FlowTween.Templates;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FlowTween.Editor {

public abstract class TemplatePropertyDrawer<T, TAsset, TProperty> : PropertyDrawer
    where T : class
    where TAsset : TemplateAsset<T> 
    where TProperty : TemplateProperty<T>
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        var root = Resources.Load<VisualTreeAsset>("FlowTween/TemplateProperty").CloneTree();

        var templateValueProperty = property.FindPropertyRelative("_templateValue");
        var inlineProperty = property.FindPropertyRelative("_inlineValue");
        var useTemplateProperty = property.FindPropertyRelative("_useTemplate");

        var foldout = root.Q<Foldout>("property-foldout");
        foldout.text = property.displayName;
        foldout.value = property.isExpanded;
        foldout.RegisterValueChangedCallback(evt => property.isExpanded = evt.newValue);
        
        var saveButton = root.Q<Button>("save-button");
        var valueField = root.Q<PropertyField>("value-field");
        var templateField = root.Q<ObjectField>("template-field");
        templateField.objectType = typeof(TAsset);

        if (UpdateUseTemplate()) BindToTemplate();
        else BindToInline();

        UpdateUseTemplate();
        templateField.RegisterValueChangedCallback(_ => UpdateUseTemplate());

        saveButton.clicked += () => {
            var path = EditorUtility.SaveFilePanelInProject("Save Template", "New Template", "asset", "");
            if (string.IsNullOrEmpty(path)) return;
            
            var templateAsset = ScriptableObject.CreateInstance<TAsset>();
            templateAsset.Template = property.GetValue<TProperty>().Value;
            AssetDatabase.CreateAsset(templateAsset, path);
            
            templateValueProperty.objectReferenceValue = templateAsset;
            property.serializedObject.ApplyModifiedProperties();
        };

        return root;

        bool UpdateUseTemplate() {
            var template = templateValueProperty.objectReferenceValue as TAsset;
            var useTemplate = useTemplateProperty.boolValue;
            
            if (useTemplateProperty.boolValue) {
                if (template == null) {
                    useTemplate = false;
                    BindToInline(); 
                }
            } else {
                if (template != null) {
                    useTemplate = true;
                    BindToTemplate();
                }
            }

            if (useTemplate != useTemplateProperty.boolValue) {
                useTemplateProperty.boolValue = useTemplate;
                property.serializedObject.ApplyModifiedProperties();
            }
            
            saveButton.SetEnabled(!useTemplate);
            return useTemplate;
        }
        
        void BindToTemplate() {
            var obj = new SerializedObject(templateValueProperty.objectReferenceValue);
            valueField.BindProperty(obj.FindProperty("_template"));
        }
        
        void BindToInline() {
            valueField.BindProperty(inlineProperty);
        }
    } 
}

}
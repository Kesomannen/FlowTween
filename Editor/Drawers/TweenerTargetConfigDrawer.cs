using System.Collections.Generic;
using System.Linq;
using FlowTween.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FlowTween.Editor {

[CustomPropertyDrawer(typeof(TweenerTargetConfig))]
internal class TweenerTargetConfigDrawer : PropertyDrawer {
    static Dictionary<string, string> _typeMap;
    
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        var config = property.GetValue<TweenerTargetConfig>();
        
        var template = Resources.Load<VisualTreeAsset>("FlowTween/TweenerTargetConfig");
        var root = template.CloneTree();

        var targetIdProperty = property.FindPropertyRelative("_targetId");
        var dataProperty = property.FindPropertyRelative("_data");
        
        var typeDropdown = root.Q<DropdownField>("type-dropdown");
        var dataField = root.Q<PropertyField>("data-field");
        var resetOnDisable = root.Q<Toggle>("reset-on-disable");
        var playOnDisable = root.Q<Toggle>("play-on-disable");
        
        _typeMap ??= TweenerTargets.Targets
            .ToDictionary(kvp => {
                var component = kvp.Value.ComponentType.Name.ToDisplayString();
                var type = kvp.Key.ToDisplayString().Replace(component, "").Trim();
                return $"{component}/{type}";
            }, kvp => kvp.Key);
        
        typeDropdown.choices = _typeMap.Keys.ToList();
        var targetId = config.TargetId;
        typeDropdown.value = _typeMap.First(kvp => kvp.Value == targetId).Key;

        typeDropdown.RegisterValueChangedCallback(evt => {
            targetIdProperty.stringValue = _typeMap[evt.newValue];
            property.serializedObject.ApplyModifiedProperties();
            RebindData();
        });

        resetOnDisable.SetEnabled(playOnDisable.value);
        playOnDisable.RegisterValueChangedCallback(evt => {
            resetOnDisable.SetEnabled(evt.newValue);
        });

        var previewButton = root.Q<Button>("preview-button");
        var previewLabel = previewButton.Q<Label>("Label");
        var previewIcon = previewButton.Q<MaterialIcon>();
        
        var obj = property.serializedObject.targetObject;
        if (obj is not GameObject gameObject) {
            if (obj is Component component) {
                gameObject = component.gameObject;
            } else {
                previewButton.SetEnabled(false);
                return root;
            }
        } 

        var preview = new TweenPreview(root);
        preview.OnTweenStoppedOrRestarted += _ => {
            config.ApplySnapshot(gameObject);
        };

        preview.OnStoppedPlaying += () => {
            previewLabel.text = "Preview";
            previewIcon.IconName = "play_arrow";
        };
        
        preview.OnStartedPlaying += () => {
            previewLabel.text = "Stop";
            previewIcon.IconName = "stop";
        };
        
        previewButton.clicked += () => {
            if (preview.IsPlaying) {
                preview.StopAll();
            } else {
                var tween = config.GetTween(gameObject);
                if (tween == null) return;
                
                config.TakeSnapshot(gameObject);
                preview.Play(tween);
            }
        };
        
        RebindData();
        return root;

        void RebindData() {
            property.serializedObject.Update();
            dataField.Unbind();
            dataField.BindProperty(dataProperty);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorUtil.LineHeight * 3;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var rect = position;
        rect.height = EditorUtil.LineHeight;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.LabelField(rect, label);
        EditorGUI.EndDisabledGroup();
        
        rect.y += EditorUtil.LineHeight;
        rect.height *= 2;
        EditorUtil.DrawIMGUIHelpMessage(rect);
    }
}

}
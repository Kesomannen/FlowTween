﻿using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FlowTween.Editor {

[CustomPropertyDrawer(typeof(TweenSettings))]
internal class TweenSettingsDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!property.isExpanded) {
            return EditorGUIUtility.singleLineHeight * 3;
        }
        
        return EditorUtil.LineHeight * 7;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
        
        if (property.isExpanded) {
            EditorGUI.indentLevel++;
            
            var duration = property.FindPropertyRelative("_duration");
            var easeType = property.FindPropertyRelative("_easeType");
            var presetEase = property.FindPropertyRelative("_presetEase");
            var customEase = property.FindPropertyRelative("_customEase");
        
            rect.y += EditorUtil.LineHeight;
            EditorGUI.PropertyField(rect, duration);
            
            rect.y += EditorUtil.LineHeight;
            EditorGUI.PropertyField(rect, easeType);
            

            var usePreset = easeType.enumValueIndex == (int)TweenSettings.EasingType.Preset;
            rect.y += EditorUtil.LineHeight;
            EditorGUI.PropertyField(rect, usePreset ? presetEase : customEase);
            
            EditorGUI.indentLevel--;
        }
        
        rect.y += EditorUtil.LineHeight;
        rect.height *= 2;
        EditorUtil.DrawIMGUIHelpMessage(rect, MessageType.Warning);
        EditorGUI.EndProperty();
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        var template = Resources.Load<VisualTreeAsset>("FlowTween/TweenSettings");
        var root = template.CloneTree();
        
        root.Q<Foldout>().text = property.displayName;
        
        var presetEase = root.Q("preset-ease");
        var presetEaseType = presetEase.Q<EnumField>("preset-ease-type");
        var presetEaseDirection = presetEase.Q<EnumField>("preset-ease-direction");
        
        UpdatePresetEase(property.FindPropertyRelative("_presetEase.Type").enumValueIndex);
        presetEaseType.RegisterValueChangedCallback(evt => UpdatePresetEase((int)(object)evt.newValue));
        
        var loopMode = root.Q<EnumField>("loop-mode");
        var loopCount = root.Q<IntegerField>("loop-count");
        var loopInfiniteButton = root.Q<Button>("loop-infinite-button");
        
        var loopCountProperty = property.FindPropertyRelative("_loopCount");
        var loopModeProperty = property.FindPropertyRelative("_loopMode");
        
        loopMode.RegisterValueChangedCallback(_ => UpdateLoopMode());
        loopInfiniteButton.clicked += () => {
            loopCountProperty.intValue = loopCountProperty.intValue == -1 ? 2 : -1;
            property.serializedObject.ApplyModifiedProperties();
            UpdateLoopMode();
        };
        UpdateLoopMode();
        
        var customEase = root.Q<CurveField>("custom-ease");
        var progressBar = root.Q<ProgressBar>("preview");
        
        var duration = root.Q<FloatField>("duration");
        duration.RegisterValueChangedCallback(evt => {
            if (evt.newValue < 0) {
                duration.value = 0;
            }
        });

        var preview = new TweenPreview(root);
        var previewButton = root.Q<Button>("preview-button");
        var previewIcon = previewButton.Q<MaterialIcon>();
        
        var playOnEdit = false;
        
        previewButton.clicked += () => {
            if (preview.IsPlaying) {
                preview.StopAll();
                playOnEdit = false;
            } else {
                StartPreview();
                playOnEdit = true;
            }
        };
        
        preview.OnStoppedPlaying += () => {
            previewIcon.IconName = "play_arrow";
        };
        
        preview.OnStartedPlaying += () => {
            previewIcon.IconName = "stop";
        };

        var easeTypeField = root.Q<EnumField>("ease-type");
        easeTypeField.RegisterValueChangedCallback(evt => {
            EaseTypeChanged(evt.newValue);
        });
        
        EaseTypeChanged(easeTypeField.value);

        root.TrackPropertyValue(property, _ => {
            if (playOnEdit) {
                StartPreview();
            }
        });
        return root;

        void EaseTypeChanged(object value) {
            var usePreset = (int)value == (int)TweenSettings.EasingType.Preset;
            presetEase.SetVisible(usePreset);
            customEase.SetVisible(!usePreset);
        }
        
        void StartPreview() {
            preview.Play(property.serializedObject.targetObject.TweenValue(0, 1)
                .OnUpdate(value => progressBar.value = value)
                .Apply(property.GetValue<TweenSettings>())
            );
        }

        void UpdateLoopMode() {
            var infinite = loopCountProperty.intValue == -1;
            if (infinite) {
                loopInfiniteButton.AddToClassList("icon-button__selected");
            }else {
                loopInfiniteButton.RemoveFromClassList("icon-button__selected");
            }
            
            var noneIsSelected = (LoopMode)loopModeProperty.enumValueIndex == LoopMode.None;
            loopCount.SetEnabled(!noneIsSelected && !infinite); 
            loopInfiniteButton.SetEnabled(!noneIsSelected);
        }

        void UpdatePresetEase(int index) {
            presetEaseDirection.SetEnabled((EaseType)index != EaseType.Linear);
        }
    }
}

}
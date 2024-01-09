using System;
using System.Collections.Generic;
using System.Linq;
using FlowTween.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FlowTween.Editor {

[CustomEditor(typeof(Tweener))]
public class TweenerEditor : UnityEditor.Editor {
    public override VisualElement CreateInspectorGUI() {
        var tweener = (Tweener)target;
        
        var template = Resources.Load<VisualTreeAsset>("FlowTween/Tweener");
        var root = template.CloneTree();
        
        var previewSelectedButton = root.Q<Button>("preview-selected-button");
        var previewAllButton = root.Q<Button>("preview-all-button");
        var stopPreviewButton = root.Q<Button>("stop-preview-button");
        
        var addButton = root.Q<Button>("add-button");
        var noTweensLabel = root.Q("no-tweens-label");
        var tweenList = root.Q("tweens");
        
        var playOnEnableAll = root.Q<Toggle>("play-on-enable-all");
        SetupAllToggle(playOnEnableAll, "_playOnEnable");
        
        var playOnDisableAll = root.Q<Toggle>("play-on-disable-all");
        SetupAllToggle(playOnDisableAll, "_playOnDisable");
        
        var selectedIndices = new HashSet<int>();
        var tweenMap = new Dictionary<int, VisualElement>();
        
        RebuildList();
        
        addButton.clicked += () => {
            var tween = tweener.AddTween(out _);
            tween.PlayOnEnable = playOnEnableAll.value;
            
            EditorUtility.SetDirty(tweener);
            serializedObject.Update();
            RebuildList();
        };

        var preview = new TweenPreview(root);
        IReadOnlyList<TweenerTargetConfig> previewingTweens = null;
        
        preview.OnStartedPlaying += () => {
            SetPreviewButtonState(false);
        };
        
        preview.OnStoppedPlaying += () => {
            SetPreviewButtonState(true);
            
            if (previewingTweens == null) return;
            foreach (var tween in previewingTweens) {
                tween.ApplySnapshot(tweener.gameObject);
            }
        };
        
        previewAllButton.clicked += () => {
            PreviewTweens(tweener.Tweens);
        };
        
        previewSelectedButton.clicked += () => {
            var selectedTweens = selectedIndices
                .Select(index => tweener.Tweens[index])
                .ToArray();

            if (selectedTweens.Length > 0) {
                PreviewTweens(selectedTweens);
            }
        };
        
        stopPreviewButton.clicked += () => {
            preview.StopAll();
        };
        
        return root;
        
        SerializedProperty GetTweenProperty(int index) {
            return serializedObject.FindProperty($"_tweens.Array.data[{index}]");
        }
        
        void SetPreviewButtonState(bool enabled) {
            previewAllButton.SetVisible(enabled);
            previewSelectedButton.SetVisible(enabled);
            previewSelectedButton.SetEnabled(selectedIndices.Any());
            
            stopPreviewButton.SetVisible(!enabled);
            root.Query("preview-button").ForEach(element => element.SetEnabled(enabled));
        }
        
        void PreviewTweens(IReadOnlyList<TweenerTargetConfig> tweens) {
            foreach (var tween in tweens) {
                tween.TakeSnapshot(tweener.gameObject);
            }

            previewingTweens = tweens;
            preview.Play(tweener.PlayTweensAndReturn(tweens));
        }
        
        void CreateTweenListItem(int index) {
            var property = GetTweenProperty(index);
            var targetIdProperty = property.FindPropertyRelative("_targetId");
            var selectedProperty = property.FindPropertyRelative("_selected");
            
            var item = new VisualElement();
            item.AddToClassList("unity-list-view__item");

            var foldout = new Foldout {
                name = "item-foldout",
                value = property.FindPropertyRelative("_selected").boolValue
            };
            foldout.RegisterCallback<ChangeEvent<bool>>(evt => {
                if (evt.propagationPhase != PropagationPhase.AtTarget) return;
                if (evt.newValue) {
                    if (!selectedIndices.Add(index)) return;
                    
                    selectedProperty.boolValue = true;
                    OnSelectionChanged();
                } else {
                    if (!selectedIndices.Remove(index)) return;
                    
                    selectedProperty.boolValue = false;
                    OnSelectionChanged();
                }
            });
            foldout.TrackPropertyValue(targetIdProperty, _ => UpdateFoldoutText());
            UpdateFoldoutText();

            var removeButton = new Button {
                name = "remove-button",
                tooltip = "Remove Tween",
            };
            removeButton.clicked += () => {
                tweener.RemoveTween(index);
                EditorUtility.SetDirty(tweener);
                serializedObject.Update();
                RebuildList();
            };
            
            removeButton.AddToClassList("icon-button");
            removeButton.AddToClassList("remove-button");
            removeButton.Add(new MaterialIcon { IconName = "remove" });
            
            var field = new PropertyField { name = "item-field" };
            field.BindProperty(property);
            
            foldout.Add(field);
            
            item.Add(foldout);
            item.Add(removeButton);

            tweenList.Add(item);
            tweenMap[index] = item;
            return;

            void UpdateFoldoutText() {
                var targetId = ObjectNames.NicifyVariableName(targetIdProperty.stringValue);
                foldout.text = $"Tween {index} ({targetId})";
            }
        }
        
        void RebuildList() {
            tweenList.Clear();
            tweenMap.Clear();
            selectedIndices.Clear();
            for (var i = 0; i < tweener.Tweens.Count; i++) {
                CreateTweenListItem(i);
            }
            
            noTweensLabel.SetVisible(tweener.Tweens.Count == 0);

            var newSelection = tweener.Tweens
                .Select((tween, i) => (tween, i))
                .Where(tuple => tuple.tween.IsSelected)
                .Select(tuple => tuple.i);
            
            foreach (var index in newSelection) {
                selectedIndices.Add(index);
            }
            
            foreach (var (i, element) in tweenMap) {
                element.Q<Foldout>("item-foldout").value = selectedIndices.Contains(i);
            }
            
            OnSelectionChanged();
        }

        void SetupAllToggle(Toggle toggle, string propertyPath) {
            UpdateValue();
            
            foreach (var property in EnumerateProperties()) {
                toggle.TrackPropertyValue(property, _ => UpdateValue());
            }
            
            /*
            toggle.RegisterCallback<ClickEvent>(_ => {
                foreach (var serializedProperty in EnumerateProperties()) {
                    serializedProperty.boolValue = toggle.value;
                }
                UpdateValue();
            });
            */
            
            IEnumerable<SerializedProperty> EnumerateProperties() {
                return EnumerateConfigs().Select(property => property.FindPropertyRelative(propertyPath));
            }
            
            void UpdateValue() {
                var enabledCount = EnumerateProperties().Count(property => property.boolValue);

                if (enabledCount == 0) {
                    toggle.value = false;
                    toggle.RemoveFromClassList("mixed-toggle");
                } else if (enabledCount == tweener.Tweens.Count) {
                    toggle.value = true;
                    toggle.RemoveFromClassList("mixed-toggle");
                } else {
                    toggle.AddToClassList("mixed-toggle");
                }
            }
        }

        void OnSelectionChanged() {
            previewSelectedButton.SetEnabled(selectedIndices.Count > 0);
            previewAllButton.SetEnabled(tweener.Tweens.Count > 0);
            serializedObject.ApplyModifiedProperties();
        }
        
        IEnumerable<SerializedProperty> EnumerateConfigs() {
            for (var i = 0; i < tweener.Tweens.Count; i++) {
                yield return GetTweenProperty(i);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        EditorUtil.DrawIMGUIHelpMessageLayout();
    }
}

}
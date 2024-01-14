using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FlowTween.Editor {

[CustomEditor(typeof(TweenManager))]
public class TweenManagerEditor : UnityEditor.Editor {
    public override VisualElement CreateInspectorGUI() {
        var root = Resources.Load<VisualTreeAsset>("FlowTween/TweenManager").CloneTree();
        
        var options = root.Q("options");
        var enabledToggle = root.Q<Toggle>("enabled-toggle");
        enabledToggle.RegisterValueChangedCallback(evt => options.SetEnabled(evt.newValue));
        options.SetEnabled(serializedObject.FindProperty("_enabled").boolValue);
        
        var maxTweensField = root.Q<IntegerField>("max-tweens-field");
        var hasMaxTweensToggle = root.Q<Toggle>("has-max-tweens-toggle");
        hasMaxTweensToggle.RegisterValueChangedCallback(evt => maxTweensField.SetEnabled(evt.newValue));
        maxTweensField.SetEnabled(serializedObject.FindProperty("_hasMaxTweens").boolValue);

        root.Q<Button>("control-panel-button").clicked += TweenManagerControlPanel.ShowPanel;
        
        return root;
    }
}

}
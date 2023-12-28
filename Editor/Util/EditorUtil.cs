using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FlowTween.Editor {

internal static class EditorUtil {
    public static readonly float LineHeight = EditorGUIUtility.singleLineHeight + 2;
    
    public static void SetVisible(this VisualElement element, bool visible) {
        element.style.display = visible? DisplayStyle.Flex : DisplayStyle.None;
    }

    public static string ToDisplayString(this object value) {
        return ObjectNames.NicifyVariableName(value.ToString());
    }
    
    const string IMGUIHelpMessage = "This inspector is using IMGUI, which is not fully supported by FlowTween." +
                                    "This is often caused by an editor extension that overrides the default inspector " +
                                    "with an IMGUI one, such as the NaughtyAttributes package.";

    public static void DrawIMGUIHelpMessage(Rect position, MessageType messageType = MessageType.Error) {
        EditorGUI.HelpBox(position, IMGUIHelpMessage, messageType);
    }
    
    public static void DrawIMGUIHelpMessageLayout(MessageType messageType = MessageType.Error) {
        EditorGUILayout.HelpBox(IMGUIHelpMessage, messageType);
    }
}

}
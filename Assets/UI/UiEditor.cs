using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UiEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/UiEditor")]
    public static void ShowExample()
    {
        UiEditor wnd = GetWindow<UiEditor>();
        wnd.titleContent = new GUIContent("UiEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

    }
}

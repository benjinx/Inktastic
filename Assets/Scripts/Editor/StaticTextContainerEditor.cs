using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(StaticTextContainer))]
public class StaticTextContainerEditor : Editor
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private VisualElement root;
    private SerializedProperty keysProp;
    private SerializedProperty dataProp;
    private SerializedProperty internalKeysProp;
    private SerializedProperty entriesProp;

    private void OnEnable()
    {
        root = m_VisualTreeAsset.CloneTree();
        entriesProp = serializedObject.FindProperty("entries");
        keysProp = serializedObject.FindProperty("keys");
        dataProp = serializedObject.FindProperty("data");
        internalKeysProp = serializedObject.FindProperty("internalKeys");
    }

    public override VisualElement CreateInspectorGUI()
    {
        var view = root.Q<MultiColumnListView>("view");
        var keyColumn = view.columns["key"];
        view.BindProperty(entriesProp);
        // keyColumn.makeCell = () => new TextField();
        // keyColumn.bindCell = (e, i) =>
        // {
        //     if (e is TextField textField)
        //     {
        //         textField.BindProperty(keysProp.GetArrayElementAtIndex(i));
        //     }
        // };

        var internalKeyColumn = view.columns["hash"];
        internalKeyColumn.makeCell = () => new Label();
        internalKeyColumn.bindCell = (e, i) =>
        {
            if (e is Label label)
            {
                var element = entriesProp.GetArrayElementAtIndex(i);
                var hash = element.FindPropertyRelative("Hash").uintValue;
                label.text = hash.ToString();
            }
        };

        return root;
    }
}

using RTPuzzle;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(InteractiveObjectTypeDefinitionInherentData), false)]
public class InteractiveObjectDefinitionCustomEditor : ObjectDefinitionCustomEditor, IObjectDefinitionCustomEditorEventListener
{
    private FoldableArea sharedDataFoldableArea;
    private Editor SharedDataEditor;

    public void BeforeOnInspectorGUI()
    {
        var InteractiveObjectTypeDefinitionInherentData = (InteractiveObjectTypeDefinitionInherentData)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObjectTypeDefinitionInherentData.InteractiveObjectID)));
        if (sharedDataFoldableArea == null)
        {
            if (InteractiveObjectTypeDefinitionInherentData.InteractiveObjectSharedDataTypeInherentData != null)
            {
                this.sharedDataFoldableArea = new FoldableArea(false, "Shared Data", true);
            }
        }

        if (InteractiveObjectTypeDefinitionInherentData.InteractiveObjectSharedDataTypeInherentData == null)
        {
            this.sharedDataFoldableArea = null;
            this.SharedDataEditor = null;
        }

        if (this.sharedDataFoldableArea != null)
        {
            this.sharedDataFoldableArea.OnGUI(() =>
            {
                if (SharedDataEditor == null) { SharedDataEditor = Editor.CreateEditor(InteractiveObjectTypeDefinitionInherentData.InteractiveObjectSharedDataTypeInherentData); }
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(InteractiveObjectTypeDefinitionInherentData.InteractiveObjectSharedDataTypeInherentData, InteractiveObjectTypeDefinitionInherentData.InteractiveObjectSharedDataTypeInherentData.GetType(), false);
                EditorGUI.EndDisabledGroup();
                this.SharedDataEditor.OnInspectorGUI();
            });
        }
        else
        {
            if (GUILayout.Button("CREATE SHARED DATA"))
            {
                InteractiveObjectTypeDefinitionInherentData.InteractiveObjectSharedDataTypeInherentData =
                    (InteractiveObjectSharedDataTypeInherentData)AssetHelper.CreateAssetAtSameDirectoryLevel(InteractiveObjectTypeDefinitionInherentData, typeof(InteractiveObjectSharedDataTypeInherentData), "SharedData");
            }
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        this.RegisterListener(this);
    }
}
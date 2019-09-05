using AdventureGame;
using UnityEditor;

[System.Serializable]
[CustomEditor(typeof(PointOfInterestDefinitionInherentData), false)]
public class PointOfInterestDefinitionCustomEditor : ObjectDefinitionCustomEditor, IObjectDefinitionCustomEditorEventListener
{
    public void BeforeOnInspectorGUI()
    {
        var PointOfInterestDefinitionInherentData = (PointOfInterestDefinitionInherentData)target;
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(PointOfInterestDefinitionInherentData.PointOfInterestId)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(PointOfInterestDefinitionInherentData.PointOfInterestSharedDataTypeInherentData)));
    }

    private void OnEnable()
    {
        this.RegisterListener(this);
    }
}

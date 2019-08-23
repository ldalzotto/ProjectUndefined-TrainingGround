using RTPuzzle;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Editor_GameCustomEditors
{
    [CustomEditor(typeof(AbstractInteractiveObjectDefinition), editorForChildClasses: true)]
    public class InteractiveObjectDefinitionCustomEditor : Editor
    {
        private List<ScriptableObjectSubstitutionFieldTracker> substitutionFieldsTrackers;

        private void OnEnable()
        {
            if (this.substitutionFieldsTrackers == null) { this.substitutionFieldsTrackers = new List<ScriptableObjectSubstitutionFieldTracker>(); }

            var fields = this.target.GetType().GetFields();
            foreach (var field in fields)
            {
                var fieldAttributes = field.GetCustomAttributes();
                foreach (var fieldAttribute in fieldAttributes)
                {
                    if (fieldAttribute.GetType() == typeof(ScriptableObjectSubstitution))
                    {
                        var ScriptableObjectSubstitution = (ScriptableObjectSubstitution)fieldAttribute;
                        this.substitutionFieldsTrackers.Add(new ScriptableObjectSubstitutionFieldTracker(field, this.target.GetType().GetField(ScriptableObjectSubstitution.SubstitutionName), ScriptableObjectSubstitution, this.serializedObject));
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (this.substitutionFieldsTrackers.Count == 0)
            {
                base.OnInspectorGUI();
            }
            else
            {
                foreach (var field in this.target.GetType().GetFields())
                {
                    bool fieldAlreadyDisaplyed = false;
                    foreach (var substitutionFieldTracker in this.substitutionFieldsTrackers)
                    {
                        fieldAlreadyDisaplyed = substitutionFieldTracker.OnGUI(this.serializedObject, field);
                        if (fieldAlreadyDisaplyed) { break; }
                    }
                    if (!fieldAlreadyDisaplyed)
                    {
                        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(field.Name), true);
                    }
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    class ScriptableObjectSubstitutionFieldTracker
    {
        private FieldInfo SourceField;
        private FieldInfo SubstitutionField;
        private ScriptableObjectSubstitution ScriptableObjectSubstitutionProperty;

        private bool sourceButtonPressed;
        private bool substitutionButtonPressed;

        public ScriptableObjectSubstitutionFieldTracker(FieldInfo sourceField, FieldInfo substitutionField, ScriptableObjectSubstitution ScriptableObjectSubstitutionProperty, SerializedObject serializedObject)
        {
            this.sourceButtonPressed = serializedObject.FindProperty(ScriptableObjectSubstitutionProperty.SourcePickerName).boolValue;
            if (!sourceButtonPressed && serializedObject.FindProperty(ScriptableObjectSubstitutionProperty.SubstitutionName).objectReferenceValue == null)
            {
                this.sourceButtonPressed = true;
            }
            this.substitutionButtonPressed = !this.sourceButtonPressed;
            SourceField = sourceField;
            SubstitutionField = substitutionField;
            this.ScriptableObjectSubstitutionProperty = ScriptableObjectSubstitutionProperty;
        }

        public bool OnGUI(SerializedObject serializedObject, FieldInfo field)
        {
            if (field.Name == this.SourceField.Name)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                this.sourceButtonPressed = EditorGUILayout.Toggle(this.ScriptableObjectSubstitutionProperty.SourceChoiceLabel, this.sourceButtonPressed, EditorStyles.miniButtonLeft);
                if (EditorGUI.EndChangeCheck())
                {
                    if (this.sourceButtonPressed) { this.substitutionButtonPressed = false; }
                }
                EditorGUI.BeginChangeCheck();
                this.substitutionButtonPressed = EditorGUILayout.Toggle(this.ScriptableObjectSubstitutionProperty.SubstitutionChoiceLabel, this.substitutionButtonPressed, EditorStyles.miniButtonRight);
                if (EditorGUI.EndChangeCheck())
                {
                    if (this.substitutionButtonPressed) { this.sourceButtonPressed = false; }
                }
                EditorGUILayout.EndHorizontal();

                if (this.sourceButtonPressed)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(SourceField.Name), true);
                    serializedObject.FindProperty(this.ScriptableObjectSubstitutionProperty.SourcePickerName).boolValue = true;
                }
                else if (this.substitutionButtonPressed)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(SubstitutionField.Name), true);
                    serializedObject.FindProperty(this.ScriptableObjectSubstitutionProperty.SourcePickerName).boolValue = false;
                }
                return true;
            }
            else if (field.Name == this.SubstitutionField.Name || field.Name == this.ScriptableObjectSubstitutionProperty.SourcePickerName)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
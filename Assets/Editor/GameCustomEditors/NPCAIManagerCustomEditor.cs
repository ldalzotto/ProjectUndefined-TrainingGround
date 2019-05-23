using UnityEngine;
using System.Collections;
using UnityEditor;
using RTPuzzle;
using System.Collections.Generic;
using System.Linq;


[CustomEditor(typeof(NPCAIManager))]
public class NPCAIManagerCustomEditor : Editor
{

    public static AIComponentsConfiguration AIComponentsConfiguration;
    public static AbstractAIComponents AIComponents;
    public static Dictionary<string, bool> ComponentGizmosEnabled = new Dictionary<string, bool>();

    private void OnEnable()
    {
        AIComponentsConfiguration = AssetFinder.SafeSingleAssetFind<AIComponentsConfiguration>("t:" + typeof(AIComponentsConfiguration).Name);
        if (AIComponentsConfiguration != null)
        {
            AIComponents = AIComponentsConfiguration.ConfigurationInherentData[(target as NPCAIManager).AiID].AIComponents;
            if (AIComponents != null)
            {
                foreach (var aiComponentField in AIComponents.GetType().GetFields())
                {
                    if (aiComponentField.FieldType.IsSubclassOf(typeof(AbstractAIComponent)))
                    {
                        ComponentGizmosEnabled[aiComponentField.Name] = true;
                    }
                }
            }

        }
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("OPEN CONFIGURATION"))
        {
            PuzzleGameConfigurationEditorV2.OpenToDesiredConfiguration(typeof(AIComponentsConfiguration));
        }
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        if (ComponentGizmosEnabled != null)
        {
            var oldGUiBackground = GUI.backgroundColor;
            Handles.BeginGUI();
            GUI.backgroundColor = new Color(oldGUiBackground.r, oldGUiBackground.g, oldGUiBackground.b, 0.5f);

            GUILayout.BeginArea(new Rect(10, 10, 200, 600));
            foreach (var key in ComponentGizmosEnabled.Keys.ToList())
            {
                ComponentGizmosEnabled[key] = GUILayout.Toggle(ComponentGizmosEnabled[key], key, EditorStyles.miniButton);
            }
            GUILayout.EndArea();
            Handles.EndGUI();

            var npcAIManager = (NPCAIManager)target;
            base.OnInspectorGUI();
            if (npcAIManager != null && AIComponentsConfiguration != null && AIComponents != null)
            {
                var oldGizmoColor = Gizmos.color;
                var oldhandlesColor = Handles.color;

                var aiComponentsType = AIComponents.GetType();

                foreach (var aiComponentField in aiComponentsType.GetFields())
                {
                    if (aiComponentField.FieldType.IsSubclassOf(typeof(AbstractAIComponent)))
                    {
                        if (ComponentGizmosEnabled.ContainsKey(aiComponentField.Name))
                        {
                            if (ComponentGizmosEnabled[aiComponentField.Name])
                            {
                                ((AbstractAIComponent)aiComponentField.GetValue(AIComponents)).EditorGUI(npcAIManager.transform);
                            }
                        }
                    }
                }
                Gizmos.color = oldGizmoColor;
                Handles.color = oldhandlesColor;
            }


            // GUI.backgroundColor = oldGUiBackground;
        }

    }

}

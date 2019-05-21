using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Editor_GameDesigner
{
    public class GameDesignerEditor : EditorWindow
    {
        [MenuItem("GameDesigner/GameDesignerEditor")]
        static void Init()
        {
            GameDesignerEditor window = (GameDesignerEditor)EditorWindow.GetWindow(typeof(GameDesignerEditor));
            window.Show();
        }

        private GameDesignerEditorProfile GameDesignerEditorProfile;

        private ChoiceTree ChoiceTree;
        
        private void OnGUI()
        {
            if (this.GameDesignerEditorProfile == null)
            {
                this.GameDesignerEditorProfile = AssetFinder.SafeSingleAssetFind<GameDesignerEditorProfile>("t:" + typeof(GameDesignerEditorProfile).Name);
            }

            if (this.GameDesignerEditorProfile != null)
            {

                if (this.ChoiceTree == null)
                {
                    this.ChoiceTree = new ChoiceTree(ref this.GameDesignerEditorProfile);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(GUILayout.Width(200f));
                if (this.ChoiceTree != null)
                {
                    this.ChoiceTree.GUITick();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                if (this.GameDesignerEditorProfile.CurrentGameDesignerModule != null)
                {
                    this.GameDesignerEditorProfile.CurrentGameDesignerModule.GUITick();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.ObjectField(this.GameDesignerEditorProfile, typeof(Object), false);

            if (GUI.changed) { this.Repaint(); }
        }
    }

}

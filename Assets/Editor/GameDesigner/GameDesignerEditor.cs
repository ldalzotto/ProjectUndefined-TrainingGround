using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

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

        public static void InitWithSelectedKey(Type designerModuleType)
        {
            GameDesignerEditor window = (GameDesignerEditor)EditorWindow.GetWindow(typeof(GameDesignerEditor));
            window.InitEditorData();
            window.ChoiceTree.Init(() => { window.Repaint(); });
            window.ChoiceTree.SetSelectedKey(designerModuleType);
            window.Show();
        }

        private GameDesignerEditorProfile GameDesignerEditorProfile;

        public GameDesignerChoiceTree ChoiceTree;

        public void InitEditorData()
        {
            if (this.GameDesignerEditorProfile == null)
            {
                this.GameDesignerEditorProfile = AssetFinder.SafeSingleAssetFind<GameDesignerEditorProfile>("t:" + typeof(GameDesignerEditorProfile).Name);
            }
            if (this.GameDesignerEditorProfile != null)
            {

                if (this.ChoiceTree == null)
                {
                    this.ChoiceTree = new GameDesignerChoiceTree(this.GameDesignerEditorProfile);
                }
            }
        }

        private void OnGUI()
        {

            this.InitEditorData();
            if (this.GameDesignerEditorProfile != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(GUILayout.Width(200f));
                if (this.ChoiceTree != null)
                {
                    this.ChoiceTree.GUITick(() => { this.Repaint(); });
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                if (this.GameDesignerEditorProfile.CurrentGameDesignerModule != null)
                {
                    this.GameDesignerEditorProfile.ScrollPosition = EditorGUILayout.BeginScrollView(this.GameDesignerEditorProfile.ScrollPosition);
                    this.GameDesignerEditorProfile.CurrentGameDesignerModule.GUITick(ref this.GameDesignerEditorProfile);
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.ObjectField(this.GameDesignerEditorProfile, typeof(UnityEngine.Object), false);

            if (GUI.changed) { this.Repaint(); }
        }

        private void OnSelectionChange()
        {
            this.Repaint();
        }
    }

}

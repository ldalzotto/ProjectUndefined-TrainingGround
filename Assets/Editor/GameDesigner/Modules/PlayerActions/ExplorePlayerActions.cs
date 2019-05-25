using UnityEngine;
using System.Collections;
using CoreGame;
using RTPuzzle;
using UnityEditor;
using Editor_PuzzleGameCreationWizard;
using System.Collections.Generic;
using System.Linq;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class ExplorePlayerActions : IGameDesignerModule
    {
        private LevelManager levelManager;
        public CommonGameConfigurations commonGameConfigurations;
        private List<PlayerActionInherentData> levelPlayerActionInherentDatas;
        private Dictionary<PlayerActionInherentData, Editor> playerActionsCachedEditors;

        public void GUITick()
        {
            this.DisplayObjects("Player actions : ", this.levelPlayerActionInherentDatas, ref this.playerActionsCachedEditors);
        }

        public void OnDisabled()
        {

        }

        public void OnEnabled()
        {
            this.levelManager = GameObject.FindObjectOfType<LevelManager>();
            this.playerActionsCachedEditors = new Dictionary<PlayerActionInherentData, Editor>();
            this.commonGameConfigurations = new CommonGameConfigurations();
            EditorInformationsHelper.InitProperties(ref this.commonGameConfigurations);
            this.levelPlayerActionInherentDatas = this.commonGameConfigurations.PuzzleGameConfigurations.PlayerActionConfiguration.ConfigurationInherentData.Select(p => p)
              .Where(p => this.commonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration.ConfigurationInherentData[this.levelManager.LevelID].PlayerActionIds.ConvertAll(pl => pl.playerActionId).Contains(p.Key))
              .Select(p => p.Value).ToList();
        }

        private void DisplayObjects<T>(string label, List<T> objs, ref Dictionary<T, Editor> cachedEditors) where T : UnityEngine.Object
        {
            if (objs != null && objs.Count > 0)
            {
                EditorGUILayout.LabelField(label);
                EditorGUI.indentLevel += 1;
                foreach (var obj in objs)
                {
                    EditorGUILayout.ObjectField(obj, typeof(T), false);
                    if (obj != null)
                    {
                        if (!cachedEditors.ContainsKey(obj))
                        {
                            cachedEditors[obj] = Editor.CreateEditor(obj);
                        }
                        cachedEditors[obj].OnInspectorGUI();
                        EditorGUILayout.Separator();
                    }
                }
                EditorGUI.indentLevel -= 1;
            }
        }
    }
}
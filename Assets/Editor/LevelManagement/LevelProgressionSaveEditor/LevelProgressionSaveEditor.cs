using UnityEngine;
using System.Collections;
using UnityEditor;
using CoreGame;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

namespace Editor_LevelProgressionSaveEditor
{
    public class LevelProgressionSaveEditor : EditorWindow
    {
        [MenuItem("Level/LevelProgressionSaveEditor")]
        static void Init()
        {
            LevelProgressionSaveEditor window = (LevelProgressionSaveEditor)EditorWindow.GetWindow(typeof(LevelProgressionSaveEditor));
            window.Show();
        }

        private LevelAvailabilityPersistanceManager LevelAvailabilityPersistanceManager;
        private LevelAvailability CurrentLevelAvailability;
        private RegexTextFinder LevelChunkIDSearch;

        #region Visual Elements
        private Box availabilityContainer;
        #endregion

        private void OnEnable()
        {
            this.LevelAvailabilityPersistanceManager = new LevelAvailabilityPersistanceManager();
            this.CurrentLevelAvailability = this.LevelAvailabilityPersistanceManager.Load();
            this.LevelChunkIDSearch = new RegexTextFinder();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.TextArea(this.LevelAvailabilityPersistanceManager.GetDataPath());

            this.LevelChunkIDSearch.GUITick();
            Dictionary<LevelZoneChunkID, bool> newValuesToSet = null;

            foreach (var levelChunkAvailability in this.CurrentLevelAvailability.LevelZoneChunkAvailability)
            {
                if (this.LevelChunkIDSearch.IsMatchingWith(levelChunkAvailability.Key.ToString()))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.EnumPopup(levelChunkAvailability.Key);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginChangeCheck();
                    var toggelValue = EditorGUILayout.Toggle(levelChunkAvailability.Value);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (newValuesToSet == null)
                        {
                            newValuesToSet = new Dictionary<LevelZoneChunkID, bool>();
                        }
                        newValuesToSet[levelChunkAvailability.Key] = toggelValue;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            if(newValuesToSet != null)
            {
                foreach(var newValue in newValuesToSet)
                {
                    this.CurrentLevelAvailability.LevelZoneChunkAvailability[newValue.Key] = newValue.Value;
                }
            }


            if (newValuesToSet != null)
            {
                this.LevelAvailabilityPersistanceManager.Save(this.CurrentLevelAvailability);
            }

            EditorGUILayout.EndVertical();
        }

    }

}


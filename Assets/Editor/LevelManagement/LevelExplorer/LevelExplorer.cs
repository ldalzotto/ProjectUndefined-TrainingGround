using UnityEngine;
using System.Collections;
using UnityEditor;
using CoreGame;
using Editor_PuzzleGameCreationWizard;
using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;

public class LevelExplorer : EditorWindow
{
    [MenuItem("Level/LevelExplorer")]
    static void Init()
    {
        LevelExplorer window = (LevelExplorer)EditorWindow.GetWindow(typeof(LevelExplorer));
        window.Show();
    }

    private LevelManager LevelManager;

    private CommonGameConfigurations commonGameConfigurations;

    private NPCAIManager[] sceneNPCAIManagers;
    private TargetZone[] targetZones;
    private List<PlayerActionInherentData> levelPlayerActionInherentDatas;
    private LevelConfigurationData puzzleLevelConfiguration;

    private Vector2 scrollPosition;

    private void Reset()
    {
        this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
        if (this.LevelManager != null)
        {
            EditorInformationsHelper.InitProperties(ref this.commonGameConfigurations);
            this.sceneNPCAIManagers = GameObject.FindObjectsOfType<NPCAIManager>();
            this.targetZones = GameObject.FindObjectsOfType<TargetZone>();
            this.levelPlayerActionInherentDatas = this.commonGameConfigurations.PuzzleGameConfigurations.PlayerActionConfiguration.ConfigurationInherentData.Select(p => p)
                .Where(p => this.commonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration.ConfigurationInherentData[this.LevelManager.LevelID].PlayerActionIds.ConvertAll(pl => pl.playerActionId).Contains(p.Key))
                .Select(p => p.Value).ToList();
            this.puzzleLevelConfiguration = this.commonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration.ConfigurationInherentData[this.LevelManager.LevelID];
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("REFRESH"))
        {
            this.Reset();
        }

        if (this.LevelManager != null)
        {
            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
            this.DisplayObjects("Puzzle level configuration : ", new List<LevelConfigurationData>() { this.puzzleLevelConfiguration });
            this.DisplayObjects("AI : ", this.sceneNPCAIManagers.ToList());
            this.DisplayObjects("Target zones : ", this.targetZones.ToList());
            this.DisplayObjects("Player actions : ", this.levelPlayerActionInherentDatas);
            EditorGUILayout.EndScrollView();
        }
    }

    private void DisplayObjects<T>(string label, List<T> objs) where T : UnityEngine.Object
    {
        if (objs != null && objs.Count > 0)
        {
            EditorGUILayout.LabelField(label);
            EditorGUI.indentLevel += 1;
            foreach (var obj in objs)
            {
                EditorGUILayout.ObjectField(obj, typeof(T), false);
            }
            EditorGUI.indentLevel -= 1;
        }
    }

}

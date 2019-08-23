using CoreGame;
using GameConfigurationID;
using RTPuzzle;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_InteractiveObjectExplorer
{
    [System.Serializable]
    public class InteractiveObjectExplorerWindow
    {

        [SerializeField]
        private List<InteractiveObjectType> SceneInteractiveObjects;
        [SerializeField]
        private List<InteractiveObjectTypeDefinitionID> PlayerActionsInteractiveObjectDefinitions;

        [SerializeField]
        private InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration;

        public void OnEnable()
        {
            this.InteractiveObjectTypeDefinitionConfiguration = AssetFinder.SafeSingleAssetFind<InteractiveObjectTypeDefinitionConfiguration>("t:" + typeof(InteractiveObjectTypeDefinitionConfiguration).Name);

            this.SceneInteractiveObjects = GameObject.FindObjectsOfType<InteractiveObjectType>().ToList();
            var levelManager = GameObject.FindObjectOfType<LevelManager>();
            this.PlayerActionsInteractiveObjectDefinitions = new List<InteractiveObjectTypeDefinitionID>();
            if (levelManager != null)
            {
                var LevelConfiguration = AssetFinder.SafeSingleAssetFind<LevelConfiguration>("t:" + typeof(LevelConfiguration).Name);
                var PlayerActionConfiguration = AssetFinder.SafeSingleAssetFind<PlayerActionConfiguration>("t:" + typeof(PlayerActionConfiguration).Name);
                LevelConfiguration.ConfigurationInherentData[levelManager.LevelID].PlayerActionIds.ForEach((pa) =>
                {
                    var playerActionAsset = PlayerActionConfiguration.ConfigurationInherentData[pa.playerActionId];
                    if (playerActionAsset != null)
                    {
                        if (playerActionAsset.GetType() == typeof(LaunchProjectileActionInherentData))
                        {
                            PlayerActionsInteractiveObjectDefinitions.Add(((LaunchProjectileActionInherentData)playerActionAsset).projectedObjectDefinitionID);
                        }
                        else if (playerActionAsset.GetType() == typeof(AttractiveObjectActionInherentData))
                        {
                            PlayerActionsInteractiveObjectDefinitions.Add(((AttractiveObjectActionInherentData)playerActionAsset).AttractiveObjectDefinitionID);
                        }
                    }
                });
            }
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Scene : ");
            foreach (var SceneInteractiveObject in this.SceneInteractiveObjects)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(SceneInteractiveObject, typeof(InteractiveObjectType), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Player actions : ");
            foreach (var PlayerActionsInteractiveObjectDefinition in PlayerActionsInteractiveObjectDefinitions)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(this.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData[PlayerActionsInteractiveObjectDefinition], typeof(InteractiveObjectTypeDefinitionInherentData), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
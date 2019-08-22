using CoreGame;
using RTPuzzle;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class ExploreAttractiveObject : ExploreModule
    {

        private List<AttractiveObjectInherentConfigurationData> AttractiveObjectInherentConfigurationDatas = new List<AttractiveObjectInherentConfigurationData>();
        private Dictionary<AttractiveObjectInherentConfigurationData, Editor> AttractiveObjectInherentConfigurationDatasEditor = new Dictionary<AttractiveObjectInherentConfigurationData, Editor>();

        public override void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.DisplayObjects("Attractive objects : ", this.AttractiveObjectInherentConfigurationDatas, ref this.AttractiveObjectInherentConfigurationDatasEditor);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            var levelManager = GameObject.FindObjectOfType<LevelManager>();
            var attractiveObjectConfiguration = AssetFinder.SafeSingleAssetFind<AttractiveObjectConfiguration>("t:" + typeof(AttractiveObjectConfiguration).Name);
            var playerActionConfiguration = AssetFinder.SafeSingleAssetFind<PlayerActionConfiguration>("t:" + typeof(PlayerActionConfiguration).Name);
            var levelConfiguraiton = AssetFinder.SafeSingleAssetFind<LevelConfiguration>("t:" + typeof(LevelConfiguration).Name);
            var interactiveObjectDefinitionConfiguration = AssetFinder.SafeSingleAssetFind<InteractiveObjectTypeDefinitionConfiguration>("t:" + typeof(InteractiveObjectTypeDefinitionConfiguration).Name);

            this.AttractiveObjectInherentConfigurationDatas.Clear();
            var attractiveObjects = GameObject.FindObjectsOfType<AttractiveObjectModule>();
            if (attractiveObjects != null)
            {
                this.AttractiveObjectInherentConfigurationDatas.AddRange(
                          attractiveObjects.ToList().Select(ao => attractiveObjectConfiguration.ConfigurationInherentData[ao.AttractiveObjectId])
                    );
            }

            this.AttractiveObjectInherentConfigurationDatas.AddRange(
                    levelConfiguraiton.ConfigurationInherentData[levelManager.LevelID].PlayerActionIds
                        .Select(actionID => playerActionConfiguration.ConfigurationInherentData[actionID.playerActionId])
                        .Where(action => action.GetType() == typeof(AttractiveObjectActionInherentData))
                        .Select(action => interactiveObjectDefinitionConfiguration.ConfigurationInherentData[((AttractiveObjectActionInherentData)action).AttractiveObjectDefinitionID].GetDefinitionModule<AttractiveObjectModuleDefinition>().AttractiveObjectId)
                        .Select(attractiveObjectId => attractiveObjectConfiguration.ConfigurationInherentData[attractiveObjectId])
                );

        }
    }
}
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RTPuzzle
{
    public class InteractiveObjectTypeGizmos : ObjectModulesGizmo
    {
        private InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID;
        private InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration;
        private InteractiveObjectTypeDefinitionInherentData InteractiveObjectTypeDefinitionConfigurationInherentData;

        protected override IObjectGizmoDisplayEnableArea HandleGizmoDisplayAreaCreation(string moduleDefinitionType, ScriptableObject definitionSO)
        {
            if (moduleDefinitionType == typeof(ObjectRepelModuleDefinition).Name && definitionSO != null)
            {
                var ObjectRepelModuleDefinition = (ObjectRepelModuleDefinition)definitionSO;
                var ObjectRepelInherentData = this.CommonGameConfigurations.GetConfiguration<ObjectRepelConfiguration>().ConfigurationInherentData[ObjectRepelModuleDefinition.ObjectRepelID];
                return new IObjectGizmoDisplayEnableArea(true, moduleDefinitionType, new List<AdditionalEnumParameter>() { new AdditionalEnumParameter(ObjectRepelInherentData.RepelableObjectDistance.GetType(), typeof(LaunchProjectileID), ObjectRepelInherentData.RepelableObjectDistance.Values.Keys.ToList().ConvertAll(e => (Enum)e)) });
            }
            else
            {
                return new IObjectGizmoDisplayEnableArea(true, moduleDefinitionType);
            }
        }

        protected override Dictionary<Type, ScriptableObject> GetDefinitionModules()
        {
            this.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData.TryGetValue(InteractiveObjectTypeDefinitionID, out InteractiveObjectTypeDefinitionInherentData interactiveObjectTypeDefinitionConfigurationInherentData);
            this.InteractiveObjectTypeDefinitionConfigurationInherentData = interactiveObjectTypeDefinitionConfigurationInherentData;
            return this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules;
        }

        public InteractiveObjectTypeGizmos(CommonGameConfigurations commonGameConfigurations, InteractiveObjectTypeDefinitionConfiguration interactiveObjectTypeDefinitionConfiguration, InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID)
                : base(commonGameConfigurations)
        {
            CommonGameConfigurations = commonGameConfigurations;
            InteractiveObjectTypeDefinitionConfiguration = interactiveObjectTypeDefinitionConfiguration;
            this.InteractiveObjectTypeDefinitionID = InteractiveObjectTypeDefinitionID;
            base.Init();
        }

        protected override void DrawGizmo(Type moduleDefinitionType, Transform objectTransform)
        {
            var drawArea = this.GetDrawDisplay(moduleDefinitionType);
            if (drawArea.IsEnabled)
            {
                this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(moduleDefinitionType, out ScriptableObject definitionSO);
                if (definitionSO != null)
                {
                    SceneHandlerDrawer.Draw(definitionSO, objectTransform, this.CommonGameConfigurations, drawArea);
                }
            }
        }
    }

}

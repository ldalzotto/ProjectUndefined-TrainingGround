using RTPuzzle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AIManagerModuleWizard : BaseModuleWizardModule<GenericPuzzleAIBehavior, AbstractAIManager>
    {
        protected override List<AbstractAIManager> GetModules(GenericPuzzleAIBehavior RootModuleObject)
        {
            return RootModuleObject.GetComponentsInChildren<AbstractAIManager>().ToList();
        }

        protected override void OnEdit(GenericPuzzleAIBehavior RootModuleObject, Type selectedType)
        {
            AIManagerTypeSafeOperation.ForAllAIManagerTypes(selectedType,
                AIRandomPatrolComponentManangerOperation: () => { this.ProcessAIManagerEdit("AIRandomPatrolComponent", this.CommonGameConfigurations.PuzzleAICommonPrefabs.AIRandomPatrolComponentMananger, typeof(AIRandomPatrolComponentMananger)); },
                AIScriptedPatrolComponentManagerOperation: () => { this.ProcessAIManagerEdit("AIRandomPatrolComponent", this.CommonGameConfigurations.PuzzleAICommonPrefabs.AIScriptedPatrolComponentManager, typeof(AIScriptedPatrolComponentManager)); },
                AIProjectileEscapeWithCollisionManagerOperation: () => { this.ProcessAIManagerEdit("AIProjectileEscapeWithCollisionComponent", this.CommonGameConfigurations.PuzzleAICommonPrefabs.AIProjectileWithCollisionEscapeManager, typeof(AIProjectileWithCollisionEscapeManager)); },
                AIEscapeWithoutTriggerManagerOperation: () => { this.ProcessAIManagerEdit("AIEscapeWithoutTriggerComponent", this.CommonGameConfigurations.PuzzleAICommonPrefabs.AIEscapeWithoutTriggerManager, typeof(AIEscapeWithoutTriggerManager)); },
                AIFearStunManagerOperation: () => { this.ProcessAIManagerEdit("AIFearStunComponent", this.CommonGameConfigurations.PuzzleAICommonPrefabs.AIFearStunManager, typeof(AIFearStunManager)); },
                AIAttractiveObjectPersistantOperation: () => { this.ProcessAIManagerEdit("AIAttractiveObjectComponent", this.CommonGameConfigurations.PuzzleAICommonPrefabs.AIAttractiveObjectPersistantManager, typeof(AIAttractiveObjectPersistantManager)); },
                AIAttractiveObjectLooseOperation: () => { this.ProcessAIManagerEdit("AIAttractiveObjectComponent", this.CommonGameConfigurations.PuzzleAICommonPrefabs.AIAttractiveObjectLooseManager, typeof(AIAttractiveObjectLooseManager)); },
                AITargetZoneManagerOperation: () => { this.ProcessAIManagerEdit("AITargetZoneComponent", this.CommonGameConfigurations.PuzzleAICommonPrefabs.AITargetZoneEscapeManager, typeof(AITargetZoneEscapeManager)); },
                AIPlayerEscapeManagerOperation: () => { this.ProcessAIManagerEdit("AIPlayerEscapeComponent", this.CommonGameConfigurations.PuzzleAICommonPrefabs.AIPlayerEscapeManager, typeof(AIPlayerEscapeManager)); },
                AISightVisionOperation: () => { this.ProcessAISightVisionEdit(); },
                AIPlayerAttractiveOperation: () => { this.ProcessAIManagerEdit("AIPlayerAttractiveComponent", this.CommonGameConfigurations.PuzzleAICommonPrefabs.AIPlayerAttractiveManager, typeof(AIPlayerAttractiveManager)); }
            );
        }

        private void ProcessAIManagerEdit(string aiComponentFieldName, AbstractAIManager aiComponentBasePrefab, Type aiManagerType)
        {
            var aiID = this.currentSelectedObjet.GetComponent<NPCAIManager>().AiID;
            var componentsFolderPath = this.CommonGameConfigurations.InstancePath.AIBehaviorConfigurationPath + "/" + aiID.ToString() + "_Components";
            if (this.add)
            {

                var di = new DirectoryInfo(componentsFolderPath);
                if (!di.Exists)
                {
                    di.Create();
                }

                var inConfigurationAIComponents = (GenericPuzzleAIComponents)this.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration.ConfigurationInherentData[aiID].AIComponents;
                var componentField = typeof(GenericPuzzleAIComponents).GetField(aiComponentFieldName);

                if (componentField.GetValue(inConfigurationAIComponents) == null)
                {
                    var createdAIComponent = (AbstractAIComponent)ScriptableObject.CreateInstance(componentField.FieldType);
                    AssetDatabase.CreateAsset(createdAIComponent, componentsFolderPath + "/" + aiID.ToString() + "_" + aiComponentFieldName + ".asset");
                    componentField.SetValue(inConfigurationAIComponents, createdAIComponent);
                    EditorUtility.SetDirty(inConfigurationAIComponents);
                }

                if (this.currentSelectedObjet.GetComponentInChildren(aiManagerType) == null)
                {
                    var aiManagerContainer = this.currentSelectedObjet.FindChildObjectRecursively("AIManagerComponents");
                    PrefabUtility.InstantiatePrefab(aiComponentBasePrefab, aiManagerContainer.transform);
                }
            }
            else if (this.remove)
            {
                var aiComponentToDelete = AssetDatabase.LoadAssetAtPath(componentsFolderPath + "/" + aiID.ToString() + "_" + aiComponentFieldName + ".asset", typeof(AbstractAIComponent));

                if (aiComponentToDelete != null)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(aiComponentToDelete));
                }

                if (this.currentSelectedObjet.GetComponentInChildren(aiManagerType) != null)
                {
                    MonoBehaviour.DestroyImmediate(this.currentSelectedObjet.GetComponentInChildren(aiManagerType).gameObject);
                }
            }
        }

        private void ProcessAISightVisionEdit()
        {
            if (this.add)
            {
                if (this.currentSelectedObjet.GetComponentInChildren<AISightVision>() == null)
                {
                    var aiManagerContainer = this.currentSelectedObjet.FindChildObjectRecursively("AIManagerComponents");
                    PrefabUtility.InstantiatePrefab(this.CommonGameConfigurations.PuzzleAICommonPrefabs.AISightVision, aiManagerContainer.transform);
                }
            }
            else if (this.remove)
            {
                if (this.currentSelectedObjet.GetComponentInChildren<AISightVision>() != null)
                {
                    MonoBehaviour.DestroyImmediate(this.currentSelectedObjet.GetComponentInChildren<AISightVision>());
                }
            }
        }

        protected override string POIModuleDescription(Type selectedType)
        {
            string description = string.Empty;
            AIManagerTypeSafeOperation.ForAllAIManagerTypes(selectedType,
                    AIRandomPatrolComponentManangerOperation: () => { description = "Random patrolling."; },
                    AIScriptedPatrolComponentManagerOperation: () => { description = "Scripted patrolling."; },
                    AIProjectileEscapeWithCollisionManagerOperation: () => { description = "Reduce FOV when a projectile is near."; },
                    AIEscapeWithoutTriggerManagerOperation: () => { description = "Reduce FOV while not taking into account physics obstacles entity."; },
                    AIFearStunManagerOperation: () => { description = "Block any movement when FOV sum values are below a threshold."; },
                    AIAttractiveObjectPersistantOperation: () => { description = "Move to the nearest attractive point in range.\nOnce targeted, the movement is never cancelled by this component."; },
                    AIAttractiveObjectLooseOperation: () => { description = "Move to the nearest attractive point in range.\nOnce targeted, the movement is cancelled if the AI exit attractive object range."; },
                    AITargetZoneManagerOperation: () => { description = "Detect weather the AI is in the selected target zone or not."; },
                    AIPlayerEscapeManagerOperation: () => { description = "Reduce FOV when the player is near."; },
                    AISightVisionOperation: () => { description = "The sight vision of prefab."; },
                    AIPlayerAttractiveOperation: () => { description = "Moving to player strategy."; }
                );
            return description;
        }
    }

}

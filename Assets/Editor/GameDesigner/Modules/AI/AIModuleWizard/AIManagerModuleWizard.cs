using Editor_MainGameCreationWizard;
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
            if (selectedType.GetType() == typeof(ObjectSightModule))
            {
                this.ProcessAISightVisionEdit();
            }
            else
            {
                foreach (var puzzleAICommonPrefabsField in this.CommonGameConfigurations.PuzzleAICommonPrefabs.GetType().GetFields())
                {
                    if (selectedType == puzzleAICommonPrefabsField.FieldType)
                    {
                        this.ProcessAIManagerEdit(AIManagerModuleWizardConstants.AIManagerAssociatedComponent[selectedType], (AbstractAIManager)puzzleAICommonPrefabsField.GetValue(this.CommonGameConfigurations.PuzzleAICommonPrefabs), selectedType);
                    }
                }
            }

        }

        private void ProcessAIManagerEdit(string aiComponentFieldName, AbstractAIManager aiComponentBasePrefab, Type aiManagerType)
        {
            var aiID = this.currentSelectedObjet.GetComponent<NPCAIManager>().AiID;
            var componentsFolderPath = InstancePath.AIBehaviorConfigurationPath + "/" + aiID.ToString() + "_Components";
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
                if (this.currentSelectedObjet.GetComponentInChildren<ObjectSightModule>() == null)
                {
                    var aiManagerContainer = this.currentSelectedObjet.FindChildObjectRecursively("AIManagerComponents");
                    PrefabUtility.InstantiatePrefab(this.CommonGameConfigurations.PuzzleAICommonPrefabs.AISightVision, aiManagerContainer.transform);
                }
            }
            else if (this.remove)
            {
                if (this.currentSelectedObjet.GetComponentInChildren<ObjectSightModule>() != null)
                {
                    MonoBehaviour.DestroyImmediate(this.currentSelectedObjet.GetComponentInChildren<ObjectSightModule>());
                }
            }
        }

        protected override string POIModuleDescription(Type selectedType)
        {
            return AIManagerModuleWizardConstants.AIManagerDescriptionMessage[selectedType];
        }
    }

}

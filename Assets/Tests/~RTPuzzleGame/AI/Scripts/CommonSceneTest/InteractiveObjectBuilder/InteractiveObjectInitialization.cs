using CoreGame;
using GameConfigurationID;
using RTPuzzle;
using UnityEngine;

namespace Tests
{
    public class InteractiveObjectInitialization
    {
        public InteractiveObjectInitializationObject InteractiveObjectInitializationObject;
        public InteractiveObjectTypeDefinitionInherentData InteractiveObjectTypeDefinitionInherentData;
        public InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID;
        public InteractiveObjectID InteractiveObjectID;

        public void InitializeTestConfigurations(InteractiveObjectTestID interactiveObjectTestID)
        {
            var interactiveObjectTestIDTree = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID];
            var puzzleGameConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleGameConfiguration>("t:" + typeof(PuzzleGameConfiguration));

            if (InteractiveObjectInitializationObject != null)
            {
                InteractiveObjectInitializationObject.AttractiveObjectInherentConfigurationData.IfNotNull(AttractiveObjectInherentConfigurationData => puzzleGameConfiguration.AttractiveObjectConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.AttractiveObjectId] = AttractiveObjectInherentConfigurationData);
                InteractiveObjectInitializationObject.TargetZoneInherentData.IfNotNull(TargetZoneInherentData => puzzleGameConfiguration.TargetZoneConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.TargetZoneID] = TargetZoneInherentData);
                InteractiveObjectInitializationObject.LaunchProjectileInherentData.IfNotNull(LaunchProjectileInherentData => puzzleGameConfiguration.LaunchProjectileConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.LaunchProjectileID] = LaunchProjectileInherentData);
                InteractiveObjectInitializationObject.DisarmObjectInherentData.IfNotNull(DisarmObjectInherentData => puzzleGameConfiguration.DisarmObjectConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.DisarmObjectID] = DisarmObjectInherentData);
                InteractiveObjectInitializationObject.GrabObjectInherentData.IfNotNull(GrabObjectInherentData => puzzleGameConfiguration.GrabObjectConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.GrabObjectID] = GrabObjectInherentData);
                InteractiveObjectInitializationObject.ActionInteractableObjectModuleInitializationData.IfNotNull(
                    ActionInteractableObjectModuleInitializationData =>
                    {
                        puzzleGameConfiguration.ActionInteractableObjectConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.ActionInteractableObjectID] = ActionInteractableObjectModuleInitializationData.ActionInteractableObjectInherentData;
                        puzzleGameConfiguration.PlayerActionConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.PlayerActionId] = ActionInteractableObjectModuleInitializationData.AssociatedPlayerActionInherentData;
                    });
            }

            InteractiveObjectID = interactiveObjectTestIDTree.InteractiveObjectID;
            InteractiveObjectTypeDefinitionID = interactiveObjectTestIDTree.InteractiveObjectTypeDefinitionID;
            puzzleGameConfiguration.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.InteractiveObjectTypeDefinitionID] = InteractiveObjectTypeDefinitionInherentData;
        }

        public InteractiveObjectType Instanciate(Vector3 worldPosition)
        {
            var puzzleGameConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleGameConfiguration>("t:" + typeof(PuzzleGameConfiguration));
            var PuzzleStaticConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleStaticConfiguration>("t:" + typeof(PuzzleStaticConfiguration));
            InteractiveObjectType intstanciatedInteractiveObject = MonoBehaviour.Instantiate(PuzzleStaticConfiguration.PuzzlePrefabConfiguration.BaseInteractiveObjectType);
            Debug.Log(intstanciatedInteractiveObject.name);
            intstanciatedInteractiveObject.transform.position = worldPosition;
            InteractiveObjectTypeDefinitionInherentData.DefineInteractiveObject(intstanciatedInteractiveObject, PuzzleStaticConfiguration.PuzzlePrefabConfiguration, puzzleGameConfiguration);
            return intstanciatedInteractiveObject;
        }

        public InteractiveObjectType InstanciateAndInit(Vector3 worldPosition)
        {
            var intstanciatedInteractiveObject = Instanciate(worldPosition);
            intstanciatedInteractiveObject.Init(this.InteractiveObjectInitializationObject);
            return intstanciatedInteractiveObject;
        }

    }
}

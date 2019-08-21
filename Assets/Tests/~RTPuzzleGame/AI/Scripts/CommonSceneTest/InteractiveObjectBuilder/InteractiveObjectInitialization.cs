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
            var puzzleGameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().PuzzleGameConfiguration;
            InteractiveObjectInitializationObject.AttractiveObjectInherentConfigurationData.IfNotNull(AttractiveObjectInherentConfigurationData => puzzleGameConfiguration.AttractiveObjectConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.AttractiveObjectId] = AttractiveObjectInherentConfigurationData);
            InteractiveObjectInitializationObject.TargetZoneInherentData.IfNotNull(TargetZoneInherentData => puzzleGameConfiguration.TargetZoneConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.TargetZoneID] = TargetZoneInherentData);
            InteractiveObjectInitializationObject.LaunchProjectileInherentData.IfNotNull(LaunchProjectileInherentData => puzzleGameConfiguration.LaunchProjectileConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.LaunchProjectileID] = LaunchProjectileInherentData);
            InteractiveObjectInitializationObject.DisarmObjectInherentData.IfNotNull(DisarmObjectInherentData => puzzleGameConfiguration.DisarmObjectConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.DisarmObjectID] = DisarmObjectInherentData);
            InteractiveObjectInitializationObject.GrabObjectInherentData.IfNotNull(GrabObjectInherentData => puzzleGameConfiguration.GrabObjectConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.GrabObjectID] = GrabObjectInherentData);
            InteractiveObjectInitializationObject.ActionInteractableObjectInherentData.IfNotNull(ActionInteractableObjectInherentData => puzzleGameConfiguration.ActionInteractableObjectConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.ActionInteractableObjectID] = ActionInteractableObjectInherentData);


            InteractiveObjectID = interactiveObjectTestIDTree.InteractiveObjectID;
            InteractiveObjectTypeDefinitionID = interactiveObjectTestIDTree.InteractiveObjectTypeDefinitionID;
            puzzleGameConfiguration.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.InteractiveObjectTypeDefinitionID] = InteractiveObjectTypeDefinitionInherentData;
        }

        public InteractiveObjectType InstanciateAndInit(Vector3 worldPosition)
        {
            var PuzzleGameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().PuzzleGameConfiguration;
            var PuzzleStaticConfiguration = GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>().PuzzleStaticConfiguration;
            InteractiveObjectType   intstanciatedInteractiveObject = MonoBehaviour.Instantiate(PuzzleStaticConfiguration.PuzzlePrefabConfiguration.BaseInteractiveObjectType);
            intstanciatedInteractiveObject.transform.position = worldPosition;
            InteractiveObjectTypeDefinitionInherentData.DefineInteractiveObject(intstanciatedInteractiveObject, PuzzleStaticConfiguration.PuzzlePrefabConfiguration, PuzzleGameConfiguration);
            intstanciatedInteractiveObject.Init(this.InteractiveObjectInitializationObject);
            return intstanciatedInteractiveObject;
        }

    }
}

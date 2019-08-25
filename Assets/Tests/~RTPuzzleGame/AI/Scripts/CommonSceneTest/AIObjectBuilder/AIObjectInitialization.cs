using GameConfigurationID;
using RTPuzzle;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Tests
{
    public class AIObjectInitialization
    {
        public AIObjectID AIObjectID;
        public AIObjectTypeDefinitionID AIObjectTypeDefinitionID;
        public AIObjectTypeDefinitionInherentData AIObjectTypeDefinitionInherentData;

        public InteractiveObjectInitialization InteractiveObjectInitialization;

        public void InitializeTestConfigurations(AIObjectTestID AIObjectTestID, InteractiveObjectTestID InteractiveObjectTestID)
        {
            var aiObjectTestIDTree = AIObjectTestIDTree.AIObjectTestIDs[AIObjectTestID];
            var puzzleGameConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleGameConfiguration>("t:" + typeof(PuzzleGameConfiguration));

            puzzleGameConfiguration.AIObjectTypeDefinitionConfiguration.ConfigurationInherentData[aiObjectTestIDTree.AIObjectTypeDefinitionID] = AIObjectTypeDefinitionInherentData;

            InteractiveObjectInitialization.InitializeTestConfigurations(InteractiveObjectTestID);
        }

        public AIObjectType Instanciate(Vector3 worldPosition, Nullable<Quaternion> worldRotation = null)
        {
            var puzzleGameConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleGameConfiguration>("t:" + typeof(PuzzleGameConfiguration));
            var PuzzleStaticConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleStaticConfiguration>("t:" + typeof(PuzzleStaticConfiguration));
            AIObjectType instanciatedAIObjectType = MonoBehaviour.Instantiate(PuzzleStaticConfiguration.PuzzlePrefabConfiguration.BaseAIObjectType);
            instanciatedAIObjectType.transform.position = worldPosition;
            if (worldRotation != null && worldRotation.HasValue)
            {
                instanciatedAIObjectType.transform.rotation = worldRotation.Value;
            }
            AIObjectTypeDefinitionInherentData.DefineAIObject(instanciatedAIObjectType, PuzzleStaticConfiguration.PuzzlePrefabConfiguration, puzzleGameConfiguration);
            instanciatedAIObjectType.GetComponent<NavMeshAgent>().Warp(worldPosition);
            return instanciatedAIObjectType;
        }

        public AIObjectType Instanciate(Transform initialPosition)
        {
            return Instanciate(initialPosition.position, initialPosition.rotation);
        }
    }
}


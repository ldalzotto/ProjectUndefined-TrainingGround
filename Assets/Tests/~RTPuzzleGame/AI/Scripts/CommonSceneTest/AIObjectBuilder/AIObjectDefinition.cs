using CoreGame;
using GameConfigurationID;
using OdinSerializer;
using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tests
{
    public static class AIObjectDefinition
    {
        public static AIObjectInitialization GenericAIDefinition(AIObjectTestID AIObjectTestID, InteractiveObjectTestID InteractiveObjectTestID,
                    List<AbstractAIComponent> aiComponents, List<SerializedScriptableObject> interactiveObjectDefinitionModules)
        {
            var AIObjectInitialization = new AIObjectInitialization()
            {
                AIObjectID = AIObjectTestIDTree.AIObjectTestIDs[AIObjectTestID].AIObjectID,
                AIObjectTypeDefinitionID = AIObjectTestIDTree.AIObjectTestIDs[AIObjectTestID].AIObjectTypeDefinitionID,
                AIObjectTypeDefinitionInherentData = new AIObjectTypeDefinitionInherentData()
                {
                    AIObjectID = AIObjectTestIDTree.AIObjectTestIDs[AIObjectTestID].AIObjectID,
                    InteractiveObjectTypeDefinitionID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID].InteractiveObjectTypeDefinitionID,
                    GenericPuzzleAIComponents = new GenericPuzzleAIComponentsV2()
                    {
                        RangeDefinitionModules = new Dictionary<Type, ScriptableObject>() { },
                        RangeDefinitionModulesActivation = new Dictionary<Type, bool>() { },
                    }
                },
                InteractiveObjectInitialization = new InteractiveObjectInitialization()
                {
                    InteractiveObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID].InteractiveObjectID,
                    InteractiveObjectTypeDefinitionID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID].InteractiveObjectTypeDefinitionID,
                    InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                    {
                        InteractiveObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID].InteractiveObjectID,
                        InteractiveObjectSharedDataTypeInherentData = new InteractiveObjectSharedDataTypeInherentData()
                        {
                            TransformMoveManagerComponent = new TransformMoveManagerComponentV3()
                            {
                                SpeedMultiplicationFactor = 20,
                                RotationSpeed = 120
                            }
                        },
                        RangeDefinitionModules = new Dictionary<Type, ScriptableObject>() { },
                        RangeDefinitionModulesActivation = new Dictionary<Type, bool>() { }
                    }
                }
            };

            //Define ai components
            if (aiComponents != null)
            {
                foreach (var aiComponent in aiComponents)
                {
                    AIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.RangeDefinitionModules[aiComponent.GetType()] = aiComponent;
                    AIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.RangeDefinitionModulesActivation[aiComponent.GetType()] = true;
                }
            }

            //Define interactive object components
            if (interactiveObjectDefinitionModules != null)
            {
                foreach (var interactiveObjectDeifnitionModule in interactiveObjectDefinitionModules)
                {
                    AIObjectInitialization.InteractiveObjectInitialization.InteractiveObjectTypeDefinitionInherentData.RangeDefinitionModules[interactiveObjectDeifnitionModule.GetType()] = interactiveObjectDeifnitionModule;
                    AIObjectInitialization.InteractiveObjectInitialization.InteractiveObjectTypeDefinitionInherentData.RangeDefinitionModulesActivation[interactiveObjectDeifnitionModule.GetType()] = true;
                }
            }

            /*
            //Define interactive object configuration
            if (interactiveObjectConfigurations != null)
            {
                AIObjectInitialization.InteractiveObjectInitialization.InteractiveObjectInitializationObject = new InteractiveObjectInitializationObject();
                foreach (var interactiveObjectConfiguration in interactiveObjectConfigurations)
                {
                    foreach (var initialisationObjectField in AIObjectInitialization.InteractiveObjectInitialization.InteractiveObjectInitializationObject.GetType().GetFields())
                    {
                        if (initialisationObjectField.FieldType == interactiveObjectConfiguration.GetType())
                        {
                            initialisationObjectField.SetValue(AIObjectInitialization.InteractiveObjectInitialization.InteractiveObjectInitializationObject, interactiveObjectConfiguration);
                        }
                    }
                }
            }
            */
            AIObjectInitialization.InitializeTestConfigurations(AIObjectTestID, InteractiveObjectTestID);
            return AIObjectInitialization;
        }

        public static AIObjectInitialization TownAIV2(AIObjectTestID AIObjectTestID, InteractiveObjectTestID InteractiveObjectTestID)
        {
            return GenericAIDefinition(AIObjectTestID, InteractiveObjectTestID,
                new List<AbstractAIComponent>()
                {
                    new AIPatrolComponent(){ AIPatrolManagerType = AIPatrolManagerType.SCRIPTED },
                    new AIMoveTowardPlayerComponent(),
                    new AIDisarmObjectComponent(),
                    new AIAttractiveObjectComponent(){ AttractiveObjectStrategyType = AttractiveObjectStrategyType.PERSISTANT }
                },
                new List<SerializedScriptableObject>()
                {
                    new ObjectSightModuleDefinition(){
                                RangeTypeObjectDefinitionIDPicker = false,
                                RangeTypeObjectDefinitionInherentData = new RangeTypeObjectDefinitionInherentData()
                                {
                                    RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                                    {
                                        {typeof(RangeTypeDefinition), new RangeTypeDefinition(){
                                            RangeTypeID = GameConfigurationID.RangeTypeID.SIGHT_VISION,
                                            RangeShapeConfiguration = new RoundedFrustumRangeShapeConfiguration() {frustum = BaseSightFrustum }
                                        }},
                                        {typeof(RangeObstacleListenerDefinition), new RangeObstacleListenerDefinition() }
                                    },
                                    RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                                    {
                                        {typeof(RangeTypeDefinition), true},
                                        {typeof(RangeObstacleListenerDefinition), true}
                                    }
                                }
                    }
                }
            );
        }

        public static AIObjectInitialization SewersAI(AIObjectTestID AIObjectTestID, InteractiveObjectTestID InteractiveObjectTestID)
        {
            return GenericAIDefinition(AIObjectTestID, InteractiveObjectTestID,
                new List<AbstractAIComponent>()
                {
                     new AIPatrolComponent(){ MaxDistance = 15f, AIPatrolManagerType = AIPatrolManagerType.RANDOM },
                     new AIProjectileEscapeComponent() {
                         EscapeDistanceV2 = new ProjectileEscapeRange(){ Values = Enum.GetValues(typeof(LaunchProjectileID)).Cast<LaunchProjectileID>().ToDictionary(p => p, p => 25f) },
                         EscapeSemiAngleV2 = new ProjectileEscapeSemiAngle(){ Values = new Dictionary<LaunchProjectileID, float>() } },
                     new AITargetZoneComponent() { TargetZoneEscapeDistance = 50f },
                     new AIFearStunComponent() { FOVSumThreshold = 20f, TimeWhileBeginFeared = 2f},
                     new AIPlayerEscapeComponent() { EscapeDistance = 15f, PlayerDetectionRadius = -1f, EscapeSemiAngle = 90f },
                     new AIEscapeWithoutTriggerComponent() { },
                     new AIAttractiveObjectComponent(){ AttractiveObjectStrategyType = AttractiveObjectStrategyType.PERSISTANT }
                },
                new List<SerializedScriptableObject>() { }
            );
        }


        public static FrustumV2 BaseSightFrustum = new FrustumV2()
        {
            F1 = new FrustumFaceV2()
            {
                Height = 15f,
                Width = 10f
            },
            F2 = new FrustumFaceV2()
            {
                FaceOffsetFromCenter = new Vector3(0, 0, 79.23f),
                Height = 81.5f,
                Width = 50f
            }
        };

    }
}

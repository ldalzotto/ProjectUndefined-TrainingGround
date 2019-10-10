using CoreGame;
using GameConfigurationID;
using NodeGraph;
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
                        RangeDefinitionModules = new Dictionary<Type, ScriptableObject>() {
                            {typeof(AILogicColliderModuleDefinition), new AILogicColliderModuleDefinition(){ Center = Vector3.zero, Size = Vector3.one } }
                        },
                        RangeDefinitionModulesActivation = new Dictionary<Type, bool>() {
                            {typeof(AILogicColliderModuleDefinition), true }
                        }
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

            AIObjectInitialization.InitializeTestConfigurations(AIObjectTestID, InteractiveObjectTestID);
            return AIObjectInitialization;
        }

        public static AIObjectInitialization TownAIV2(AIObjectTestID AIObjectTestID, InteractiveObjectTestID InteractiveObjectTestID, bool sphereSightInfinite = false)
        {
            RTPuzzle.RangeObjectInitialization RangeObjectInitialization = null;
            if (!sphereSightInfinite)
            {
                RangeObjectInitialization = new SphereRangeObjectInitialization
                {
                    RangeTypeID = RangeTypeID.SIGHT_VISION,
                    IsTakingIntoAccountObstacles = true,
                    SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                    {
                        Radius = 99999f
                    }
                };
            }
            else
            {
                RangeObjectInitialization = new RoundedFrustumRangeObjectInitialization
                {
                    RangeTypeID = RangeTypeID.SIGHT_VISION,
                    IsTakingIntoAccountObstacles = true,
                    RoundedFrustumRangeTypeDefinition = new RoundedFrustumRangeTypeDefinition
                    {
                        FrustumV2 = BaseSightFrustum
                    }
                };
            }
            
            return GenericAIDefinition(AIObjectTestID, InteractiveObjectTestID,
                new List<AbstractAIComponent>()
                {
                    new AIMoveTowardPlayerComponent(),
                    new AIAttractiveObjectComponent(){ AttractiveObjectStrategyType = AttractiveObjectStrategyType.PERSISTANT }
                },
                new List<SerializedScriptableObject>()
                {
                }
            ); ;
        }

        public static AIObjectInitialization SewersAI(AIObjectTestID AIObjectTestID, InteractiveObjectTestID InteractiveObjectTestID)
        {
            return GenericAIDefinition(AIObjectTestID, InteractiveObjectTestID,
                new List<AbstractAIComponent>()
                {
                     new AIProjectileEscapeComponent() {
                         EscapeDistanceV2 = new ProjectileEscapeRange(){ Values = Enum.GetValues(typeof(LaunchProjectileID)).Cast<LaunchProjectileID>().ToDictionary(p => p, p => 25f) },
                         EscapeSemiAngleV2 = new ProjectileEscapeSemiAngle(){ Values = new Dictionary<LaunchProjectileID, float>() } },
                     new AITargetZoneComponent() { TargetZoneEscapeDistance = 50f },
                     new AIFearStunComponent() { FOVSumThreshold = 20f, TimeWhileBeginFeared = 2f},
                     new AIPlayerEscapeComponent() { EscapeDistance = 15f, PlayerDetectionRadius = -1f, EscapeSemiAngle = 90f },
                     new AIEscapeWithoutTriggerComponent() { },
                     new AIAttractiveObjectComponent(){ AttractiveObjectStrategyType = AttractiveObjectStrategyType.PERSISTANT }
                },
                new List<SerializedScriptableObject>() {
                    new FovModuleDefinition()
                }
            );
        }

        public static AIObjectInitialization RangeEffectTestAI(AIObjectTestID AIObjectTestID, InteractiveObjectTestID InteractiveObjectTestID)
        {
            return GenericAIDefinition(AIObjectTestID, InteractiveObjectTestID,
               new List<AbstractAIComponent>()
               {
               },
               new List<SerializedScriptableObject>()
               {
               }
           );
        }

        #region Frustum Definitions
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
        public static FrustumV2 RangeEffectManagerTestFrustum = new FrustumV2()
        {
            F1 = new FrustumFaceV2()
            {
                Height = 80f,
                Width = 80f
            },
            F2 = new FrustumFaceV2()
            {
                Height = 100f,
                Width = 100f,
                FaceOffsetFromCenter = new Vector3(0, 0, 304.2f)
            }
        };
        #endregion

      
    }
}

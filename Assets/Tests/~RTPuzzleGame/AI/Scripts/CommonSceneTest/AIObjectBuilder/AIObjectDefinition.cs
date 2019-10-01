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
            var RangeTypeObjectDefinitionInherentData = new RangeTypeObjectDefinitionInherentData()
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
            };

            if (sphereSightInfinite)
            {
                RangeTypeObjectDefinitionInherentData.RangeDefinitionModules[typeof(RangeTypeDefinition)] = new RangeTypeDefinition()
                {
                    RangeTypeID = RangeTypeID.SIGHT_VISION,
                    RangeShapeConfiguration = new SphereRangeShapeConfiguration() { Radius = 9999f }
                };
            }

            return GenericAIDefinition(AIObjectTestID, InteractiveObjectTestID,
                new List<AbstractAIComponent>()
                {
                    new AIPatrolComponent(){ AIPatrolManagerType = AIPatrolManagerType.SCRIPTED, AIPatrolGraphID = AIPatrolGraphID.NONE },
                    new AIMoveTowardPlayerComponent(),
                    new AIDisarmObjectComponent(),
                    new AIAttractiveObjectComponent(){ AttractiveObjectStrategyType = AttractiveObjectStrategyType.PERSISTANT }
                },
                new List<SerializedScriptableObject>()
                {
                    new ObjectSightModuleDefinition(){
                                RangeTypeObjectDefinitionIDPicker = false,
                                RangeTypeObjectDefinitionInherentData = RangeTypeObjectDefinitionInherentData
                    }
                }
            ); ;
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
                    new AIPatrolComponent(){ AIPatrolManagerType = AIPatrolManagerType.SCRIPTED, AIPatrolGraphID = AIPatrolGraphID.NONE }
               },
               new List<SerializedScriptableObject>()
               {
                    new ObjectSightModuleDefinition(){
                                LocalPosition = new Vector3(0, 4.65f, 1.27f),
                                RangeTypeObjectDefinitionIDPicker = false,
                                RangeTypeObjectDefinitionInherentData = new RangeTypeObjectDefinitionInherentData()
                                {
                                    RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                                    {
                                        {typeof(RangeTypeDefinition), new RangeTypeDefinition(){
                                            RangeTypeID = RangeTypeID.SIGHT_VISION,
                                            RangeShapeConfiguration = new RoundedFrustumRangeShapeConfiguration() {frustum = RangeEffectManagerTestFrustum }
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

        public static AIObjectInitialization AIPathTest(AIObjectTestID AIObjectTestID, InteractiveObjectTestID InteractiveObjectTestID, bool hasAttractive, AIPatrolGraphInherentData aIPatrolGraphInherentData)
        {
            var AIObjectIdTree = AIObjectTestIDTree.AIObjectTestIDs[AIObjectTestID];
            var AIComponents = new List<AbstractAIComponent>();

            AIComponents.Add(new AIPatrolComponent() { AIPatrolGraphID = AIObjectIdTree.AIPatrolGraphID, AIPatrolManagerType = AIPatrolManagerType.SCRIPTED });

            if (hasAttractive)
            {
                AIComponents.Add(new AIAttractiveObjectComponent() { AttractiveObjectStrategyType = AttractiveObjectStrategyType.PERSISTANT });
            }

            AssetFinder.SafeSingleAssetFind<AIPatrolGraphConfiguration>("t:" + typeof(AIPatrolGraphConfiguration).Name)
                .ConfigurationInherentData[AIObjectIdTree.AIPatrolGraphID] = aIPatrolGraphInherentData;

            return GenericAIDefinition(AIObjectTestID, InteractiveObjectTestID, AIComponents, null);
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

        #region Patrol Graph Definitions
        public static AIPatrolGraphInherentData TwoDestinationGraph(AIPositionMarkerID destination1, AIPositionMarkerID destination2)
        {
            var AIPtrolGraph = new AIPatrolGraph() { Nodes = new Dictionary<int, NodeProfile>() };

            //Start Node
            var CutsceneStartNode = new CutsceneStartNode();
            var CutsceneStartEdge = new CutsceneActionConnectionEdge();
            CutsceneStartEdge.NodeProfileRef = CutsceneStartNode;
            CutsceneStartNode.StartEdge = CutsceneStartEdge;
            AIPtrolGraph.Nodes[0] = CutsceneStartNode;

            //Branch Inifinite loop Node
            var InitialBranchInfiniteLoop = BuildACutsceneNode
                    (new BranchInfiniteLoopNode(), new BranchInfiniteLoopAction(new List<SequencedAction>()), new BranchInfiniteLoopEdge(), new List<NodeEdgeProfile>() { CutsceneStartEdge });
            AIPtrolGraph.Nodes[1] = InitialBranchInfiniteLoop;

            //First Position Node
            var FirstPositionNode = BuildACutsceneNode(new AIMoveToNode(), new AIMoveToAction(new List<SequencedAction>()) { Position = destination1 }, new AIMoveToEdge(),
                     new List<NodeEdgeProfile>() { InitialBranchInfiniteLoop.outputCutsceneConnectionEdge });
            AIPtrolGraph.Nodes[2] = FirstPositionNode;

            var SecondPositionNode = BuildACutsceneNode(new AIMoveToNode(), new AIMoveToAction(new List<SequencedAction>()) { Position = destination2 }, new AIMoveToEdge(),
                    new List<NodeEdgeProfile>() { FirstPositionNode.outputCutsceneConnectionEdge });
            AIPtrolGraph.Nodes[3] = SecondPositionNode;

            return new AIPatrolGraphInherentData() { AIPatrolGraph = AIPtrolGraph };
        }

        public static ACutsceneNode<T, E> BuildACutsceneNode<T, E>(ACutsceneNode<T, E> actionNode, T sequencedAction, E ACutsceneEdge, List<NodeEdgeProfile> backwardConnectedEdges) where T : SequencedAction where E : ACutsceneEdge<T>
        {
            var inputCutsceneEdge = new CutsceneActionConnectionEdge() { NodeProfileRef = actionNode, BackwardConnectedNodeEdges = backwardConnectedEdges };
            actionNode.InputEdges = new List<NodeEdgeProfile>() { inputCutsceneEdge };
            ACutsceneEdge.associatedAction = sequencedAction;
            actionNode.actionEdge = ACutsceneEdge;
            actionNode.inputCutsceneConnectionEdge = new CutsceneActionConnectionEdge() { BackwardConnectedNodeEdges = backwardConnectedEdges, NodeProfileRef = actionNode };
            actionNode.outputCutsceneConnectionEdge = new CutsceneActionConnectionEdge() { ConnectedNodeEdges = new List<NodeEdgeProfile>(), NodeProfileRef = actionNode };
            inputCutsceneEdge.NodeProfileRef = actionNode;
            foreach (var backwardConnectedEdge in backwardConnectedEdges)
            {
                if (backwardConnectedEdge.ConnectedNodeEdges == null) { backwardConnectedEdge.ConnectedNodeEdges = new List<NodeEdgeProfile>(); }
                backwardConnectedEdge.ConnectedNodeEdges.Add(actionNode.inputCutsceneConnectionEdge);
            }
            return actionNode;
        }
        #endregion
    }
}

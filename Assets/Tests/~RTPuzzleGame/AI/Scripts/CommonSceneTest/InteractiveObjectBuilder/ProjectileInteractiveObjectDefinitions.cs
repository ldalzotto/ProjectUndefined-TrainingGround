﻿using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{

    public static class ProjectileInteractiveObjectDefinitions
    {
        public static InteractiveObjectInitialization ExplodingProjectile(
            InteractiveObjectTestID interactiveObjectTestID,
            float explodingEffectRange, float travelDistancePerSeconds)
        {
            return new InteractiveObjectInitialization()
            {
                InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                {
                    RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                    {
                        {typeof(LaunchProjectileModuleDefinition), new LaunchProjectileModuleDefinition(){ LaunchProjectileID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].LaunchProjectileID }},
                        {typeof(ModelObjectModuleDefinition), new ModelObjectModuleDefinition() }
                    },
                    RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                    {
                        {typeof(LaunchProjectileModuleDefinition), true },
                        {typeof(ModelObjectModuleDefinition), new ModelObjectModuleDefinition() }
                    }
                },
                InteractiveObjectInitializationObject = new InteractiveObjectInitializationObject()
                {
                    LaunchProjectileInherentData = new LaunchProjectileInherentData()
                    {
                        ExplodingEffectRange = explodingEffectRange,
                        TravelDistancePerSeconds = travelDistancePerSeconds,
                        isExploding = true,
                        isPersistingToAttractiveObject = false
                    }
                }
            };
        }

        public static InteractiveObjectInitialization MutateToAttractiveProjectile(
                InteractiveObjectTestID interactiveObjectTestID,
                float explodingEffectRange, float travelDistancePerSeconds,
                float attractiveObjectEffectRange, float attractiveObjectEffectiveTime)
        {
            var InteractiveObjectInitialization = new InteractiveObjectInitialization()
                {
                    InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                    {
                        RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                        {
                            {typeof(LaunchProjectileModuleDefinition), new LaunchProjectileModuleDefinition(){ LaunchProjectileID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].LaunchProjectileID }},
                            {typeof(AttractiveObjectModuleDefinition), new AttractiveObjectModuleDefinition(){ AttractiveObjectId = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].AttractiveObjectId }},
                            {typeof(ModelObjectModuleDefinition), new ModelObjectModuleDefinition() }
                        },
                        RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                        {
                            {typeof(LaunchProjectileModuleDefinition), true},
                            {typeof(AttractiveObjectModuleDefinition), true},
                            {typeof(ModelObjectModuleDefinition), new ModelObjectModuleDefinition() }
                        }
                    },
                    InteractiveObjectInitializationObject = new InteractiveObjectInitializationObject()
                    {
                        LaunchProjectileInherentData = new LaunchProjectileInherentData()
                        {
                            ExplodingEffectRange = explodingEffectRange,
                            TravelDistancePerSeconds = travelDistancePerSeconds,
                            isExploding = false,
                            isPersistingToAttractiveObject = true
                        },
                        AttractiveObjectInherentConfigurationData = new AttractiveObjectInherentConfigurationData()
                        {
                            EffectRange = attractiveObjectEffectRange,
                            EffectiveTime = attractiveObjectEffectiveTime
                        }
                    }
                };
            InteractiveObjectInitialization.InteractiveObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].InteractiveObjectID;
            InteractiveObjectInitialization.InteractiveObjectTypeDefinitionInherentData.InteractiveObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].InteractiveObjectID;
            InteractiveObjectInitialization.InitializeTestConfigurations(interactiveObjectTestID);
            return InteractiveObjectInitialization;
        }

        public static InteractiveObjectInitialization _1_Town_Speaker(
                            InteractiveObjectTestID interactiveObjectTestID,
                            float explodingEffectRange, float travelDistancePerSeconds, float projectileThrowRange,
                            float attractiveObjectEffectRange, float attractiveObjectEffectiveTime,
                            float grabObjectRadius,
                            float disarmInteractionRange, float disarmTime)
        {
            var InteractiveObjectInitialization =
                new InteractiveObjectInitialization()
                {
                    InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                    {
                        RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                       {
                        {typeof(LaunchProjectileModuleDefinition), new LaunchProjectileModuleDefinition(){ LaunchProjectileID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].LaunchProjectileID }},
                        {typeof(AttractiveObjectModuleDefinition), new AttractiveObjectModuleDefinition(){ AttractiveObjectId = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].AttractiveObjectId }},
                        {typeof(ModelObjectModuleDefinition), new ModelObjectModuleDefinition() }
                       },
                        RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                       {
                        {typeof(LaunchProjectileModuleDefinition), true},
                        {typeof(AttractiveObjectModuleDefinition), true},
                        {typeof(ModelObjectModuleDefinition), true }
                       }
                    },
                    InteractiveObjectInitializationObject = new InteractiveObjectInitializationObject()
                    {
                        LaunchProjectileInherentData = new LaunchProjectileInherentData()
                        {
                            ProjectileThrowRange = projectileThrowRange,
                            ExplodingEffectRange = explodingEffectRange,
                            TravelDistancePerSeconds = travelDistancePerSeconds,
                            isExploding = false,
                            isPersistingToAttractiveObject = true
                        },
                        AttractiveObjectInherentConfigurationData = new AttractiveObjectInherentConfigurationData()
                        {
                            EffectRange = attractiveObjectEffectRange,
                            EffectiveTime = attractiveObjectEffectiveTime
                        },
                    }
                };

            InteractiveObjectInitialization.InteractiveObjectTypeDefinitionInherentData.InteractiveObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].InteractiveObjectID;
            InteractiveObjectInitialization.InitializeTestConfigurations(interactiveObjectTestID);
            return InteractiveObjectInitialization;
        }
    }

}

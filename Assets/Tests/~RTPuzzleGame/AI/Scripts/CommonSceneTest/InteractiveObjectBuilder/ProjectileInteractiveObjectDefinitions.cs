using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    /*
     * 
     * 
             public void Init(float explodingEffectRange, float projectileThrowRange, float travelDistancePerSeconds, InteractiveObjectType projectilePrefab, bool isExploding, bool isPersistingToAttractiveObject)
        {
            this.ExplodingEffectRange = explodingEffectRange;
            this.ProjectileThrowRange = projectileThrowRange;
            this.TravelDistancePerSeconds = travelDistancePerSeconds;
            this.AssociatedInteractiveObjectType = projectilePrefab;
            this.isExploding = isExploding;
            this.isPersistingToAttractiveObject = isPersistingToAttractiveObject;
        }
         */

    public static class ProjectileInteractiveObjectDefinitions
    {
        public static InteractiveObjectInitialization ExplodingProjectile(float explodingEffectRange, float travelDistancePerSeconds)
        {
            return new InteractiveObjectInitialization()
            {
                InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                {
                    RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                    {
                        {typeof(LaunchProjectileModuleDefinition), new LaunchProjectileModuleDefinition(){ LaunchProjectileID = GameConfigurationID.LaunchProjectileID.TEST_PROJECTILE_EXPLODE }},
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

        public static InteractiveObjectInitialization MutateToAttractiveProjectile(float explodingEffectRange, float travelDistancePerSeconds, float attractiveObjectEffectRange, float attractiveObjectEffectiveTime)
        {
            return new InteractiveObjectInitialization()
            {
                InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                {
                    RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                    {
                        {typeof(LaunchProjectileModuleDefinition), new LaunchProjectileModuleDefinition(){ LaunchProjectileID = GameConfigurationID.LaunchProjectileID.TEST_PROJECTILE_TOATTRACTIVE }},
                        {typeof(AttractiveObjectModuleDefinition), new AttractiveObjectModuleDefinition(){ }},
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
        }

        public static InteractiveObjectInitialization _1_Town_Speaker(
                            float explodingEffectRange, float travelDistancePerSeconds, float projectileThrowRange,
                            float attractiveObjectEffectRange, float attractiveObjectEffectiveTime, 
                            float grabObjectRadius,
                            float disarmInteractionRange, float disarmTime)
        {
            return new InteractiveObjectInitialization()
            {
                InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                {
                    RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                    {
                        {typeof(LaunchProjectileModuleDefinition), new LaunchProjectileModuleDefinition(){ LaunchProjectileID = GameConfigurationID.LaunchProjectileID.TEST_PROJECTILE_TOATTRACTIVE }},
                        {typeof(AttractiveObjectModuleDefinition), new AttractiveObjectModuleDefinition(){ }},
                        {typeof(DisarmObjectModuleDefinition), new DisarmObjectModuleDefinition(){ } },
                        {typeof(GrabObjectModuleDefinition), new GrabObjectModuleDefinition(){ } },
                        {typeof(ModelObjectModuleDefinition), new ModelObjectModuleDefinition() }
                    },
                    RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                    {
                        {typeof(LaunchProjectileModuleDefinition), true},
                        {typeof(AttractiveObjectModuleDefinition), true},
                        {typeof(DisarmObjectModuleDefinition), true},
                        {typeof(GrabObjectModuleDefinition), true },
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
                    GrabObjectInherentData = new GrabObjectInherentData()
                    {
                        EffectRadius = grabObjectRadius
                    },
                    DisarmObjectInherentData = new DisarmObjectInherentData()
                    {
                        DisarmInteractionRange = disarmInteractionRange,
                        DisarmTime = disarmTime
                    }
                }
            };
        }
    }

}

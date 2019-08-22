//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Editor_GameDesigner
{
    using System.Collections.Generic;
    using RTPuzzle;
    using System;
    
    
    public class AIManagerModuleWizardConstants
    {
        
        public static System.Collections.Generic.Dictionary<System.Type, string> AIManagerDescriptionMessage = new Dictionary<Type, string>(){
{typeof(ObjectSightModule),"The sight vision of prefab."},
{typeof(AIAttractiveObjectLooseManager),"Move to the nearest attractive point in range. Once targeted, the movement is cancelled if the AI exit attractive object range."},
{typeof(AIAttractiveObjectPersistantManager),"Move to the nearest attractive point in range. Once targeted, the movement is never cancelled by this component."},
{typeof(AIEscapeWithoutTriggerManager),"Reduce FOV while not taking into account physics obstacles entity."},
{typeof(AIFearStunManager),"Block any movement when FOV sum values are below a threshold."},
{typeof(AIMoveTowardPlayerManager),"Moving to player strategy."},
{typeof(AIRandomPatrolComponentMananger),"Random patrolling."},
{typeof(AIScriptedPatrolComponentManager),"Scripted patrolling."},
{typeof(AIPlayerEscapeManager),"Reduce FOV when the player is near."},
{typeof(AIProjectileWithCollisionEscapeManager),"Reduce FOV when a projectile is near."},
{typeof(AITargetZoneEscapeManager),"Detect weather the AI is in the selected target zone or not."},
{typeof(AIManagerModuleWizardConstants),""},
{typeof(AIDisarmObjectManager),""},
};
        
        public static System.Collections.Generic.Dictionary<System.Type, string> AIManagerAssociatedComponent = new Dictionary<Type, string>(){
{typeof(ObjectSightModule),""},
{typeof(AIAttractiveObjectLooseManager),"AIAttractiveObjectComponent"},
{typeof(AIAttractiveObjectPersistantManager),"AIAttractiveObjectComponent"},
{typeof(AIEscapeWithoutTriggerManager),"AIEscapeWithoutTriggerComponent"},
{typeof(AIFearStunManager),"AIFearStunComponent"},
{typeof(AIMoveTowardPlayerManager),"AIMoveTowardPlayerComponent"},
{typeof(AIRandomPatrolComponentMananger),"AIRandomPatrolComponent"},
{typeof(AIScriptedPatrolComponentManager),"AIRandomPatrolComponent"},
{typeof(AIPlayerEscapeManager),"AIPlayerEscapeComponent"},
{typeof(AIProjectileWithCollisionEscapeManager),"AIProjectileEscapeWithCollisionComponent"},
{typeof(AITargetZoneEscapeManager),"AITargetZoneComponent"},
{typeof(AIManagerModuleWizardConstants),"AIModuleTestComponent"},
{typeof(AIDisarmObjectManager),"AIDisarmObjectComponent"},
};
    }
}

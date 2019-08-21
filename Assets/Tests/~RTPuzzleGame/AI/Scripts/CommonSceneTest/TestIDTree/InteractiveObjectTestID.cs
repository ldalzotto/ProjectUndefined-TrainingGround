using GameConfigurationID;
using System.Collections.Generic;

namespace Tests
{

    public enum InteractiveObjectTestID
    {
        TEST_1,
        TEST_2
    }

    public class InteractiveObjectTestIDTree
    {
        public TargetZoneID TargetZoneID;
        public InteractiveObjectID InteractiveObjectID;
        public NearPlayerGameOverTriggerID NearPlayerGameOverTriggerID;
        public LaunchProjectileID LaunchProjectileID;
        public AttractiveObjectId AttractiveObjectId;
        public DisarmObjectID DisarmObjectID;
        public GrabObjectID GrabObjectID;
        public ActionInteractableObjectID ActionInteractableObjectID;
        public InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID;

        public InteractiveObjectTestIDTree(TargetZoneID targetZoneID, InteractiveObjectID interactiveObjectID, NearPlayerGameOverTriggerID nearPlayerGameOverTriggerID,
            LaunchProjectileID launchProjectileID, AttractiveObjectId attractiveObjectId, DisarmObjectID disarmObjectID, GrabObjectID grabObjectID, ActionInteractableObjectID actionInteractableObjectID,
            InteractiveObjectTypeDefinitionID interactiveObjectTypeDefinitionID)
        {
            TargetZoneID = targetZoneID;
            InteractiveObjectID = interactiveObjectID;
            NearPlayerGameOverTriggerID = nearPlayerGameOverTriggerID;
            LaunchProjectileID = launchProjectileID;
            AttractiveObjectId = attractiveObjectId;
            DisarmObjectID = disarmObjectID;
            GrabObjectID = grabObjectID;
            ActionInteractableObjectID = actionInteractableObjectID;
            InteractiveObjectTypeDefinitionID = interactiveObjectTypeDefinitionID;
        }

        public static Dictionary<InteractiveObjectTestID, InteractiveObjectTestIDTree> InteractiveObjectTestIDs = new Dictionary<InteractiveObjectTestID, InteractiveObjectTestIDTree>()
        {
            {InteractiveObjectTestID.TEST_1, new InteractiveObjectTestIDTree(TargetZoneID.TEST_1,InteractiveObjectID.TEST_1, NearPlayerGameOverTriggerID.TEST_1,LaunchProjectileID.TEST_1,
                AttractiveObjectId.TEST_1, DisarmObjectID.TEST_1,GrabObjectID.TEST_1,ActionInteractableObjectID.TEST_1, InteractiveObjectTypeDefinitionID.TEST_1) },
            {InteractiveObjectTestID.TEST_2, new InteractiveObjectTestIDTree(TargetZoneID.TEST_2,InteractiveObjectID.TEST_2, NearPlayerGameOverTriggerID.TEST_2,LaunchProjectileID.TEST_2,
                AttractiveObjectId.TEST_2, DisarmObjectID.TEST_2,GrabObjectID.TEST_2,ActionInteractableObjectID.TEST_2, InteractiveObjectTypeDefinitionID.TEST_2) }
        };
    }

}

using GameConfigurationID;
using System.Collections.Generic;

namespace Tests
{
    public enum AIObjectTestID
    {
        TEST_1 = 0,
    }

    public class AIObjectTestIDTree
    {
        public AIObjectID AIObjectID;
        public AIObjectTypeDefinitionID AIObjectTypeDefinitionID;
        public AIPatrolGraphID AIPatrolGraphID;

        public AIObjectTestIDTree(AIObjectID aIObjectID, AIObjectTypeDefinitionID aIObjectTypeDefinitionID, AIPatrolGraphID aIPatrolGraphID)
        {
            AIObjectID = aIObjectID;
            AIObjectTypeDefinitionID = aIObjectTypeDefinitionID;
            AIPatrolGraphID = aIPatrolGraphID;
        }

        public static Dictionary<AIObjectTestID, AIObjectTestIDTree> AIObjectTestIDs = new Dictionary<AIObjectTestID, AIObjectTestIDTree>()
        {
            {AIObjectTestID.TEST_1, new AIObjectTestIDTree(AIObjectID.TEST_1, AIObjectTypeDefinitionID.TEST_1, AIPatrolGraphID.TEST_1) }
        };
    }
}

using UnityEngine;
using System.Collections;
using OdinSerializer;
using RTPuzzle;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class AIPatrolSystemDefinition : SerializedScriptableObject
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public AIPatrolGraphV2 AIPatrolGraph;
    }

    public class AIPatrollingState
    {
        public bool isPatrolling;
    }

}

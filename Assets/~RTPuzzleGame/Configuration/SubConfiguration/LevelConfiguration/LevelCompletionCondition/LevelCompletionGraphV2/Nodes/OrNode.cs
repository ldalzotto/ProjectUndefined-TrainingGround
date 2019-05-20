using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    public class OrNode : ALogicExecutionNode
    {
        public override ConditionType ConditionType => ConditionType.OR;
    }
}
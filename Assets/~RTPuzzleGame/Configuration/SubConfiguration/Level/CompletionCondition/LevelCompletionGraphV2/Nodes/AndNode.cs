using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    public class AndNode : ALogicExecutionNode
    {
        public override ConditionType ConditionType => ConditionType.AND;

    }
}
using UnityEngine;
using System.Collections;
using OdinSerializer;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeObjectV2DefinitionInherentData", menuName = "Configuration/PuzzleGame/RangeTypeObjectDefinitionConfiguration/RangeObjectV2DefinitionInherentData", order = 1)]
    public class RangeObjectV2DefinitionInherentData : SerializedScriptableObject
    {
        public RangeObjectInitialization RangeObjectInitialization;
    }


    public static class RangeTypeObjectDefinitionConfigurationInherentDataBuilderV2
    {
        public static RangeObjectV2DefinitionInherentData SphereRangeWithObstacleListener(float sphereRange, RangeTypeID rangeTypeID)
        {
            return new RangeObjectV2DefinitionInherentData
            {
                RangeObjectInitialization = new SphereRangeObjectInitialization
                {
                    RangeTypeID = rangeTypeID,
                    IsTakingIntoAccountObstacles = true,
                    SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                    {
                        Radius = sphereRange
                    }
                }
            };
        }
    }
}

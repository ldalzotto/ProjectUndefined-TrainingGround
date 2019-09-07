using UnityEngine;
using System.Collections;
using GameConfigurationID;
using System.Collections.Generic;

namespace Tests
{
    public enum PointOfInterestTestID
    {
        TEST_1,
        TEST_2
    }

    public class PointOfInterestIDTree 
    {
        public PointOfInterestId PointOfInterestId;
        public PointOfInterestDefinitionID PointOfInterestDefinitionID;
        public PointOfInterestVisualMovementID PointOfInterestVisualMovementID;

        public PointOfInterestIDTree(PointOfInterestId pointOfInterestId, PointOfInterestDefinitionID pointOfInterestDefinitionID, PointOfInterestVisualMovementID pointOfInterestVisualMovementID)
        {
            PointOfInterestId = pointOfInterestId;
            PointOfInterestDefinitionID = pointOfInterestDefinitionID;
            PointOfInterestVisualMovementID = pointOfInterestVisualMovementID;
        }

        public static Dictionary<PointOfInterestTestID, PointOfInterestIDTree> PointOfInterestTestIDs = new Dictionary<PointOfInterestTestID, PointOfInterestIDTree>()
        {
            {PointOfInterestTestID.TEST_1, new PointOfInterestIDTree(PointOfInterestId.TEST_1, PointOfInterestDefinitionID.TEST_1, PointOfInterestVisualMovementID.TEST_1 ) },
            {PointOfInterestTestID.TEST_2, new PointOfInterestIDTree(PointOfInterestId.TEST_2, PointOfInterestDefinitionID.TEST_2, PointOfInterestVisualMovementID.TEST_2 ) },
        };
    }

}

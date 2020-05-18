﻿using UnityEngine;
using System.Collections;
using ConfigurationEditor;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PointOfInterestConfiguration", menuName = "Configuration/AdventureGame/PointOfInterestConfiguration/PointOfInterestConfiguration", order = 1)]
    public class PointOfInterestConfiguration : ConfigurationSerialization<PointOfInterestId, PointOfInterestInherentData>
    {
    }

    public enum PointOfInterestId
    {
        NONE = 0,
        BOUNCER = 1,
        ID_CARD = 2,
        ID_CARD_V2 = 3,
        PLAYER = 4,
        DUMBSTER = 5,
        CROWBAR = 6,
        SEWER_ENTRANCE = 7,
        SEWER_EXIT = 8,
        SEWER_TO_PUZZLE = 9
    }

}

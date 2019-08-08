using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class CutscenePositionsType : AbstractPositionsType<CutscenePositionMarkerID, CutscenePositionMarker>
    {
        [CustomEnum(isCreateable: true)]
        public CutsceneId CutsceneId;
    }
}

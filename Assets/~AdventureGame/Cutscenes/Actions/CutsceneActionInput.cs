﻿using CoreGame;
using GameConfigurationID;

namespace AdventureGame
{
    public class CutsceneActionInput : SequencedActionInput
    {
        private CutsceneId cutsceneId;
        private PointOfInterestManager pointOfInterestManager;
        private CutscenePositionsManager cutscenePositionsManager;
        private ContextActionEventManager contextActionEventManager;
        private AdventureGameConfigurationManager adventureGameConfigurationManager;

        public CutsceneActionInput(CutsceneId cutsceneId, PointOfInterestManager pointOfInterestManager,
            CutscenePositionsManager cutscenePositionsManager, ContextActionEventManager contextActionEventManager, AdventureGameConfigurationManager AdventureGameConfigurationManager)
        {
            this.cutsceneId = cutsceneId;
            this.pointOfInterestManager = pointOfInterestManager;
            this.cutscenePositionsManager = cutscenePositionsManager;
            this.contextActionEventManager = contextActionEventManager;
            this.adventureGameConfigurationManager = AdventureGameConfigurationManager;
        }

        public CutsceneId CutsceneId { get => cutsceneId; }
        public CutscenePositionsManager CutscenePositionsManager { get => cutscenePositionsManager; }
        public PointOfInterestManager PointOfInterestManager { get => pointOfInterestManager; }
        public ContextActionEventManager ContextActionEventManager { get => contextActionEventManager; }
        public AdventureGameConfigurationManager AdventureGameConfigurationManager { get => adventureGameConfigurationManager; }
    }
}

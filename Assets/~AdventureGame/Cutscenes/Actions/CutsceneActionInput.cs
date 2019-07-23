using CoreGame;
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
        private CutsceneGlobalController cutsceneGlobalController;
        private GhostsPOIManager ghostsPOIManager;
        private LevelManager levelManager;

        public CutsceneActionInput(CutsceneId cutsceneId, PointOfInterestManager pointOfInterestManager,
            CutscenePositionsManager cutscenePositionsManager, ContextActionEventManager contextActionEventManager, AdventureGameConfigurationManager AdventureGameConfigurationManager, CutsceneGlobalController CutsceneGlobalController,
            GhostsPOIManager ghostsPOIManager, LevelManager levelManager)
        {
            this.cutsceneId = cutsceneId;
            this.pointOfInterestManager = pointOfInterestManager;
            this.cutscenePositionsManager = cutscenePositionsManager;
            this.contextActionEventManager = contextActionEventManager;
            this.adventureGameConfigurationManager = AdventureGameConfigurationManager;
            this.cutsceneGlobalController = CutsceneGlobalController;
            this.ghostsPOIManager = ghostsPOIManager;
            this.levelManager = levelManager;
        }

        public CutsceneId CutsceneId { get => cutsceneId; }
        public CutscenePositionsManager CutscenePositionsManager { get => cutscenePositionsManager; }
        public PointOfInterestManager PointOfInterestManager { get => pointOfInterestManager; }
        public ContextActionEventManager ContextActionEventManager { get => contextActionEventManager; }
        public AdventureGameConfigurationManager AdventureGameConfigurationManager { get => adventureGameConfigurationManager; }
        public CutsceneGlobalController CutsceneGlobalController { get => cutsceneGlobalController;  }
        public GhostsPOIManager GhostsPOIManager { get => ghostsPOIManager;}
        public LevelManager LevelManager { get => levelManager;  }
    }
}

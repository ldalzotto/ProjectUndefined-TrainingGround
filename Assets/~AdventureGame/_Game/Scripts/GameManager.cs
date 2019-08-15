using CoreGame;
using System.Linq;
using UnityEngine;

namespace AdventureGame
{
    public class GameManager : AsbtractCoreGameManager
    {

        private ContextActionManager ContextActionManager;
        private ContextActionWheelManager ContextActionWheelManager;
        private PlayerManager PlayerManager;
        private NPCManager NPCManager;
        private InventoryManager InventoryManager;
        private DiscussionWindowsContainer DiscussionWindowsContainer;
        private PointOfInterestManager PointOfInterestManager;
        private CutscenePlayerManagerV2 CutscenePlayerManagerV2;
        private CameraMovementManager CameraMovementManager;

#if UNITY_EDITOR
        private EditorOnlyModules EditorOnlyModules = new EditorOnlyModules();
#endif

        private void Awake()
        {
            GameObject.FindObjectOfType<GameManagerPersistanceInstance>().Init();
            this.AfterGameManagerPersistanceInstanceInitialization();
            base.OnAwake(LevelType.ADVENTURE);
        }

        protected virtual void AfterGameManagerPersistanceInstanceInitialization() { }

        void Start()
        {
            base.OnStart();
            
            //load dynamic POI
            var allLoadedLevelChunkID = GameObject.FindObjectOfType<LevelManager>().AllLoadedLevelZonesChunkID;
            var allActivePOIIds = GameObject.FindObjectsOfType<APointOfInterestType>().ToList().ConvertAll(p => p.PointOfInterestId);
            foreach (var elligiblePOIIdTo in CoreGameSingletonInstances.AGhostPOIManager.GetAllPOIIdElligibleToBeDynamicallyInstanciated(allLoadedLevelChunkID))
            {
                if (!allActivePOIIds.Contains(elligiblePOIIdTo))
                {
                    PointOfInterestType.Instanciate(elligiblePOIIdTo);
                }
            }

            var InventoryMenu = AInventoryMenu.FindCurrentInstance();
            InventoryMenu.gameObject.SetActive(true);

            ContextActionManager = FindObjectOfType<ContextActionManager>();
            ContextActionWheelManager = FindObjectOfType<ContextActionWheelManager>();
            PlayerManager = FindObjectOfType<PlayerManager>();
            NPCManager = FindObjectOfType<NPCManager>();
            InventoryManager = FindObjectOfType<InventoryManager>();
            DiscussionWindowsContainer = FindObjectOfType<DiscussionWindowsContainer>();
            PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            CutscenePlayerManagerV2 = GameObject.FindObjectOfType<CutscenePlayerManagerV2>();
            CameraMovementManager = GameObject.FindObjectOfType<CameraMovementManager>();

            //initialization
            CameraMovementManager.Init();
            GameObject.FindObjectOfType<CutscenePositionsManager>().Init();
            CutscenePlayerManagerV2.Init();
            PlayerManager.Init();
            FindObjectOfType<InventoryEventManager>().Init();
            GameObject.FindObjectOfType<InventoryMenu>().Init();
            InventoryManager.Init();
            PointOfInterestManager.Init();
            GameObject.FindObjectOfType<CutsceneGlobalController>().Init();
            GameObject.FindObjectOfType<CutsceneEventManager>().Init();
            DiscussionWindowsContainer.Init();

#if UNITY_EDITOR
            this.EditorOnlyModules.Init();
#endif

        }

        void Update()
        {
            var d = Time.deltaTime;

            this.BeforeTick(d);

            CutscenePlayerManagerV2.Tick(d);
            ContextActionWheelManager.Tick(d);
            ContextActionManager.Tick(d);
            PointOfInterestManager.Tick(d);
            PlayerManager.Tick(d);
            NPCManager.Tick(d);
            CameraMovementManager.Tick(d);
            DiscussionWindowsContainer.Tick(d);
            InventoryManager.Tick(d);

#if UNITY_EDITOR
            this.EditorOnlyModules.Tick(d);
#endif
        }

        private void FixedUpdate()
        {
            var d = Time.fixedDeltaTime;
            PlayerManager.FixedTick(d);
            NPCManager.FixedTick(d);
        }

        private void LateUpdate()
        {
            var d = Time.deltaTime;
            PointOfInterestManager.LateTick(d);
            PlayerManager.LateTick(d);
        }

        private void OnGUI()
        {
         
        }

        private void OnDrawGizmos()
        {
            if (PlayerManager != null)
            {
                PlayerManager.OnGizmoTick();
            }
        }


    }

#if UNITY_EDITOR
    class EditorOnlyModules
    {
        private AdventureDebugModule AdventureDebugModule;

        public void Init()
        {
            this.AdventureDebugModule = GameObject.FindObjectOfType<AdventureDebugModule>();
            this.AdventureDebugModule.Init();
        }

        public void Tick(float d)
        {
            this.AdventureDebugModule.Tick(d);
        }
    }
#endif

}

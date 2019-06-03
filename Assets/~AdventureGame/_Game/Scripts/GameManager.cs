using CoreGame;
using System.Collections;
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
        private DiscussionWindowManager DiscussionWindowManager;
        private AdventureLevelChunkFXTransitionManager AdventureLevelChunkFXTransitionManager;

        private void Awake()
        {
            GameObject.FindObjectOfType<GameManagerPersistanceInstance>().Init();

            //Level chunk initialization
            base.OnAwake();
            GameObject.FindObjectOfType<LevelManager>().Init(LevelType.ADVENTURE);
        }

        void Start()
        {
            var InventoryMenu = AInventoryMenu.FindCurrentInstance();
            InventoryMenu.gameObject.SetActive(true);

            ContextActionManager = FindObjectOfType<ContextActionManager>();
            ContextActionWheelManager = FindObjectOfType<ContextActionWheelManager>();
            this.PointOfInterestInitialisation();
            PlayerManager = FindObjectOfType<PlayerManager>();
            NPCManager = FindObjectOfType<NPCManager>();
            InventoryManager = FindObjectOfType<InventoryManager>();
            DiscussionWindowManager = FindObjectOfType<DiscussionWindowManager>();
            AdventureLevelChunkFXTransitionManager = GameObject.FindObjectOfType<AdventureLevelChunkFXTransitionManager>();
            
            //initialization
            GameObject.FindObjectOfType<AbstractLevelTransitionManager>().Init();
            AdventureLevelChunkFXTransitionManager.Init();
            PlayerManager.Init();
            StartCoroutine(PointOfInterestInitialisationAtEndOfFrame());
            FindObjectOfType<InventoryEventManager>().Init();
            GameObject.FindObjectOfType<InventoryMenu>().Init();
            InventoryManager.Init();
            FindObjectOfType<PointOfInterestEventManager>().Init();
            GameObject.FindObjectOfType<AdventureEventManager>().Init();

            var WayPointPathContainer = FindObjectOfType<WayPointPathContainer>();
            if (WayPointPathContainer != null)
            {
                WayPointPathContainer.Init();
            }

        }

        void Update()
        {
            var d = Time.deltaTime;

            AdventureLevelChunkFXTransitionManager.Tick(d);
            ContextActionWheelManager.Tick(d);
            ContextActionManager.Tick(d);
            PlayerManager.Tick(d);
            NPCManager.Tick(d);
            InventoryManager.Tick(d);
            DiscussionWindowManager.Tick(d);
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
            PlayerManager.LateTick(d);
        }

        private void OnGUI()
        {
            DiscussionWindowManager.GUITick();
        }

        private void OnDrawGizmos()
        {
            if (PlayerManager != null)
            {
                PlayerManager.OnGizmoTick();
            }
        }

        private IEnumerator PointOfInterestInitialisationAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            var allActivePOI = GameObject.FindObjectsOfType<PointOfInterestType>();
            if (allActivePOI != null)
            {
                for (var i = 0; i < allActivePOI.Length; i++)
                {
                    allActivePOI[i].Init_EndOfFrame();
                }
            }
        }

        private void PointOfInterestInitialisation()
        {
            var allActivePOI = GameObject.FindObjectsOfType<PointOfInterestType>();
            if (allActivePOI != null)
            {
                for (var i = 0; i < allActivePOI.Length; i++)
                {
                    allActivePOI[i].Init();
                }
            }
        }

    }

}

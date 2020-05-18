﻿using System.Collections;
using System.Linq;
using UnityEngine;

namespace CoreGame
{
    public abstract class AsbtractCoreGameManager : MonoBehaviour
    {
        private Coroutiner Coroutiner;

        private ATimelinesManager ATimelinesManager;
        private GameInputManager GameInputManager;
        private PersistanceManager PersistanceManager;
        protected AGhostPOIManager AGhostPOIManager;
        private LevelChunkFXTransitionManager LevelChunkFXTransitionManager;
        private TutorialManager TutorialManager;

        protected void OnAwake(LevelType levelType)
        {
            this.PersistanceManager = GameObject.FindObjectOfType<PersistanceManager>();
            this.ATimelinesManager = GameObject.FindObjectOfType<ATimelinesManager>();
            this.GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            this.Coroutiner = GameObject.FindObjectOfType<Coroutiner>();
            this.AGhostPOIManager = GameObject.FindObjectOfType<AGhostPOIManager>();
            this.LevelChunkFXTransitionManager = GameObject.FindObjectOfType<LevelChunkFXTransitionManager>();
            this.TutorialManager = GameObject.FindObjectOfType<TutorialManager>();

            this.PersistanceManager.Init();
            this.GameInputManager.Init();
            GameObject.FindObjectOfType<LevelAvailabilityManager>().Init();
            this.AGhostPOIManager.Init();
            this.ATimelinesManager.Init();
            GameObject.FindObjectOfType<TimelinesEventManager>().Init();
            GameObject.FindObjectOfType<LevelTransitionManager>().Init();
            GameObject.FindObjectOfType<LevelManagerEventManager>().Init();
            GameObject.FindObjectOfType<PlayerAdventurePositionManager>().Init();
            GameObject.FindObjectOfType<APointOfInterestEventManager>().Init();
            GameObject.FindObjectOfType<LevelManager>().Init(levelType);
            this.LevelChunkFXTransitionManager.Init();
            GameObject.FindObjectOfType<DiscussionPositionManager>().Init();
            this.TutorialManager.Init();

            Coroutiner.StartCoroutine(this.InitializeTimelinesAtEndOfFrame());
        }


        protected void OnStart()
        {
            this.PointOfInterestInitialisation();
            this.Coroutiner.StartCoroutine(this.PointOfInterestInitialisationAtEndOfFrame());
        }

        protected void BeforeTick(float d)
        {
            this.PersistanceManager.Tick(d);
            this.LevelChunkFXTransitionManager.Tick(d);
            this.TutorialManager.Tick(d);
        }

        private IEnumerator InitializeTimelinesAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            ATimelinesManager.InitTimelinesAtEndOfFrame();
        }

        private void PointOfInterestInitialisation()
        {
            var allActivePOI = GameObject.FindObjectsOfType<APointOfInterestType>();
            if (allActivePOI != null)
            {
                for (var i = 0; i < allActivePOI.Length; i++)
                {
                    allActivePOI[i].Init();
                }
            }
        }

        private IEnumerator PointOfInterestInitialisationAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            var allActivePOI = GameObject.FindObjectsOfType<APointOfInterestType>();
            if (allActivePOI != null)
            {
                for (var i = 0; i < allActivePOI.Length; i++)
                {
                    allActivePOI[i].Init_EndOfFrame();
                }
            }
        }



    }
}


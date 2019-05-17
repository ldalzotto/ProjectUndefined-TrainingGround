using UnityEngine;
using System.Collections;
using CoreGame;
using System;

namespace AdventureGame
{
    public class AdventureLevelChunkFXTransitionManager : MonoBehaviour
    {

        private CurrentTransitionableLevelFXTypeManager CurrentTransitionableLevelFXTypeManager;
        private FXTransitionAnimationManager FXTransitionAnimationManager;

        public void Init()
        {
            var levelChunkTrackers = GameObject.FindObjectsOfType<LevelChunkTracker>();
            foreach (var levelChunkTracker in levelChunkTrackers)
            {
                levelChunkTracker.Init();
            }
            this.CurrentTransitionableLevelFXTypeManager = new CurrentTransitionableLevelFXTypeManager(this);
            this.FXTransitionAnimationManager = new FXTransitionAnimationManager();
        }

        #region External Events
        public void OnChunkLevelSwitch(LevelChunkTracker nextLevelChunkTracker)
        {
            this.CurrentTransitionableLevelFXTypeManager.OnChunkLevelSwitch(nextLevelChunkTracker);
        }
        #endregion

        #region Internal Events
        public void OnNewChunkLevel(TransitionableLevelFXType old, TransitionableLevelFXType current)
        {
            this.FXTransitionAnimationManager.OnNewChunkLevel(old, current);
        }
        #endregion

        public void Tick(float d)
        {
            this.FXTransitionAnimationManager.Tick(d);
        }
    }

    class CurrentTransitionableLevelFXTypeManager
    {
        private AdventureLevelChunkFXTransitionManager AdventureLevelChunkFXTransitionManagerRef;

        public CurrentTransitionableLevelFXTypeManager(AdventureLevelChunkFXTransitionManager adventureLevelChunkFXTransitionManagerRef)
        {
            AdventureLevelChunkFXTransitionManagerRef = adventureLevelChunkFXTransitionManagerRef;
        }

        private TransitionableLevelFXType current;
        private TransitionableLevelFXType old;

        public void OnChunkLevelSwitch(LevelChunkTracker nextLevelChunkTracker)
        {
            bool transitionCompleted = false;
            if (old == null)
            {
                this.old = nextLevelChunkTracker.TransitionableLevelFXType;
                transitionCompleted = true;
            }
            else if (current == null)
            {
                if (this.old != nextLevelChunkTracker.TransitionableLevelFXType)
                {
                    this.current = nextLevelChunkTracker.TransitionableLevelFXType;
                    transitionCompleted = true;
                }
            }
            else
            {
                if (this.current != nextLevelChunkTracker.TransitionableLevelFXType)
                {
                    this.old = this.current;
                    this.current = nextLevelChunkTracker.TransitionableLevelFXType;
                    transitionCompleted = true;
                }
            }

            if (transitionCompleted)
            {
                AdventureLevelChunkFXTransitionManagerRef.OnNewChunkLevel(this.old, this.current);
            }
        }

    }

    class FXTransitionAnimationManager
    {

        #region State
        private bool isTransitioning;
        #endregion

        private float elapsedTime;
        private const float MAX_TIME = 1f;

        private TransitionableLevelFXType old;
        private TransitionableLevelFXType current;

        public void OnNewChunkLevel(TransitionableLevelFXType old, TransitionableLevelFXType current)
        {
            this.old = old;
            this.current = current;

            old.PostProcessVolume.gameObject.SetActive(true);
            if (current == null)
            {
                this.isTransitioning = false;
            }
            else
            {
                current.PostProcessVolume.gameObject.SetActive(true);
                this.isTransitioning = true;
                this.ResetState();
            }
        }

        private void ResetState()
        {
            this.elapsedTime = 0f;
        }

        public void Tick(float d)
        {
            if (this.isTransitioning)
            {
                this.elapsedTime += d;
                var completionPercent = this.elapsedTime / MAX_TIME;
                if (completionPercent >= 1)
                {
                    this.isTransitioning = false;
                    this.old.PostProcessVolume.gameObject.SetActive(false);
                    this.current.PostProcessVolume.weight = 1f;
                    this.old.PostProcessVolume.weight = 0f;
                }
                else
                {
                    //the completionPercent - 0.3f is for delaying the old postprecessing transition -> causing artifacts
                    this.old.PostProcessVolume.weight = Mathf.SmoothStep(1, 0, completionPercent - 0.3f);
                    this.current.PostProcessVolume.weight = Mathf.SmoothStep(0, 1, completionPercent);
                }
            }
        }

    }

}

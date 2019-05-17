using UnityEngine;
using System.Collections;
using CoreGame;
using System;
using System.Collections.Generic;

namespace AdventureGame
{
    public class AdventureLevelChunkFXTransitionManager : MonoBehaviour
    {

        private CurrentTransitionableLevelFXTypeManager CurrentTransitionableLevelFXTypeManager;
        private FXTransitionAnimationManager FXTransitionAnimationManager;

        private List<LevelChunkTracker> currentInsideTracker = new List<LevelChunkTracker>();

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
        public void OnChunkLevelEnter(LevelChunkTracker nextLevelChunkTracker)
        {
            this.currentInsideTracker.Add(nextLevelChunkTracker);
            this.CurrentTransitionableLevelFXTypeManager.OnChunkLevelEnter(nextLevelChunkTracker);
        }

        public void OnChunkLevelExit(LevelChunkTracker levelChunkTracker)
        {
            this.currentInsideTracker.Remove(levelChunkTracker);
            if (this.currentInsideTracker.Count == 1)
            {
                //If when there is only one chunk tracker, the current tracker considered is not the last one
                if (!this.CurrentTransitionableLevelFXTypeManager.IsCurrentChunkTrackerEqualsTo(this.currentInsideTracker[0]))
                {
                    //we transition
                    this.CurrentTransitionableLevelFXTypeManager.OnChunkLevelEnter(this.currentInsideTracker[0]);
                }
            }

        }
        #endregion

        #region Internal Events
        public void OnNewChunkLevel(TransitionableLevelFXType old, TransitionableLevelFXType current, ChunkFXTransitionType TransitionType)
        {
            this.FXTransitionAnimationManager.OnNewChunkLevel(old, current, TransitionType);
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

        public void OnChunkLevelEnter(LevelChunkTracker nextLevelChunkTracker)
        {
            if (old == null)
            {
                this.old = nextLevelChunkTracker.TransitionableLevelFXType;
                Debug.Log(MyLog.Format("SAME"));
                AdventureLevelChunkFXTransitionManagerRef.OnNewChunkLevel(this.old, this.current, ChunkFXTransitionType.INSTANT);
            }
            else if (current == null)
            {
                if (this.old != nextLevelChunkTracker.TransitionableLevelFXType)
                {
                    this.current = nextLevelChunkTracker.TransitionableLevelFXType;
                    if (!this.IsNewPostProcessDifferent(nextLevelChunkTracker))
                    {
                        AdventureLevelChunkFXTransitionManagerRef.OnNewChunkLevel(this.old, this.current, ChunkFXTransitionType.INSTANT);
                    }
                    else
                    {
                        AdventureLevelChunkFXTransitionManagerRef.OnNewChunkLevel(this.old, this.current, ChunkFXTransitionType.SMOOTH);
                    }
                }
            }
            else
            {
                if (this.current != nextLevelChunkTracker.TransitionableLevelFXType)
                {
                    this.old = this.current;
                    this.current = nextLevelChunkTracker.TransitionableLevelFXType;
                    if (!this.IsNewPostProcessDifferent(nextLevelChunkTracker))
                    {
                        AdventureLevelChunkFXTransitionManagerRef.OnNewChunkLevel(this.old, this.current, ChunkFXTransitionType.INSTANT);
                    }
                    else
                    {
                        AdventureLevelChunkFXTransitionManagerRef.OnNewChunkLevel(this.old, this.current, ChunkFXTransitionType.SMOOTH);
                    }
                }
            }
        }

        #region Logical conditions
        private bool IsNewPostProcessDifferent(LevelChunkTracker nextLevelChunkTracker)
        {
            return this.old.PostProcessVolume.sharedProfile.name != nextLevelChunkTracker.TransitionableLevelFXType.PostProcessVolume.sharedProfile.name;
        }

        public bool IsCurrentChunkTrackerEqualsTo(LevelChunkTracker compareChunkTracker)
        {
            return this.current != null && this.current == compareChunkTracker;
        }
        #endregion

    }

    class FXTransitionAnimationManager
    {

        #region State
        private bool isTransitioning;
        private float oldPostProcessingStartingWeight = 1f;
        private float currentPostProcessingStartingWeight = 0f;
        #endregion

        private float elapsedTime;
        private const float MAX_TIME = 1f;

        private TransitionableLevelFXType old;
        private TransitionableLevelFXType current;

        public void OnNewChunkLevel(TransitionableLevelFXType old, TransitionableLevelFXType current, ChunkFXTransitionType TransitionType)
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

                if (TransitionType == ChunkFXTransitionType.SMOOTH)
                {
                    if (this.isTransitioning)
                    {
                        this.OnTimeElapsingReverse();
                    }
                    else
                    {
                        this.isTransitioning = true;
                        this.ResetState();
                    }
                }
                else
                {
                    this.OnTransitionEnd();
                }
            }
        }

        private void ResetState()
        {
            this.elapsedTime = 0f;
            this.oldPostProcessingStartingWeight = 1f;
            this.currentPostProcessingStartingWeight = 0f;
        }

        public void Tick(float d)
        {
            if (this.isTransitioning)
            {
                UpdateElapsedTime(d);
                float completionPercent = CalculateCompletionPercent();
                if (IsTransitionFinished(completionPercent))
                {
                    this.OnTransitionEnd();
                }
                else
                {
                    UpdatePostProcessesWeight(completionPercent);
                }
            }
        }

        private bool IsTransitionFinished(float completionPercent)
        {
            return completionPercent >= 1;
        }

        private void UpdateElapsedTime(float d)
        {
            this.elapsedTime += d;
        }

        private float CalculateCompletionPercent()
        {
            return this.elapsedTime / MAX_TIME;
        }

        private void UpdatePostProcessesWeight(float completionPercent)
        {
            //the completionPercent - 0.3f is for delaying the old postprecessing transition -> causing artifacts
            this.old.PostProcessVolume.weight = Mathf.SmoothStep(this.oldPostProcessingStartingWeight, 0, completionPercent - 0.3f);
            this.current.PostProcessVolume.weight = Mathf.SmoothStep(this.currentPostProcessingStartingWeight, 1, completionPercent);
        }

        private void OnTransitionEnd()
        {
            Debug.Log(MyLog.Format("Transition end"));
            this.isTransitioning = false;
            this.old.PostProcessVolume.gameObject.SetActive(false);
            this.current.PostProcessVolume.weight = 1f;
            this.old.PostProcessVolume.weight = 0f;
        }

        private void OnTimeElapsingReverse()
        {
            //we reverse PP weight to have continuity in weight
            this.oldPostProcessingStartingWeight = this.old.PostProcessVolume.weight;
            this.currentPostProcessingStartingWeight = this.current.PostProcessVolume.weight;
            this.elapsedTime = 0f;
        }

    }

    public enum ChunkFXTransitionType
    {
        INSTANT, SMOOTH
    }

}

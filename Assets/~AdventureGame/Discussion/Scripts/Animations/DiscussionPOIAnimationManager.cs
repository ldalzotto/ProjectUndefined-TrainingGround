using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class DiscussionPOIAnimationManager
    {

        #region External Dependencies
        private AdventureDiscussionStaticConfiguration AdventureDiscussionStaticConfiguration;
        #endregion

        #region Internal State
        private Dictionary<PointOfInterestModelObjectModule, DiscussionScaleAnimation> CurrentAnimations;
        #endregion

        public DiscussionPOIAnimationManager()
        {
            this.AdventureDiscussionStaticConfiguration = AdventureGameSingletonInstances.AdventureStaticConfigurationContainer.AdventureStaticConfiguration.AdventureDiscussionStaticConfiguration;
            this.CurrentAnimations = new Dictionary<PointOfInterestModelObjectModule, DiscussionScaleAnimation>();
        }

        public void Tick(float d)
        {
            List<PointOfInterestModelObjectModule> animationsToRemove = null;
            foreach (var currentAnimation in this.CurrentAnimations)
            {
                currentAnimation.Value.Tick(d);
                if (currentAnimation.Value.IsOver)
                {
                    if (animationsToRemove == null) { animationsToRemove = new List<PointOfInterestModelObjectModule>(); }
                    animationsToRemove.Add(currentAnimation.Key);
                }
            }

            if (animationsToRemove != null)
            {
                foreach (var animationToRemove in animationsToRemove)
                {
                    this.CurrentAnimations.Remove(animationToRemove);
                }
            }
        }

        #region External Events
        public void OnAdventureDiscussionTextOnlyStart(PointOfInterestType talkingPointOfInterestType)
        {
            var poiModelObject = talkingPointOfInterestType.GetPointOfInterestModelObject();
            if (poiModelObject != null)
            {
                if (this.CurrentAnimations.ContainsKey(poiModelObject)) { this.CurrentAnimations[poiModelObject].ForceAnimationEnd(); }
                this.CurrentAnimations[poiModelObject] = new DiscussionScaleAnimation(poiModelObject, this.AdventureDiscussionStaticConfiguration);
            }
        }
        #endregion
    }


    class DiscussionScaleAnimation
    {
        private PointOfInterestModelObjectModule PointOfInterestModelObjectModule;
        private AdventureDiscussionStaticConfiguration AdventureDiscussionStaticConfiguration;

        #region Internal State
        private float elapsedTime;
        private bool isOver;
        public bool IsOver { get => isOver; }
        #endregion

        public DiscussionScaleAnimation(PointOfInterestModelObjectModule PointOfInterestModelObjectModule, AdventureDiscussionStaticConfiguration AdventureDiscussionStaticConfiguration)
        {
            this.elapsedTime = 0f;
            this.isOver = false;
            this.PointOfInterestModelObjectModule = PointOfInterestModelObjectModule;
            this.AdventureDiscussionStaticConfiguration = AdventureDiscussionStaticConfiguration;
        }

        public void Tick(float d)
        {
            if (this.PointOfInterestModelObjectModule == null)
            {
                this.EndAnimation();
            }

            if (!this.isOver)
            {
                this.elapsedTime += d;

                var currentScale = this.AdventureDiscussionStaticConfiguration.ModelScaleAnimation.Evaluate(this.elapsedTime / this.AdventureDiscussionStaticConfiguration.ModelScaleAnimationTotalTime);
                this.PointOfInterestModelObjectModule.SetModelLocalScaleRelativeTo(new Vector3(currentScale, currentScale, currentScale), this.PointOfInterestModelObjectModule.AverageModeBounds.Bounds.min);

                if (this.elapsedTime >= this.AdventureDiscussionStaticConfiguration.ModelScaleAnimationTotalTime)
                {
                    this.EndAnimation();
                }
            }
        }

        public void ForceAnimationEnd()
        {
            this.EndAnimation();
        }

        private void EndAnimation()
        {
            this.isOver = true;
            this.PointOfInterestModelObjectModule.SetModelLocalScaleRelativeTo(Vector3.one, Vector3.zero);
        }

    }

}

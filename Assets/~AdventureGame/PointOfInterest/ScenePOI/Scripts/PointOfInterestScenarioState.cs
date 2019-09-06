using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class PointOfInterestScenarioState
    {
        public ReceivableItemsComponent ReceivableItemsComponent;
        public InteractableItemsComponent InteractableItemsComponent;
    }

    [System.Serializable]
    public abstract class POIIdContainer<E> where E : Enum
    {
        public List<E> containedIDs = new List<E>();
        public bool IsElligible(E ID)
        {
            return containedIDs.Contains(ID);
        }
        public void Add(E ID)
        {
            containedIDs.Add(ID);
        }
        public void Remove(E ID)
        {
            containedIDs.Remove(ID);
        }
    }

    #region Receive Items
    [System.Serializable]
    public class ReceivableItemsComponent : POIIdContainer<ItemID> { }
    #endregion

    #region Interactable Items
    [System.Serializable]
    public class InteractableItemsComponent : POIIdContainer<ItemID> { }
    #endregion

    #region Model State
    [System.Serializable]
    public class PointOfInterestModelState
    {
        [SerializeField]
        private bool isDisabled;

        public bool IsDisabled { get => isDisabled; set => isDisabled = value; }

        public void SyncPointOfInterestModelState(Renderer[] scenePOIRenderers)
        {
            for (var i = 0; i < scenePOIRenderers.Length; i++)
            {
                if (scenePOIRenderers[i] != null)
                {
                    scenePOIRenderers[i].enabled = !this.isDisabled;
                }
            }
        }

        #region External Events
        public void OnPOIDisabled()
        {
            this.IsDisabled = true;
        }

        internal void OnPOIEnabled()
        {
            this.isDisabled = false;
        }
        #endregion

    }
    #endregion

    #region Animation Positioning state 
    [System.Serializable]
    public class PointOfInterestAnimationPositioningState
    {

        [SerializeField]
        public AnimationID LastPlayedAnimation = AnimationID.NONE;

        public void SyncPointOfInterestAnimationPositioningState(AnimationID animationID, ref PointOfInterestModelObjectModule pointOfInterestModelObjectModule, AnimationConfiguration animationConfiguration)
        {
            this.LastPlayedAnimation = animationID;
            var animationConfigurationData = animationConfiguration.ConfigurationInherentData[animationID];
            var layerIndex = animationConfigurationData.GetLayerIndex(pointOfInterestModelObjectModule.Animator);
            pointOfInterestModelObjectModule.Animator.Play(animationConfigurationData.AnimationName, layerIndex);
            pointOfInterestModelObjectModule.Animator.Update(0f);
            float currentStateDuration = pointOfInterestModelObjectModule.Animator.GetCurrentAnimatorStateInfo(layerIndex).length;
            pointOfInterestModelObjectModule.Animator.Update(currentStateDuration);
        }

    }
    #endregion

    #region Level Positioning State
    [System.Serializable]
    public class PointOfInterestLevelPositioningState
    {
        [SerializeField]
        public LevelZoneChunkID LevelZoneChunkID;
        [SerializeField]
        public TransformBinarry TransformBinarry;

        public PointOfInterestLevelPositioningState()
        {
            this.LevelZoneChunkID = LevelZoneChunkID.NONE;
        }

        public void SyncPointOfInterestLevelPositioning(LevelZoneChunkID levelZoneChunkID, ref PointOfInterestModelObjectModule pointOfInterestModelObjectModule)
        {
            this.LevelZoneChunkID = levelZoneChunkID;
            this.TransformBinarry = new TransformBinarry(pointOfInterestModelObjectModule.transform);
        }

    }
    #endregion

    #region Identification State
    [System.Serializable]
    public class PointOfInterestIdentificationState
    {
        [SerializeField]
        public PointOfInterestDefinitionID PointOfInterestDefinitionID;
    }
    #endregion
}
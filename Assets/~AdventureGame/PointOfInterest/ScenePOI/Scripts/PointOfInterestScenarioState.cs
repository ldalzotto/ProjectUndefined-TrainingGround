using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
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
                scenePOIRenderers[i].enabled = !this.isDisabled;
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
}
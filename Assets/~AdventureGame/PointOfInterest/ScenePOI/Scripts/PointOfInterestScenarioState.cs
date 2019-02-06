using System;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestScenarioState
{
    public ReceivableItemsComponent ReceivableItemsComponent;
    public DiscussionTree DiscussionTree;
    public InteractableItemsComponent InteractableItemsComponent;
}


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
public class PointOfInterestModelState
{
    private bool isDestroyed;
    private Dictionary<string, bool> MeshRendererStates = new Dictionary<string, bool>();

    public bool IsDestroyed { get => isDestroyed; set => isDestroyed = value; }

    public PointOfInterestModelState(Renderer[] renderers)
    {
        SynchGhostPOIRenderers(renderers);
    }

    public void SyncScenePOIRenderers(Renderer[] scenePOIRenderers)
    {
        for (var i = 0; i < scenePOIRenderers.Length; i++)
        {
            scenePOIRenderers[i].enabled = MeshRendererStates[scenePOIRenderers[i].name];
        }
    }

    public void SynchGhostPOIRenderers(Renderer[] scenePOIRenderers)
    {
        MeshRendererStates = new Dictionary<string, bool>();
        for (var i = 0; i < scenePOIRenderers.Length; i++)
        {
            MeshRendererStates[scenePOIRenderers[i].name] = scenePOIRenderers[i].enabled;
        }
    }
}
#endregion
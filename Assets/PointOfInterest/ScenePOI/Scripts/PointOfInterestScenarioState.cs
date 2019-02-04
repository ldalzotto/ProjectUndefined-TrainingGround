using System;
using System.Collections.Generic;

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
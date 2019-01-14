public interface ScenarioAction { }

public class GrabScenarioAction : ScenarioAction
{
    private ItemID itemInvolved;
    private PointOfInterestId poiInvolved;

    public GrabScenarioAction(ItemID itemInvolved, PointOfInterestId poiInvolved)
    {
        this.itemInvolved = itemInvolved;
        this.poiInvolved = poiInvolved;
    }

    public ItemID ItemInvolved { get => itemInvolved; }
    public PointOfInterestId PoiInvolved { get => poiInvolved; }

    public override bool Equals(object obj)
    {
        var action = obj as GrabScenarioAction;
        return action != null &&
               itemInvolved == action.itemInvolved &&
               poiInvolved == action.poiInvolved;
    }

    public override int GetHashCode()
    {
        var hashCode = 1300380373;
        hashCode = hashCode * -1521134295 + itemInvolved.GetHashCode();
        hashCode = hashCode * -1521134295 + poiInvolved.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return "GrabScenarioAction. " + " Item involved : " + itemInvolved.ToString() + ", POIInvolved : " + poiInvolved.ToString();
    }
}

public class GiveScenarioAction : ScenarioAction
{
    private ItemID itemInvolved;
    private PointOfInterestId poiInvolved;

    public GiveScenarioAction(ItemID itemInvolved, PointOfInterestId poiInvolved)
    {
        this.itemInvolved = itemInvolved;
        this.poiInvolved = poiInvolved;
    }

    public ItemID ItemInvolved { get => itemInvolved; }
    public PointOfInterestId PoiInvolved { get => poiInvolved; }

    public override bool Equals(object obj)
    {
        var action = obj as GiveScenarioAction;
        return action != null &&
               itemInvolved == action.itemInvolved &&
               poiInvolved == action.poiInvolved;
    }

    public override int GetHashCode()
    {
        var hashCode = 1300380373;
        hashCode = hashCode * -1521134295 + itemInvolved.GetHashCode();
        hashCode = hashCode * -1521134295 + poiInvolved.GetHashCode();
        return hashCode;
    }


    public override string ToString()
    {
        return "GiveScenarioAction. " + " Item involved : " + itemInvolved.ToString() + ", POIInvolved : " + poiInvolved.ToString();
    }
}
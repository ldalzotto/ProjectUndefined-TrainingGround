using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public interface ScenarioAction { }

public class GrabScenarioAction : ScenarioAction
{
    [JsonConverter(typeof(StringEnumConverter))]
    public ItemID ItemInvolved { get; }
    [JsonConverter(typeof(StringEnumConverter))]
    public PointOfInterestId PoiInvolved { get; }

    public GrabScenarioAction(ItemID itemInvolved, PointOfInterestId poiInvolved)
    {
        this.ItemInvolved = itemInvolved;
        this.PoiInvolved = poiInvolved;
    }

    public override bool Equals(object obj)
    {
        var action = obj as GrabScenarioAction;
        return action != null &&
               ItemInvolved == action.ItemInvolved &&
               PoiInvolved == action.PoiInvolved;
    }

    public override int GetHashCode()
    {
        var hashCode = 1300380373;
        hashCode = hashCode * -1521134295 + ItemInvolved.GetHashCode();
        hashCode = hashCode * -1521134295 + PoiInvolved.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return "GrabScenarioAction. " + " Item involved : " + ItemInvolved.ToString() + ", POIInvolved : " + PoiInvolved.ToString();
    }
}

public class GiveScenarioAction : ScenarioAction
{
    [JsonConverter(typeof(StringEnumConverter))]
    public ItemID ItemInvolved { get; }
    [JsonConverter(typeof(StringEnumConverter))]
    public PointOfInterestId PoiInvolved { get; }

    public GiveScenarioAction(ItemID itemInvolved, PointOfInterestId poiInvolved)
    {
        ItemInvolved = itemInvolved;
        this.PoiInvolved = poiInvolved;
    }

    public override bool Equals(object obj)
    {
        var action = obj as GiveScenarioAction;
        return action != null &&
               ItemInvolved == action.ItemInvolved &&
               PoiInvolved == action.PoiInvolved;
    }

    public override int GetHashCode()
    {
        var hashCode = 1300380373;
        hashCode = hashCode * -1521134295 + ItemInvolved.GetHashCode();
        hashCode = hashCode * -1521134295 + PoiInvolved.GetHashCode();
        return hashCode;
    }


    public override string ToString()
    {
        return "GiveScenarioAction. " + " Item involved : " + ItemInvolved.ToString() + ", POIInvolved : " + PoiInvolved.ToString();
    }
}

public class DiscussionChoiceScenarioAction : ScenarioAction
{
    [JsonConverter(typeof(StringEnumConverter))]
    public DiscussionChoiceTextId ChoiceId { get; }

    public DiscussionChoiceScenarioAction(DiscussionChoiceTextId choiceId)
    {
        this.ChoiceId = choiceId;
    }

    public override bool Equals(object obj)
    {
        var action = obj as DiscussionChoiceScenarioAction;
        return action != null &&
               ChoiceId == action.ChoiceId;
    }

    public override int GetHashCode()
    {
        return -1877750589 + ChoiceId.GetHashCode();
    }

    public override string ToString()
    {
        return "DiscussionChoiceScenarioAction. " + " Choice made : " + ChoiceId.ToString();
    }

}
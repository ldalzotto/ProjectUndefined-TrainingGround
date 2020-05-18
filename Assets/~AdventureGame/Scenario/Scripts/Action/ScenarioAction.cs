﻿
using CoreGame;

namespace AdventureGame
{

    public class GrabScenarioAction : TimeLineAction
    {
        public ItemID ItemInvolved { get; }
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

        public void NodeGUI()
        {
           
        }
    }

    public class GiveScenarioAction : TimeLineAction
    {
        public ItemID ItemInvolved { get; }
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

        public void NodeGUI()
        {
          
        }
    }

    public class DiscussionChoiceScenarioAction : TimeLineAction
    {
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

        public void NodeGUI()
        {
         
        }
    }

    public class CutsceneTimelineScenarioAction : TimeLineAction
    {
        public CutsceneId CutsceneId;
        public PointOfInterestId TargetedPOI;

        public CutsceneTimelineScenarioAction(CutsceneId cutsceneId, PointOfInterestId targetedPOI)
        {
            CutsceneId = cutsceneId;
            TargetedPOI = targetedPOI;
        }

        public override bool Equals(object obj)
        {
            var action = obj as CutsceneTimelineScenarioAction;
            return action != null &&
                   CutsceneId == action.CutsceneId &&
                   TargetedPOI == action.TargetedPOI;
        }

        public override int GetHashCode()
        {
            var hashCode = -827486109;
            hashCode = hashCode * -1521134295 + CutsceneId.GetHashCode();
            hashCode = hashCode * -1521134295 + TargetedPOI.GetHashCode();
            return hashCode;
        }

        public void NodeGUI()
        {
           
        }

        public override string ToString()
        {
            return "CutsceneTimelineScenarioAction. " + " Cutscene executed : " + CutsceneId.ToString() + " to POI : " + TargetedPOI.ToString();
        }
    }

}

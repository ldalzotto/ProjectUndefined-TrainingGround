
using CoreGame;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace AdventureGame
{

    [System.Serializable]
    public class GrabScenarioAction : TimeLineAction
    {
        public ItemID ItemInvolved { get => itemInvolved; }
        public PointOfInterestId PoiInvolved { get => poiInvolved; }

        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private PointOfInterestId poiInvolved;

        public GrabScenarioAction(ItemID itemInvolved, PointOfInterestId poiInvolved)
        {
            this.itemInvolved = itemInvolved;
            this.poiInvolved = poiInvolved;
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

#if UNITY_EDITOR
        public void NodeGUI()
        {
            this.itemInvolved = (ItemID)EditorGUILayout.EnumPopup("ITEM : ", this.itemInvolved);
            this.poiInvolved = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ", this.poiInvolved);
        }
#endif
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
        public CutsceneId CutsceneId { get => cutsceneId; }
        public PointOfInterestId TargetedPOI { get => targetedPOI; }

        [SerializeField]
        private CutsceneId cutsceneId;
        [SerializeField]
        private PointOfInterestId targetedPOI;

        public CutsceneTimelineScenarioAction(CutsceneId cutsceneId, PointOfInterestId targetedPOI)
        {
            this.cutsceneId = cutsceneId;
            this.targetedPOI = targetedPOI;
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

#if UNITY_EDITOR
        public void NodeGUI()
        {
            this.cutsceneId = (CutsceneId)EditorGUILayout.EnumPopup("Cutscene : ", this.cutsceneId);
            this.targetedPOI = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ", this.targetedPOI);
        }
#endif

        public override string ToString()
        {
            return "CutsceneTimelineScenarioAction. " + " Cutscene executed : " + CutsceneId.ToString() + " to POI : " + TargetedPOI.ToString();
        }
    }

}

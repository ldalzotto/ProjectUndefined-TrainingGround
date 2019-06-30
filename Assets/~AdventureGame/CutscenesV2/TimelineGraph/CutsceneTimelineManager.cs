using CoreGame;
using GameConfigurationID;

namespace AdventureGame
{
    public class CutsceneTimelineManager : TimelineNodeManagerV2<CutsceneTimelinePassedData, int>
    {
        protected override CutsceneTimelinePassedData workflowActionPassedDataStruct => throw new System.NotImplementedException();

        protected override TimelineID TimelineID => TimelineID.CUTSCENE_TIMELINE;

        protected override bool isPersisted => false;
    }

}

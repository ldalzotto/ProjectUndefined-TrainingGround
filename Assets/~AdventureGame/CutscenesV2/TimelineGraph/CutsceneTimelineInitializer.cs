using CoreGame;
using GameConfigurationID;

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneTimelineInitializer : TimelineInitializerV2<CutsceneTimelinePassedData, int>
    {

    }

    public class CutsceneTimelinePassedData
    {
        public CutsceneId cutsceneId;
    }

}

using UnityEngine;
using UnityEngine.Playables;

public class WaitEndOfCutscene : CustomYieldInstruction
{

    private PlayableDirector playableDirector;

    public WaitEndOfCutscene(PlayableDirector playableDirector)
    {
        this.playableDirector = playableDirector;
    }

    public override bool keepWaiting
    {
        get
        {
          //  Debug.Log(MyLog.Format("Time : " + playableDirector.time + ", duration : " + playableDirector.duration));
            return playableDirector.time < playableDirector.duration;
        }
    }
}

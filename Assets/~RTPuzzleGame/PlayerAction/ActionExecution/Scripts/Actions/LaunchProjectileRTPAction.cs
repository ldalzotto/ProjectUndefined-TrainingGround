using System.Collections;
using UnityEngine;

public class LaunchProjectileRTPAction : RTPPlayerAction
{
    public override SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId => SelectionWheelNodeConfigurationId.THROW_PLAYER_PUZZLE_WHEEL_CONFIG;

    private bool isActionFinished = false;

    public override bool FinishedCondition()
    {
        return isActionFinished;
    }

    public override void FirstExecution()
    {
        isActionFinished = false;
        Coroutiner.Instance.StartCoroutine(Dum());
    }

    public override void Tick(float d)
    {
    }

    public override void GizmoTick()
    {

    }

    private IEnumerator Dum()
    {
        yield return new WaitForSeconds(1f);
        isActionFinished = true;
    }

}

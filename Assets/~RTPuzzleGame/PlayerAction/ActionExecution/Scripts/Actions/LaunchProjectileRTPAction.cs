using UnityEngine;

public class LaunchProjectileRTPAction : RTPPlayerAction
{
    public override bool FinishedCondition()
    {
        return true;
    }

    public override void FirstExecution()
    {
        Debug.Log("Dummy LaunchProjectileAction start.");
    }

    public override void Tick(float d)
    {
        Debug.Log("Dummy LaunchProjectileAction tick.");
    }
}

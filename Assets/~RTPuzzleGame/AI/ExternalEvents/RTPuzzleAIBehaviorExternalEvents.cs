public class OnLaunchProjectileDestroyed : AIBehaviorAbstractExternalEvent
{

    private LaunchProjectile launchProjectile;

    public OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile)
    {
        this.launchProjectile = launchProjectile;
    }

    public override AIBehaviorAbstractExternalEventData GetEventData()
    {
        return launchProjectile;
    }
}
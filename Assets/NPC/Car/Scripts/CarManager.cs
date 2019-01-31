using UnityEngine;

public class CarManager : MonoBehaviour
{
    private const string CarModelName = "CarModel";
    private const string CarHeadPhysicsName = "CarHead_Physics";

    public WayPointLinearFollowManagerComponent WayPointLinearFollowManagerComponent;
    public WaypointPathId WayPointIdToFollow;

    private LinearWayPointFollow LinearWayPointFollow;
    private CarPositionManager CarPositionManager;

    private void Start()
    {
        #region External Dependencies
        var wayPointCOntainer = GameObject.FindObjectOfType<WayPointPathContainer>();
        #endregion

        GameObject.FindObjectOfType<NPCManager>().AddCar(this);
        var carModelObject = gameObject.FindChildObjectRecursively(CarModelName);
        var carHeadPhysics = gameObject.FindChildObjectRecursively(CarHeadPhysicsName);

        LinearWayPointFollow = new LinearWayPointFollow(WayPointLinearFollowManagerComponent, carHeadPhysics.GetComponent<Rigidbody>(), wayPointCOntainer.GetWayPointPath(WayPointIdToFollow));
        CarPositionManager = new CarPositionManager(carModelObject.transform, carHeadPhysics.transform);
    }

    public void Tick(float d)
    {
        LinearWayPointFollow.Tick(d);
        CarPositionManager.Tick(d);
    }

    public void FixedTick(float d)
    {
        LinearWayPointFollow.FixedTick(d);
    }
}

class CarPositionManager
{
    public Transform source;
    public Transform target;

    public CarPositionManager(Transform source, Transform target)
    {
        this.source = source;
        this.target = target;
    }

    public void Tick(float d)
    {
        Vector3 targetDir = target.position - source.position;

        // The step size is equal to speed times frame time.
        float step = 100 * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(source.forward, targetDir, step, 0.0f);
        Debug.DrawRay(source.position, newDir, Color.red);

        // Move our position a step closer to the target.
        source.rotation = Quaternion.LookRotation(newDir);
    }
}
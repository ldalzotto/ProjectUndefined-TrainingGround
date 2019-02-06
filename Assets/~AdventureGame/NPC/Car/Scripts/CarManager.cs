using System.Collections;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    private const string CarModelName = "CarModel";
    private const string CarHeadPhysicsName = "CarHead_Physics";

    #region External Dependnceis
    private CarSpawnerManager CarSpawnerManager;
    #endregion

    public WayPointLinearFollowManagerComponent WayPointLinearFollowManagerComponent;
    public WaypointPathId WayPointIdToFollow;

    private LinearWayPointFollow LinearWayPointFollow;
    private CarPositionManager CarPositionManager;

    public void PrefabInit(WaypointPathId WayPointIdToFollow)
    {
        this.WayPointIdToFollow = WayPointIdToFollow;

        #region External Dependencies
        var wayPointCOntainer = GameObject.FindObjectOfType<WayPointPathContainer>();
        CarSpawnerManager = GameObject.FindObjectOfType<CarSpawnerManager>();
        #endregion

        var carModelObject = gameObject.FindChildObjectRecursively(CarModelName);
        var carHeadPhysics = gameObject.FindChildObjectRecursively(CarHeadPhysicsName);

        LinearWayPointFollow = new LinearWayPointFollow(WayPointLinearFollowManagerComponent, carHeadPhysics.GetComponent<Rigidbody>(), wayPointCOntainer.GetWayPointPath(WayPointIdToFollow), OnWayPointPathEnd);
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

    #region Internal Events
    private void OnWayPointPathEnd()
    {
        StartCoroutine(DestroyCarAtEndOfFrame());
    }
    #endregion

    private IEnumerator DestroyCarAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        CarSpawnerManager.DestroyCar(this);
        CarSpawnerManager.Spawn();
        Destroy(gameObject);
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
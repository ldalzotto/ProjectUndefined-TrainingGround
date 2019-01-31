using UnityEngine;

public class LinearWayPointFollow
{

    public LinearWayPointFollow(WayPointLinearFollowManagerComponent wayPointLinearFollowManagerComponent, Rigidbody bodyToMove, WayPointPath pathToFollow)
    {
        WayPointLinearFollowManager = new WayPointLinearFollowManager(pathToFollow, bodyToMove, wayPointLinearFollowManagerComponent);
        OnWaypointFollowStart();
    }

    #region Internal Dependencies
    private WayPointLinearFollowManager WayPointLinearFollowManager;
    #endregion

    #region External Events
    public void OnWaypointFollowStart()
    {
        WayPointLinearFollowManager.OnStart();
    }
    #endregion

    public void Tick(float d)
    {
        WayPointLinearFollowManager.Tick(d);
    }

    public void FixedTick(float d)
    {
        WayPointLinearFollowManager.FixedTick(d);
    }

}

[System.Serializable]
public class WayPointLinearFollowManagerComponent
{
    public float Speed;
}

class WayPointLinearFollowManager
{
    private WayPointPath WayPointPath;
    private Rigidbody bodyToMove;
    private WayPointLinearFollowManagerComponent WayPointLinearFollowManagerComponent;

    public WayPointLinearFollowManager(WayPointPath wayPointPath, Rigidbody bodyToMove, WayPointLinearFollowManagerComponent WayPointLinearFollowManagerComponent)
    {
        WayPointPath = wayPointPath;
        this.bodyToMove = bodyToMove;
        this.WayPointLinearFollowManagerComponent = WayPointLinearFollowManagerComponent;
    }

    private WayPoint activeWayPoint;

    public void Tick(float d)
    {
        if (Vector3.Distance(bodyToMove.position, activeWayPoint.transform.position) <= 0.0001)
        {
            bodyToMove.position = activeWayPoint.transform.position;
            OnWayPointChange();
        }
    }

    public void FixedTick(float d)
    {
        var nextPosition = Vector3.MoveTowards(bodyToMove.position, activeWayPoint.transform.position, WayPointLinearFollowManagerComponent.Speed * d);
        bodyToMove.MovePosition(nextPosition);
    }

    public void OnStart()
    {
        this.activeWayPoint = WayPointPath.WayPointsToFollow[0];
        bodyToMove.position = (this.activeWayPoint.transform.position);
        this.OnWayPointChange();
    }

    private void OnWayPointChange()
    {
        var oldWayPoint = this.activeWayPoint;
        if (WayPointPath.WayPointsToFollow.Count > WayPointPath.WayPointsToFollow.IndexOf(this.activeWayPoint) + 1)
        {
            this.activeWayPoint = WayPointPath.WayPointsToFollow[WayPointPath.WayPointsToFollow.IndexOf(this.activeWayPoint) + 1];
        }
        else
        {
            if (WayPointPath.Loop)
            {
                this.activeWayPoint = WayPointPath.WayPointsToFollow[0];
            }
        }

    }
}
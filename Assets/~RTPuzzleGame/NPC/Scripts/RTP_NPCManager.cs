using UnityEngine;
using UnityEngine.AI;

public class RTP_NPCManager : MonoBehaviour
{

    #region External Dependencies
    private RTPlayerManagerDataRetriever RTPlayerManagerDataRetriever;
    #endregion

    #region AI Behavior Components
    [Header("AI Behavior Components")]
    public AIRandomPatrolComponent AIRandomPatrolComponent;
    #endregion

    public AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;

    private AIDestinationMoveManager AIDestinationMoveManager;
    private NPCSpeedAdjusterManager NPCSpeedAdjusterManager;
    private RTPuzzleAIBehavior RTPuzzleAIBehavior;

    public void Init()
    {
        #region External Dependencies
        RTPlayerManagerDataRetriever = GameObject.FindObjectOfType<RTPlayerManagerDataRetriever>();
        #endregion

        var agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        AIDestinationMoveManager = new AIDestinationMoveManager(AIDestimationMoveManagerComponent, agent, transform);
        NPCSpeedAdjusterManager = new NPCSpeedAdjusterManager(agent);
        RTPuzzleAIBehavior = new MouseAIBehavior(agent, AIRandomPatrolComponent);
    }

    public void Tick(float d)
    {
        RTPuzzleAIBehavior.Tick();
        if (RTPuzzleAIBehavior.NewDestinationDefined)
        {
            SetDestination(RTPuzzleAIBehavior.NewDestination);
        }

        AIDestinationMoveManager.Tick(d);
        NPCSpeedAdjusterManager.Tick(d, RTPlayerManagerDataRetriever.GetPlayerSpeedMagnitude());
    }

    public void GizmoTick()
    {
        RTPuzzleAIBehavior.TickGizmo();
    }

    #region External Events
    private void SetDestination(Vector3 destination)
    {
        AIDestinationMoveManager.SetDestination(destination);
    }
    public void EnableAgent()
    {
        AIDestinationMoveManager.EnableAgent();
    }
    public void DisableAgent()
    {
        AIDestinationMoveManager.DisableAgent();
    }
    #endregion

}

class NPCSpeedAdjusterManager
{
    NavMeshAgent agent;

    public NPCSpeedAdjusterManager(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public void Tick(float d, float playerSpeedMagnitude)
    {
        this.agent.speed = this.agent.speed * playerSpeedMagnitude;
    }
}
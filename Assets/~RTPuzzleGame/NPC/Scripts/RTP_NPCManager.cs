using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RTP_NPCManager : MonoBehaviour
{

    #region External Dependencies
    private RTPlayerManagerDataRetriever RTPlayerManagerDataRetriever;
    #endregion

    #region Internal Dependencies
    private NavMeshAgent agent;
    #endregion

    #region AI Behavior Components
    [Header("AI Behavior Components")]
    public AIRandomPatrolComponent AIRandomPatrolComponent;
    public AIProjectileEscapeComponent AIProjectileEscapeComponent;
    #endregion

    public AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;

    private AIDestinationMoveManager AIDestinationMoveManager;
    private NPCSpeedAdjusterManager NPCSpeedAdjusterManager;
    private RTPuzzleAIBehavior RTPuzzleAIBehavior;

    private Coroutine destinatioNReachedCoroutine;

    public void Init()
    {
        #region External Dependencies
        RTPlayerManagerDataRetriever = GameObject.FindObjectOfType<RTPlayerManagerDataRetriever>();
        #endregion

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        AIDestinationMoveManager = new AIDestinationMoveManager(AIDestimationMoveManagerComponent, agent, transform);
        NPCSpeedAdjusterManager = new NPCSpeedAdjusterManager(agent);
        RTPuzzleAIBehavior = new MouseAIBehavior(agent, AIRandomPatrolComponent, AIProjectileEscapeComponent);
    }

    public void Tick(float d)
    {
        var newDestination = RTPuzzleAIBehavior.TickAI();
        if (newDestination.HasValue)
        {
            SetDestinationWithCoroutineReached(newDestination.Value);
        }

        AIDestinationMoveManager.Tick(d);
        NPCSpeedAdjusterManager.Tick(d, RTPlayerManagerDataRetriever.GetPlayerSpeedMagnitude());
    }

    private void OnTriggerEnter(Collider other)
    {
        RTPuzzleAIBehavior.OnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        RTPuzzleAIBehavior.OnTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        RTPuzzleAIBehavior.OnTriggerExit(other);
    }

    public void GizmoTick()
    {
        RTPuzzleAIBehavior.TickGizmo();
    }

    #region External Events
    private void SetDestinationWithCoroutineReached(Vector3 destination)
    {
        AIDestinationMoveManager.SetDestination(destination);

        if (destinatioNReachedCoroutine != null)
        {
            StopCoroutine(destinatioNReachedCoroutine);
        }

        destinatioNReachedCoroutine = StartCoroutine(OnDestinationReached());
    }
    private void SetDestinationSilent(Vector3 destination)
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

    #region Internal Events
    private IEnumerator OnDestinationReached()
    {
        yield return new WaitForNavAgentDestinationReached(agent);
        RTPuzzleAIBehavior.OnDestinationReached();
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
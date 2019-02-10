using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RTP_NPCManager : MonoBehaviour
{

    [Header("Debug")]
    public bool DebugEabled;

    #region Internal Dependencies
    private NavMeshAgent agent;
    #endregion

    #region AI Behavior Components
    [Header("AI Behavior Components")]
    public AiID AiID;
    #endregion

    public AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;

    private AIDestinationMoveManager AIDestinationMoveManager;
    private NPCSpeedAdjusterManager NPCSpeedAdjusterManager;
    private RTPuzzleAIBehavior RTPuzzleAIBehavior;

    private Coroutine destinatioNReachedCoroutine;

    public void Init()
    {

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        var aiComponent = GameObject.FindObjectOfType<AIComponentsManager>().Get(AiID);

        AIDestinationMoveManager = new AIDestinationMoveManager(AIDestimationMoveManagerComponent, agent, transform);
        NPCSpeedAdjusterManager = new NPCSpeedAdjusterManager(agent);
        RTPuzzleAIBehavior = new MouseAIBehavior(agent, aiComponent.AIRandomPatrolComponent, aiComponent.AIProjectileEscapeComponent);
    }

    public void Tick(float d, float timeAttenuationFactor)
    {
        var newDestination = RTPuzzleAIBehavior.TickAI();
        if (newDestination.HasValue)
        {
            SetDestinationWithCoroutineReached(newDestination.Value);
        }

        AIDestinationMoveManager.Tick(d);
        NPCSpeedAdjusterManager.Tick(d, timeAttenuationFactor);
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

    public void GUITick()
    {
        if (DebugEabled)
        {
            var mouseAIBehavior = RTPuzzleAIBehavior as MouseAIBehavior;
            GUILayout.BeginVertical("box");
            GUILayout.Label("Position : " + transform.position.ToString());
            mouseAIBehavior.DebugGUITick();
            GUILayout.EndVertical();
        }
    }

    #region External Events
    private void SetDestinationWithCoroutineReached(Vector3 destination)
    {
        Debug.Log("Set Destination");
        AIDestinationMoveManager.SetDestination(destination);

        if (destinatioNReachedCoroutine != null)
        {
            StopCoroutine(destinatioNReachedCoroutine);
        }

        destinatioNReachedCoroutine = Coroutiner.Instance.StartCoroutine(OnDestinationReached());
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
    public void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile)
    {
        RTPuzzleAIBehavior.OnExternalEvent(new OnLaunchProjectileDestroyed(launchProjectile));
    }
    #endregion

    #region Internal Events
    private IEnumerator OnDestinationReached()
    {
        yield return new WaitForNavAgentDestinationReached(agent);
        Debug.Log("Destination Reached");
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
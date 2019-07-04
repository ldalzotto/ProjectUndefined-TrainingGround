using UnityEngine;

public class IntersectionTestGameManager : MonoBehaviour
{

    private GroundObstacleRendererManager GroundObstacleRendererManager;
    private SquareObstaclesManager SquareObstaclesManager;
    private ObstaclesListenerManager ObstaclesListenerManager;
    private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;

    void Start()
    {
        GroundObstacleRendererManager = GameObject.FindObjectOfType<GroundObstacleRendererManager>();
        this.ObstaclesListenerManager = GameObject.FindObjectOfType<ObstaclesListenerManager>();
        this.SquareObstaclesManager = GameObject.FindObjectOfType<SquareObstaclesManager>();
        this.ObstacleFrustumCalculationManager = GameObject.FindObjectOfType<ObstacleFrustumCalculationManager>();


        this.ObstacleFrustumCalculationManager.Init();
        this.GroundObstacleRendererManager.Init();
        this.ObstaclesListenerManager.Init();
        this.SquareObstaclesManager.Init();
    }

    void Update()
    {
        float d = Time.deltaTime;

        ObstaclesListenerManager.Tick(d);
        this.ObstacleFrustumCalculationManager.Tick(d);
        GroundObstacleRendererManager.Tick(d);
    }
}

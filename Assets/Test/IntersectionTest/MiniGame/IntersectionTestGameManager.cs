﻿using UnityEngine;

public class IntersectionTestGameManager : MonoBehaviour
{

    private GroundObstacleRendererManager GroundObstacleRendererManager;

    void Start()
    {
        GroundObstacleRendererManager = GameObject.FindObjectOfType<GroundObstacleRendererManager>();

        GroundObstacleRendererManager.Init();

        var testSpheres = GameObject.FindObjectsOfType<Test_Sphere>();
        if (testSpheres != null)
        {
            foreach (var testSphere in testSpheres)
            {
                testSphere.Init();
            }
        }
    }

    void Update()
    {
        float d = Time.deltaTime;
        GroundObstacleRendererManager.Tick(d);
    }
}

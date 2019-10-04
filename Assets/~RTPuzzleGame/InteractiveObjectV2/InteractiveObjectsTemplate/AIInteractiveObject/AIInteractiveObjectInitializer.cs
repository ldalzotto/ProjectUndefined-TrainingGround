using UnityEngine;
using System.Collections;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class AIInteractiveObjectInitializerData
    {
        public float SpeedMultiplicationFactor;
        public float RotationSpeed;
        public float MinAngleThatAllowThePositionUpdate;

        public bool HasSight;
        public SightObjectSystemDefinition SightObjectSystemDefinition;
    }

    [System.Serializable]
    public class AIInteractiveObjectInitializer : AInteractiveObjectInitializer
    {
        public AIInteractiveObjectInitializerData AIInteractiveObjectInitializerData;

        public override void Init()
        {
            InteractiveObjectV2Manager.Get().OnInteractiveObjectCreated(new AIInteractiveObject(new InteractiveGameObject(this.gameObject), AIInteractiveObjectInitializerData));
        }
    }

}

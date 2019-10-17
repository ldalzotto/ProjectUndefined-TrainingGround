using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjectTest
{

    public struct AIDestination
    {
        public Vector3 WorldPosition;
        public Quaternion? Rotation;
    }


    public static class AIMovementDefinitions
    {
        public enum AIMovementSpeedDefinition
        {
            RUN = 0,
            WALK = 1,
            ZERO = 2
        }

        public static Dictionary<AIMovementSpeedDefinition, float> AIMovementSpeedAttenuationFactorLookup = new Dictionary<AIMovementSpeedDefinition, float>()
    {
        { AIMovementSpeedDefinition.ZERO, 0f },
        { AIMovementSpeedDefinition.WALK, 0.5f },
        { AIMovementSpeedDefinition.RUN, 1f }
    };
    }
    
    public struct AnimationObjectSetUnscaledSpeedMagnitudeEvent
    {
        public float UnscaledSpeedMagnitude;
    }



    [System.Serializable]
    [SceneHandleDraw]
    public class AIAgentDefinition
    {
        [WireCircle(R = 0f, G = 1f, B = 0f)]
        public float AgentStoppingDistance = 0.5f;
        [WireDirectionalLineAttribute(R = 0f, G = 1f, B = 0f, dY = 1f)]
        public float AgentHeight = 2f;
    }
}
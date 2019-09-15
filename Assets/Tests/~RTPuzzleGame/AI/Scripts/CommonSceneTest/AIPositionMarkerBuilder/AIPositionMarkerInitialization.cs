using GameConfigurationID;
using RTPuzzle;
using UnityEngine;

namespace Tests
{
    public class AIPositionMarkerInitialization
    {
        public Vector3 WorldPosition;
        public Quaternion WorldRotation;
        public AIPositionMarkerID AIPositionMarkerID;

        public AIPositionMarkerInitialization(Vector3 worldPosition, Quaternion worldRotation, AIPositionMarkerID aIPositionMarkerID)
        {
            WorldPosition = worldPosition;
            WorldRotation = worldRotation;
            AIPositionMarkerID = aIPositionMarkerID;
        }

        public AIPositionMarker InstanciateInScene()
        {
            var aiPositionMarkerObject = new GameObject();
            var AIPositionMarker = aiPositionMarkerObject.AddComponent<AIPositionMarker>();
            AIPositionMarker.PositionMarkerID = this.AIPositionMarkerID;
            AIPositionMarker.transform.position = this.WorldPosition;
            AIPositionMarker.transform.rotation = this.WorldRotation;
            return AIPositionMarker;
        }
       
    }

}

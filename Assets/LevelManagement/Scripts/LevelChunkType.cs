using UnityEngine;

namespace LevelManagement
{
    public class LevelChunkType : MonoBehaviour
    {
        public LevelZoneChunkID LevelZoneChunkID;

        public static void DestroyAllDestroyOnStartObjects()
        {
            foreach (var objectToDestroy in GameObject.FindGameObjectsWithTag(TagConstants.TO_DESTROY_ON_START)) Destroy(objectToDestroy);
        }

        private void Start()
        {
            GetComponent<LevelChunkTracker>().Init();
        }

        private void Update()
        {
            var d = Time.deltaTime;
        }
    }
}
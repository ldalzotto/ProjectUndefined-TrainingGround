using UnityEngine;

namespace CoreGame
{
    public class CircleFillBarType : MonoBehaviour
    {
        private Camera cam;
        private MeshFilter meshFilter;
        private float currentProgression;

        public MeshFilter MeshFilter { get => meshFilter; }
        public float CurrentProgression { get => currentProgression; }

        private CircleFillBarRendererManager CircleFillBarRendererManager;

        public void Init(Camera camera)
        {
            this.cam = camera;
            this.meshFilter = GetComponent<MeshFilter>();
            this.CircleFillBarRendererManager = GameObject.FindObjectOfType<CircleFillBarRendererManager>();
            this.CircleFillBarRendererManager.OnCircleFillBarTypeCreated(this);
        }

        public void Tick(float progression)
        {
            this.currentProgression = progression;
            this.transform.LookAt(this.cam.transform);
        }

        public void OnCircleFillBarTypeEnabled()
        {
            this.CircleFillBarRendererManager.OnCircleFillBarTypeCreated(this);
        }

        private void OnDisable()
        {
            if (this.CircleFillBarRendererManager != null)
            {
                this.CircleFillBarRendererManager.OnCircleFillBarTypeDestroyed(this);
            }
        }

        public static void EnableInstace(CircleFillBarType CircleFillBarTypeRef)
        {
            CircleFillBarTypeRef.gameObject.SetActive(true);
            CircleFillBarTypeRef.OnCircleFillBarTypeEnabled();
        }

    }
}

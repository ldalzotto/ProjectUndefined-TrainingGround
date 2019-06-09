using UnityEngine;

namespace RTPuzzle
{
    public class DottedLine : MonoBehaviour
    {
        [Header("Model scale")]
        public Vector3 ModelScale;

        private Vector3 LastFrameWorldSpaceStartPoint;
        private Vector3 LastFrameWorldSpaceEndPoint;

        private MeshFilter meshFilter;
        private MeshRenderer MeshRenderer;

        private DottedLineRendererManager DottedLineManager;

        public MeshFilter MeshFilter { get => meshFilter; set => meshFilter = value; }

        public static DottedLine CreateInstance()
        {
            var instanciatedDottedLine = MonoBehaviour.Instantiate(PrefabContainer.Instance.ProjectileDottedLineBasePrefab);
            instanciatedDottedLine.transform.position = Vector3.zero;
            instanciatedDottedLine.transform.localScale = Vector3.one;
            instanciatedDottedLine.transform.rotation = Quaternion.identity;
            instanciatedDottedLine.Init();
            return instanciatedDottedLine;
        }

        public void DestroyInstance()
        {
            if (this != null)
            {
                this.DottedLineManager.OnDottedLineDestroyed(this);
                MonoBehaviour.Destroy(this.gameObject);
            }
        }

        public void Init()
        {
            this.DottedLineManager = GameObject.FindObjectOfType<DottedLineRendererManager>();
            this.MeshFilter = GetComponent<MeshFilter>();
            this.MeshFilter.mesh = new Mesh();
            this.MeshRenderer = GetComponent<MeshRenderer>();
        }

        private BeziersControlPoints BeziersControlPoints;
        private float currentPosition = 0f;


        public void Tick(float d, Vector3 worldSpaceStartPoint, Vector3 worldSpaceEndPoint)
        {
            if ((worldSpaceStartPoint != LastFrameWorldSpaceStartPoint) || (worldSpaceEndPoint != LastFrameWorldSpaceEndPoint))
            {
                if (Vector3.Distance(worldSpaceStartPoint, worldSpaceEndPoint) <= 2.5f)
                {
                    this.ClearLine();
                }
                else
                {
                    this.RePositionLine(worldSpaceStartPoint, worldSpaceEndPoint, Vector3.up);
                }
            }

            this.LastFrameWorldSpaceStartPoint = worldSpaceStartPoint;
            this.LastFrameWorldSpaceEndPoint = worldSpaceEndPoint;

            this.currentPosition += d;
            if (this.currentPosition > 1)
            {
                this.currentPosition = 1 - this.currentPosition;
            }

            this.MeshRenderer.material.SetVector("_ColorPointPosition", this.transform.TransformPoint(this.BeziersControlPoints.ResolvePoint(this.currentPosition)));
        }

        public ComputeBeziersInnerPointEvent BuilComputeBeziersInnerPointEvent()
        {
            return new ComputeBeziersInnerPointEvent(this.GetInstanceID(), this.BeziersControlPoints, this.ModelScale, 0.5f);
        }

        private void RePositionLine(Vector3 worldSpaceStartPoint, Vector3 worldSpaceEndPoint, Vector3 normal)
        {
            this.BeziersControlPoints = BeziersControlPoints.Build(this.transform.InverseTransformDirection(worldSpaceStartPoint), this.transform.InverseTransformDirection(worldSpaceEndPoint), Vector3.up);
            this.DottedLineManager.OnComputeBeziersInnerPointEvent(this);
        }

        private void ClearLine()
        {
            this.meshFilter.mesh = new Mesh();
        }

    }
}

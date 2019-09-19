using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class DottedLine : MonoBehaviour
    {
        private DottedLineID dottedLineID;
        private DottedLineInherentData dottedLineInherentData;

        #region Internal Dependencies
        private MeshFilter meshFilter;
        private MeshRenderer MeshRenderer;

        public MeshFilter MeshFilter { get => meshFilter; set => meshFilter = value; }
        #endregion

        #region External Dependencies
        private IDottedLineRendererManagerEvent IDottedLineRendererManagerEvent;
        #endregion

        public static DottedLine CreateInstance(DottedLineID DottedLineID, PuzzleGameConfigurationManager puzzleGameConfigurationManager)
        {
            var instanciatedDottedLine = MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration.BaseDottedLineBasePrefab,
                   PuzzleGameSingletonInstances.DottedLineContainer.transform);
            instanciatedDottedLine.transform.position = Vector3.zero;
            instanciatedDottedLine.transform.localScale = Vector3.one;
            instanciatedDottedLine.transform.rotation = Quaternion.identity;
            instanciatedDottedLine.Init(DottedLineID, puzzleGameConfigurationManager.DottedLineConfiguration()[DottedLineID]);
            return instanciatedDottedLine;
        }

        public void DestroyInstance()
        {
            if (this != null)
            {
                this.IDottedLineRendererManagerEvent.OnDottedLineDestroyed(this);
                MonoBehaviour.Destroy(this.gameObject);
            }
        }

        public void Init(DottedLineID dottedLineID, DottedLineInherentData dottedLineInherentData)
        {
            this.dottedLineID = dottedLineID;
            this.dottedLineInherentData = dottedLineInherentData;

            #region External Dependencies
            this.IDottedLineRendererManagerEvent = PuzzleGameSingletonInstances.DottedLineRendererManager;
            #endregion

            #region Internal Dependencies
            this.MeshFilter = GetComponent<MeshFilter>();
            this.MeshRenderer = GetComponent<MeshRenderer>();
            #endregion

            this.MeshFilter.mesh = new Mesh();

            this.MeshRenderer.material.SetColor("_BaseColor", this.dottedLineInherentData.BaseColor);
            this.MeshRenderer.material.SetColor("_MovingColor", this.dottedLineInherentData.MovingColor);
            this.MeshRenderer.material.SetFloat("_MovingWidth", this.dottedLineInherentData.MovingWidth);
        }

        #region State
        private BeziersControlPoints BeziersControlPoints;
        private float currentPosition = 0f;
        private Vector3 LastFrameWorldSpaceStartPoint;
        private Vector3 LastFrameWorldSpaceEndPoint;
        #endregion

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
            
            this.currentPosition += d;
            if (this.currentPosition > 1)
            {
                this.currentPosition = 1 - this.currentPosition;
            }

            //The beziers control points can be null because DottedLine can be instanciated and immediately clear line. Thus, this.RePositionLine is not called,
            //thus, beziers calculation is not triggered.
            if (this.BeziersControlPoints != null)
            {
                this.MeshRenderer.material.SetVector("_ColorPointPosition", this.transform.TransformPoint(this.BeziersControlPoints.ResolvePoint(this.currentPosition)));
            }

            this.LastFrameWorldSpaceStartPoint = worldSpaceStartPoint;
            this.LastFrameWorldSpaceEndPoint = worldSpaceEndPoint;
        }

        public ComputeBeziersInnerPointEvent BuildComputeBeziersInnerPointEvent()
        {
            return new ComputeBeziersInnerPointEvent(this.GetInstanceID(), this.BeziersControlPoints, this.dottedLineInherentData.ModelScale, this.dottedLineInherentData.DotPerUnitDistance);
        }

        private void RePositionLine(Vector3 worldSpaceStartPoint, Vector3 worldSpaceEndPoint, Vector3 normal)
        {
            BeziersControlPointsShape BeziersControlPointsShape = BeziersControlPointsShape.CURVED;
            if(this.dottedLineInherentData.DottedLineType == DottedLineType.STRAIGHT)
            {
                BeziersControlPointsShape = BeziersControlPointsShape.STRAIGHT;
            }
            this.BeziersControlPoints = BeziersControlPoints.Build(this.transform.InverseTransformDirection(worldSpaceStartPoint), this.transform.InverseTransformDirection(worldSpaceEndPoint), Vector3.up, BeziersControlPointsShape);
            this.IDottedLineRendererManagerEvent.OnComputeBeziersInnerPointEvent(this);
        }

        private void ClearLine()
        {
            this.meshFilter.mesh = new Mesh();
        }

    }
}

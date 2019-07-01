using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTPuzzle
{
    public class GroundEffectsManagerV2 : MonoBehaviour
    {

        public Material MasterRangeMaterial;

        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private CommandBuffer command;
        private GroundEffectType[] AffectedGroundEffectsType;
        private HashSet<MeshRenderer> RenderedRenderers = new HashSet<MeshRenderer>();

        private Dictionary<RangeTypeID, IAbstractGroundEffectManager> rangeEffectManagers = new Dictionary<RangeTypeID, IAbstractGroundEffectManager>();

        private List<RangeTypeID> rangeEffectRenderOrder = new List<RangeTypeID>() {
            RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
            RangeTypeID.ATTRACTIVE_OBJECT,
            RangeTypeID.LAUNCH_PROJECTILE,
            RangeTypeID.LAUNCH_PROJECTILE_CURSOR,
            RangeTypeID.TARGET_ZONE
        };

        private List<CircleRangeBufferData> CircleRangeBufferValues = new List<CircleRangeBufferData>();
        private ComputeBuffer CircleRangeBuffer;

        private List<BoxRangeBufferData> BoxRangeBufferValues = new List<BoxRangeBufferData>();
        private ComputeBuffer BoxRangeBuffer;

        private List<RangeExecutionOrderBufferData> RangeExecutionOrderBufferDataValues = new List<RangeExecutionOrderBufferData>();
        private ComputeBuffer RangeExecutionOrderBuffer;

        public void Init()
        {

            #region External Dependencies
            PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            #endregion

            var camera = Camera.main;

            this.command = new CommandBuffer();
            this.command.name = "GroundEffectsManagerV2";
            camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.command);

            AffectedGroundEffectsType = GetComponentsInChildren<GroundEffectType>();
            for (var i = 0; i < AffectedGroundEffectsType.Length; i++)
            {
                AffectedGroundEffectsType[i].Init();
            }

            this.CircleRangeBuffer = new ComputeBuffer(this.rangeEffectRenderOrder.Count, CircleRangeBufferData.GetByteSize());
            this.CircleRangeBuffer.SetData(this.CircleRangeBufferValues);
            this.MasterRangeMaterial.SetBuffer("CircleRangeBuffer", this.CircleRangeBuffer);

            this.BoxRangeBuffer = new ComputeBuffer(this.rangeEffectRenderOrder.Count, BoxRangeBufferData.GetByteSize());
            this.BoxRangeBuffer.SetData(this.BoxRangeBufferValues);
            this.MasterRangeMaterial.SetBuffer("BoxRangeBuffer", this.BoxRangeBuffer);

            this.RangeExecutionOrderBuffer = new ComputeBuffer(this.rangeEffectRenderOrder.Count, 3 * sizeof(int));
            this.RangeExecutionOrderBuffer.SetData(this.RangeExecutionOrderBufferDataValues);
            this.MasterRangeMaterial.SetBuffer("RangeExecutionOrderBuffer", this.RangeExecutionOrderBuffer);
        }

        public void Tick(float d)
        {
            this.RenderedRenderers.Clear();
            foreach (var groundEffectManager in this.rangeEffectManagers.Values)
            {
                if (groundEffectManager != null)
                {
                    groundEffectManager.Tick(d);
                    groundEffectManager.MeshToRender(ref this.RenderedRenderers, this.AffectedGroundEffectsType);
                }
            }

            this.CircleRangeBufferValues.Clear();
            this.BoxRangeBufferValues.Clear();
            this.RangeExecutionOrderBufferDataValues.Clear();
            foreach (var rangeEffectId in this.rangeEffectRenderOrder)
            {
                if (this.rangeEffectManagers.ContainsKey(rangeEffectId))
                {
                    var rangeEffectManager = this.rangeEffectManagers[rangeEffectId];
                    if (rangeEffectManager.GetType() == typeof(SphereGroundEffectManager))
                    {
                        this.CircleRangeBufferValues.Add(((SphereGroundEffectManager)rangeEffectManager).ToSphereBuffer());
                        this.RangeExecutionOrderBufferDataValues.Add(new RangeExecutionOrderBufferData(1, 0, this.CircleRangeBufferValues.Count - 1));
                    }
                    else if (rangeEffectManager.GetType() == typeof(BoxGroundEffectManager))
                    {
                        this.BoxRangeBufferValues.Add(((BoxGroundEffectManager)rangeEffectManager).ToBoxBuffer());
                        this.RangeExecutionOrderBufferDataValues.Add(new RangeExecutionOrderBufferData(0, 1, this.BoxRangeBufferValues.Count - 1));
                    }
                }
            }

            if (this.CircleRangeBuffer.IsValid())
            {
                this.CircleRangeBuffer.SetData(this.CircleRangeBufferValues);
            }
            if (this.BoxRangeBuffer.IsValid())
            {
                this.BoxRangeBuffer.SetData(this.BoxRangeBufferValues);
            }
            if (this.RangeExecutionOrderBuffer.IsValid())
            {
                this.RangeExecutionOrderBuffer.SetData(this.RangeExecutionOrderBufferDataValues);
            }

            this.OnCommandBufferUpdate();
        }

        #region External events
        public void OnRangeAdded(RangeType rangeType)
        {
            if (rangeType.IsRangeConfigurationDefined())
            {
                if (rangeType.GetType() == typeof(SphereRangeType))
                {
                    var sphereRangeType = (SphereRangeType)rangeType;
                    this.rangeEffectManagers[rangeType.RangeTypeID] = (IAbstractGroundEffectManager)new SphereGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeType.RangeTypeID]);
                    this.rangeEffectManagers[rangeType.RangeTypeID].OnRangeCreated(sphereRangeType);
                }
                else if (rangeType.GetType() == typeof(BoxRangeType))
                {
                    var boxRangeType = (BoxRangeType)rangeType;
                    this.rangeEffectManagers[rangeType.RangeTypeID] = (IAbstractGroundEffectManager)new BoxGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeType.RangeTypeID]);
                    this.rangeEffectManagers[rangeType.RangeTypeID].OnRangeCreated(boxRangeType);
                }
            }
        }

        internal void OnLevelExit()
        {
            //release buffers
            ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.CircleRangeBuffer);
            ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.BoxRangeBuffer);
            ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.RangeExecutionOrderBuffer);
        }

        internal void OnRangeDestroy(RangeType rangeType)
        {
            if (rangeType.IsRangeConfigurationDefined())
            {
                this.rangeEffectManagers[rangeType.RangeTypeID].OnRangeDestroyed();
                this.rangeEffectManagers.Remove(rangeType.RangeTypeID);
            }
        }

        private void OnDestroy()
        {
            this.OnLevelExit();
        }
        #endregion

        internal void OnCommandBufferUpdate()
        {
            this.command.Clear();
            this.MasterRangeMaterial.SetInt("_CountSize", this.RangeExecutionOrderBufferDataValues.Count);

            foreach (var rangeEffectId in this.rangeEffectRenderOrder)
            {
                if (this.rangeEffectManagers.ContainsKey(rangeEffectId))
                {
                    this.rangeEffectManagers[rangeEffectId].MeshToRender(ref this.RenderedRenderers, this.AffectedGroundEffectsType);
                }
            }

            foreach (var meshToRender in this.RenderedRenderers)
            {
                this.command.DrawRenderer(meshToRender, this.MasterRangeMaterial);
            }
        }

    }

    struct RangeExecutionOrderBufferData
    {
        public int IsSphere;
        public int IsCube;
        public int Index;

        public RangeExecutionOrderBufferData(int isSphere, int isCube, int index)
        {
            IsSphere = isSphere;
            IsCube = isCube;
            Index = index;
        }
    }
}

﻿using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using CoreGame;
using System;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class GroundEffectsManagerV2 : MonoBehaviour
    {

        public const string GroundEffectBufferShaderProperty = "_GroundEffectBuffer";

        public const string AURA_RADIUS_MATERIAL_PROPERTY = "_Radius";
        public const string AURA_CENTER_MATERIAL_PROPERTY = "_CenterWorldPosition";
        public const string AURA_COLOR_MATERIAL_PROPERTY = "_AuraColor";

        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private CommandBuffer command;
        private GroundEffectType[] AffectedGroundEffectsType;

        private Dictionary<RangeTypeID, SphereGroundEffectManager> rangeEffectManagers = new Dictionary<RangeTypeID, SphereGroundEffectManager>();
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
        }

        public void Tick(float d)
        {
            foreach (var groundEffectManager in this.rangeEffectManagers.Values)
            {
                if (groundEffectManager != null)
                {
                    groundEffectManager.Tick(d);
                }
            }

            this.OnCommandBufferUpdate();
        }

        public void OnRangeAdded(RangeType rangeType)
        {
            var sphereRangeType = (SphereRangeType)rangeType;
            this.rangeEffectManagers[rangeType.RangeTypeID] = new SphereGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeType.RangeTypeID]);
            this.rangeEffectManagers[rangeType.RangeTypeID].OnAttractiveObjectActionStart(sphereRangeType);
            OnCommandBufferUpdate();
        }

        internal void OnRangeDeleted(RangeType rangeType)
        {
            this.rangeEffectManagers[rangeType.RangeTypeID].OnAttractiveObjectActionEnd();
            this.rangeEffectManagers.Remove(rangeType.RangeTypeID);
            OnCommandBufferUpdate();
        }

        internal void OnCommandBufferUpdate()
        {
            this.command.Clear();
            foreach (var groundEffectManager in this.rangeEffectManagers.Values)
            {
                if (groundEffectManager != null)
                {
                    groundEffectManager.OnCommandBufferUpdate(this.command, this.AffectedGroundEffectsType);
                }
            }
        }
    }

    class SphereGroundEffectManager
    {
        private RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData;
        private FloatAnimation rangeAnimation;
        private SphereRangeType associatedSphereRange;

        public SphereGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData)
        {
            this.rangeTypeInherentConfigurationData = rangeTypeInherentConfigurationData;
            this.isAttractiveObjectRangeEnabled = false;
        }

        private bool isAttractiveObjectRangeEnabled;

        public bool IsAttractiveObjectRangeEnabled { get => isAttractiveObjectRangeEnabled; }

        internal void OnCommandBufferUpdate(CommandBuffer commandBuffer, GroundEffectType[] affectedGroundEffectsType)
        {
            if (isAttractiveObjectRangeEnabled)
            {
                foreach (var affectedGroundEffectType in affectedGroundEffectsType)
                {
                    if (affectedGroundEffectType.MeshRenderer.isVisible)
                    {
                        if(rangeTypeInherentConfigurationData.RangeColorProvider != null)
                        {
                            rangeTypeInherentConfigurationData.GoundEffectMaterial.SetColor(GroundEffectsManagerV2.AURA_COLOR_MATERIAL_PROPERTY, this.rangeTypeInherentConfigurationData.RangeColorProvider.Invoke());
                        }

                        commandBuffer.DrawRenderer(affectedGroundEffectType.MeshRenderer, rangeTypeInherentConfigurationData.GoundEffectMaterial, 0, 0);
                    }
                }
            }
        }

        public void Tick(float d)
        {
            if (isAttractiveObjectRangeEnabled)
            {
                this.rangeAnimation.Tick(d);
                this.rangeTypeInherentConfigurationData.GoundEffectMaterial.SetVector(GroundEffectsManagerV2.AURA_CENTER_MATERIAL_PROPERTY, this.associatedSphereRange.GetCenterWorldPos());
                this.rangeTypeInherentConfigurationData.GoundEffectMaterial.SetFloat(GroundEffectsManagerV2.AURA_RADIUS_MATERIAL_PROPERTY, this.rangeAnimation.CurrentValue);
            }
        }

        internal void OnAttractiveObjectActionStart(SphereRangeType sphereRangeType)
        {
            this.rangeAnimation = new FloatAnimation(sphereRangeType.GetRadiusRange(), rangeTypeInherentConfigurationData.RangeAnimationSpeed, 0f);
            this.isAttractiveObjectRangeEnabled = true;
            this.associatedSphereRange = sphereRangeType;
            this.Tick(0);
        }

        internal void OnAttractiveObjectActionEnd()
        {
            this.isAttractiveObjectRangeEnabled = false;
        }
    }
}

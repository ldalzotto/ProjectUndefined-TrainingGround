﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTPuzzle
{
    public class InRangeVisualFeedbackManager : MonoBehaviour
    {
        #region External dependencies
        private InteractiveObjectContainer InteractiveObjectContainer;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private CommandBuffer commandBuffer;

        public void Init()
        {
            this.commandBuffer = new CommandBuffer();
            this.commandBuffer.name = this.GetType().Name;
            Camera.main.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.commandBuffer);

            this.InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            this.PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
        }

        public void Tick(float d)
        {
            this.OnCommandBufferUpdate();
        }

        private void OnCommandBufferUpdate()
        {
            this.commandBuffer.Clear();

            foreach (var visualFeedbackEmitterModule in this.InteractiveObjectContainer.VisualFeedbackEmitterModules)
            {
                var effectMaterial = this.PuzzleGameConfigurationManager.RangeTypeConfiguration()[visualFeedbackEmitterModule.GetAssociatedRangeTypeID()].InRangeEffectMaterial;
                foreach (ModelObjectModule modelObjectModule in visualFeedbackEmitterModule.GetInRangeModelObjectsForVisual())
                {
                    if (modelObjectModule != null)
                    {
                        foreach (var r in modelObjectModule.GetAllRenderers())
                        {
                            this.commandBuffer.DrawRenderer(r, effectMaterial);
                        }
                    }
                }
            }
        }
    }
}
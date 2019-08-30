using System;
using UnityEngine;

namespace CoreGame
{
    public class TransformChangeListenerManager
    {
        private bool positionListening;
        private bool rotationListening;
        private TransformChangeListener TransformChangeListener;

        private Transform listeningTransform;

        private Nullable<Vector3> lastFramePosition;
        private Nullable<Quaternion> lastFrameRotation;

        private bool positionChangedThatFrame;
        private bool rotationChangedThatFrame;

        public TransformChangeListenerManager(Transform listeningTransform, bool positionListening, bool rotationListening, TransformChangeListener TransformChangeListener = null)
        {
            this.listeningTransform = listeningTransform;
            this.positionListening = positionListening;
            this.rotationListening = rotationListening;
            this.TransformChangeListener = TransformChangeListener;
            this.positionChangedThatFrame = false;
            this.rotationChangedThatFrame = false;
        }

        #region Logical Conditions
        public bool TransformChangedThatFrame() { return this.positionChangedThatFrame || this.rotationChangedThatFrame; }
        #endregion

        public void Tick()
        {
            this.positionChangedThatFrame = false;
            this.rotationChangedThatFrame = false;

            if (this.positionListening)
            {
                if (this.lastFramePosition == null)
                {
                    this.PositionChanged();
                }
                else
                {
                    if (this.lastFramePosition.Value != listeningTransform.position)
                    {
                        this.PositionChanged();
                    }
                }
                this.lastFramePosition = listeningTransform.position;
            }

            if (this.rotationListening)
            {
                if (this.lastFrameRotation == null)
                {
                    this.RotationChanged();
                }
                else
                {
                    if (this.lastFrameRotation.Value != listeningTransform.rotation)
                    {
                        this.RotationChanged();
                    }
                }
                this.lastFrameRotation = listeningTransform.rotation;
            }
        }

        private void PositionChanged()
        {
            if (this.TransformChangeListener != null) { this.TransformChangeListener.onPositionChange(); }
            this.positionChangedThatFrame = true;
        }

        private void RotationChanged()
        {
            if (this.TransformChangeListener != null) { this.TransformChangeListener.onRotationChange(); }
            this.rotationChangedThatFrame = true;
        }
    }

    public interface TransformChangeListener
    {
        void onPositionChange();
        void onRotationChange();
    }
}


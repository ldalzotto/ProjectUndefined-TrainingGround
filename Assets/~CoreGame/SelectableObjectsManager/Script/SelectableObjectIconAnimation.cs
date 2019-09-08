using UnityEngine;

namespace CoreGame
{
    public class SelectableObjectIconAnimation
    {
        private const float MaxIconScale = 1f;
        private const float MinIconScale = 0.5f;

        private float iconScale;
        private float rotationAngle;

        public SelectableObjectIconAnimation()
        {
            this.iconScale = MaxIconScale;
        }

        public float GetIconScale()
        {
            return Mathf.Clamp(this.iconScale, MinIconScale, MaxIconScale);
        }

        public float GetRotationAngleDeg()
        {
            return this.rotationAngle;
        }

        public void Tick(float d, bool hasMultipleAvailableSelectionObjects)
        {
            this.iconScale -= (d * 2f);
            if (this.iconScale <= 0)
            {
                this.iconScale = MaxIconScale;
            }

            if (hasMultipleAvailableSelectionObjects)
            {
             //   this.iconScale = MaxIconScale;
                this.rotationAngle += d * 360f;
            }
            else
            {
                this.rotationAngle = 0f;
            }
        }
    }
}

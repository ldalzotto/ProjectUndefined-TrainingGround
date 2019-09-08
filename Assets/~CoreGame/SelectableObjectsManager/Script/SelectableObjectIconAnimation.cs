using UnityEngine;

namespace CoreGame
{
    public class SelectableObjectIconAnimation
    {
        private const float MaxIconScale = 1f;
        private const float MinIconScale = 0.5f;

        private float iconScale;

        public SelectableObjectIconAnimation()
        {
            this.iconScale = MaxIconScale;
        }
        
        public float GetIconScale()
        {
            return Mathf.Clamp(this.iconScale, MinIconScale, MaxIconScale);
        }

        public void Tick(float d)
        {
            this.iconScale -= (d * 2f);
            if (this.iconScale <= 0)
            {
                this.iconScale = MaxIconScale;
            }
        }
    }
}

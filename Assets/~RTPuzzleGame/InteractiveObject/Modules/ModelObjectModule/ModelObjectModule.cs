using UnityEngine;
using System.Collections;
using CoreGame;

namespace RTPuzzle
{
    public class ModelObjectModule : InteractiveObjectModule, IRenderBoundRetrievable
    {
        #region Properties
        private ExtendedBounds AverageModeBounds;
        #endregion

        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            return this.AverageModeBounds;
        }

        public void Init()
        {
            this.AverageModeBounds = BoundsHelper.GetAverageRendererBounds(this.GetComponentsInChildren<Renderer>());
        }
    }
}

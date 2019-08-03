using UnityEngine;
using System.Collections;
using CoreGame;

namespace RTPuzzle
{
    public class ModelObjectModule : InteractiveObjectModule, IRenderBoundRetrievable
    {
        #region Properties
        private ExtendedBounds AverageModeBounds;
        private Animator animator;
        #endregion

        #region Data Retrieval
        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            return this.AverageModeBounds;
        }
        public Animator Animator { get => animator; }
        #endregion

        public void Init()
        {
            this.AverageModeBounds = BoundsHelper.GetAverageRendererBounds(this.GetComponentsInChildren<Renderer>());
            this.animator = GetComponent<Animator>();
            if(this.animator == null)
            {
                this.animator = GetComponentInChildren<Animator>();
            }
        }
    }
}

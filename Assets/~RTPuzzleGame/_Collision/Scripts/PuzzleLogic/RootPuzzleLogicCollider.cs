using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class RootPuzzleLogicCollider : MonoBehaviour
    {
        private Collider boxCollider;

        public void Init()
        {
            this.boxCollider = GetComponent<BoxCollider>();
        }

        #region Data Retrieval
        public Collider GetRootCollider()
        {
            return this.boxCollider;
        }
        #endregion
    }
}

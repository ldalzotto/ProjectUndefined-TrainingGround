﻿using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class LaunchProjectileGroundColliderTracker : MonoBehaviour
    {

        #region External Dependencies
        private LaunchProjectileEventManager LaunchProjectileEventManager;
        private LaunchProjectile LaunchProjectileRef;
        #endregion

        private Collider sphereCollider;

        public Collider SphereCollider { get => sphereCollider; }

        public void Init(LaunchProjectile LaunchProjectileRef)
        {
            this.LaunchProjectileEventManager = GameObject.FindObjectOfType<LaunchProjectileEventManager>();
            this.LaunchProjectileRef = LaunchProjectileRef;
            this.sphereCollider = GetComponent<SphereCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER))
            {
                this.LaunchProjectileEventManager.OnProjectileGroundTriggerEnter(this.LaunchProjectileRef);
            }
        }
    }
}

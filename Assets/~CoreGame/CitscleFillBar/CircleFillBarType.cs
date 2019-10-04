﻿using UnityEngine;

namespace CoreGame
{
    public class CircleFillBarType : MonoBehaviour
    {
        private Camera cam;
        private float currentProgression;
        
        public float CurrentProgression { get => currentProgression; }

        public void Init(Camera camera)
        {
            this.cam = camera;
            CoreGameSingletonInstances.CircleFillBarRendererManager.OnCircleFillBarTypeCreated(this);
        }

        public void Tick(float progression)
        {
            this.currentProgression = progression;
            this.transform.LookAt(this.cam.transform);
        }

        public void OnCircleFillBarTypeEnabled()
        {
            CoreGameSingletonInstances.CircleFillBarRendererManager.OnCircleFillBarTypeCreated(this);
        }

        private void OnDisable()
        {
            if (CoreGameSingletonInstances.CircleFillBarRendererManager != null)
            {
                CoreGameSingletonInstances.CircleFillBarRendererManager.OnCircleFillBarTypeDestroyed(this);
            }
        }

        public static void EnableInstace(CircleFillBarType CircleFillBarTypeRef)
        {
            CircleFillBarTypeRef.gameObject.SetActive(true);
            CircleFillBarTypeRef.OnCircleFillBarTypeEnabled();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimationSystem
{

    public class AnimationActor : MonoBehaviour
    {
        public AnimationActorDefinition AnimationActorDefinition;

        private Dictionary<Type, AbstractAnimationActorModule> enabledModules;

        public void Init()
        {
            this.enabledModules = new Dictionary<Type, AbstractAnimationActorModule>();

            var modelModule = this.GetComponentsInCurrentAndChild<AnimationActorModelModule>();
            if (modelModule != null && modelModule.Count > 0)
            {
                this.enabledModules.Add(modelModule[0].GetType(), modelModule[0]);
            }

            this.Define();
            this.enabledModules.Values.ToList().ForEach(m => m.Init(this));
        }

        private void Define()
        {
            if (this.AnimationActorDefinition.AnimationActorModelModuleDefinition.isEnabled)
            {
                var AnimationActorMovementModuleObject = new GameObject(typeof(AnimationActorMovementModule).Name);
                AnimationActorMovementModuleObject.transform.SetParent(this.transform);
                AnimationActorMovementModuleObject.transform.localPosition = Vector3.zero;
                var AnimationActorMovementModule = AnimationActorMovementModuleObject.AddComponent<AnimationActorMovementModule>();
                var SphereCollider = AnimationActorMovementModuleObject.AddComponent<SphereCollider>();
                SphereCollider.isTrigger = true;
                SphereCollider.radius = this.AnimationActorDefinition.AnimationActorModelModuleDefinition.triggerRange;

                this.enabledModules.Add(typeof(AnimationActorMovementModule), AnimationActorMovementModule);
            }
        }

        public T GetModule<T>() where T : AbstractAnimationActorModule
        {
            return (T)this.enabledModules[typeof(T)];
        }

        public void SeenByCameraTick(float d)
        {
            foreach (var enabledModule in this.enabledModules.Values)
            {
                enabledModule.SeenByCameraTick(d);
            }
        }

        public void OnVisible()
        {
            this.gameObject.SetActive(true);
            foreach (var enabledModule in this.enabledModules.Values)
            {
                enabledModule.OnVisible();
            }
        }

        public void OnNotVisible()
        {
            this.gameObject.SetActive(false);
            foreach (var enabledModule in this.enabledModules.Values)
            {
                enabledModule.OnNotVisible();
            }
        }

        private void OnDrawGizmos()
        {
            if (this.AnimationActorDefinition.AnimationActorModelModuleDefinition.isEnabled)
            {
                Gizmos.DrawWireSphere(this.transform.position, this.AnimationActorDefinition.AnimationActorModelModuleDefinition.triggerRange);
            }
        }
    }
    

    [System.Serializable]
    public class AnimationActorDefinition
    {
        public AnimationActorModelModuleDefinition AnimationActorModelModuleDefinition;
    }

}

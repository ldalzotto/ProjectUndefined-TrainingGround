using System;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class ContextMarkVisualFeedbackMarkType : MonoBehaviour
    {

        private const string MarkObjectBaseName = "MarkObject";
        private const string ParticleObjectBaseName = "MarkParticles";
        
        private VertexUnlitInstanciatedPropertySetter[] materialPropertySetters;
        
        public static ContextMarkVisualFeedbackMarkType Instanciate(ContextMarkVisualFeedbackMarkType prefab, Action<ContextMarkVisualFeedbackMarkType> afterCreation = null)
        {
            var aiFeedbackMarkType = MonoBehaviour.Instantiate(prefab, PuzzleGameSingletonInstances.ContextMarkVisualFeedbackContainer.transform);
            if (afterCreation != null) { afterCreation.Invoke(aiFeedbackMarkType); }
            aiFeedbackMarkType.Init();
            return aiFeedbackMarkType;
        }


        public void Init()
        {
            var childsPropertySetter = GetComponentsInChildren<VertexUnlitInstanciatedPropertySetter>();
            var basePeropertySetter = GetComponents<VertexUnlitInstanciatedPropertySetter>();
            this.materialPropertySetters = childsPropertySetter.Concat(basePeropertySetter).ToArray();
            foreach (var materialPropertySetter in this.materialPropertySetters)
            {
                materialPropertySetter.Init();
            }
        }

        public void Tick(float d)
        {
            foreach (var materialPropertySetter in this.materialPropertySetters)
            {
                materialPropertySetter.Tick(d);
            }
        }

    }
}

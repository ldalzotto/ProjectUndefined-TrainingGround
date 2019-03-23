﻿using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class AIFeedbackMarkType : MonoBehaviour
    {

        private VertexUnlitInstanciatedPropertySetter[] materialPropertySetters;


        public static AIFeedbackMarkType Instanciate(AIFeedbackMarkType prefab, Transform parent)
        {
            var aiFeedbackMarkType = MonoBehaviour.Instantiate(prefab, parent);
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

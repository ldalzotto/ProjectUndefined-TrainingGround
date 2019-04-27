using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;
using System.Collections.Generic;

namespace Editor_AIContextMarkVisualFeedbackCreationWizardEditor
{
    [System.Serializable]
    public class MarkCreatorInput : CreatablePrefabInput
    {
        public List<SingleMarkCreatorInput> AIFeedbackMarks;
    }
    [System.Serializable]
    public class SingleMarkCreatorInput : CreatablePrefabInput
    {
        public string MarkParticleAdditionalName;
        public GameObject AIMarkModel;
        public Color ModelBaseColor = new Color(1,1,1,1);
        public Color ParticleColor;
    }

    public class SingleMarkCreator : CreateablePrefabComponent<MarkCreatorInput, AIFeedbackMarkType>
    {

        public SingleMarkCreator(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

    }

}

using UnityEngine;
using System.Collections;
using RTPuzzle;
using System.Collections.Generic;
using System;

namespace Editor_AIContextMarkVisualFeedbackCreationWizardEditor
{

    public class AlternanceMarkCreator : CreateablePrefabComponent<AIFeedbackMarkType>
    {
        public AlternanceMarkCreator(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public override Func<Dictionary<string, CreationModuleComponent>, AIFeedbackMarkType> BasePrefabProvider => throw new NotImplementedException();

        public override void ResetEditor()
        {
        }
    }

}

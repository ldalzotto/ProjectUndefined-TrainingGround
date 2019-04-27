using UnityEngine;
using System.Collections;
using RTPuzzle;
using System.Collections.Generic;

namespace Editor_AIContextMarkVisualFeedbackCreationWizardEditor
{

    public class AlternanceMarkCreator : CreateablePrefabComponent<MarkCreatorInput, AIFeedbackMarkType>
    {
        public AlternanceMarkCreator(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public override void ResetEditor()
        {
        }
    }

}

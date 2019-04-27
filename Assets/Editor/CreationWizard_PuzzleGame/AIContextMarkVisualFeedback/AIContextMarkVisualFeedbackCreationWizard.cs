using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using RTPuzzle;
using UnityEditor;
using System;

namespace Editor_AIContextMarkVisualFeedbackCreationWizardEditor
{
    public class AIContextMarkVisualFeedbackCreationWizard : AbstractCreationWizardEditor<AIContextMarkVisualFeedbackEditorProfile>
    {
        protected override void OnGenerationClicked(Scene tmpScene)
        {
            var GenericInformation = this.GetModule<GenericInformation>();
            var SingleMarkCreator = this.GetModule<SingleMarkCreator>();
            var AlternanceMarkCreator = this.GetModule<AlternanceMarkCreator>();

            if (SingleMarkCreator.ModuleEnabled)
            {
                var aiFeedbackMark = SingleMarkCreator.Create(GenericInformation.SingleAIMark, tmpScene, GenericInformation.PathConfiguration.AIFeedbackPrefabPath,
                    NamingConventionHelper.BuildName(GenericInformation.MarkObjectBaseName, PrefixType.AI_FEEDBACK_MARK, SufixType.NONE),
                      afterBaseCreation: (AIFeedbackMarkType AIFeedbackMarkType, MarkCreatorInput markCreatorInput) =>
                      {
                          AIContextMarkCreationHelper.CreateSingleMark(tmpScene, AIFeedbackMarkType, markCreatorInput.AIFeedbackMarks[0], GenericInformation, ref this.editorProfile);
                      });
                this.editorProfile.GeneratedObjects.Add(aiFeedbackMark);
            }
            else if (AlternanceMarkCreator.ModuleEnabled)
            {
                var aiFeedbackMark = AlternanceMarkCreator.Create(GenericInformation.AlternanceAIMark, tmpScene, GenericInformation.PathConfiguration.AIFeedbackPrefabPath,
                   NamingConventionHelper.BuildName(GenericInformation.MarkObjectBaseName, PrefixType.AI_FEEDBACK_MARK, SufixType.NONE),
                     afterBaseCreation: (AIFeedbackMarkType AIFeedbackMarkType, MarkCreatorInput markCreatorInput) =>
                     {
                         for (var i = 0; i < markCreatorInput.AIFeedbackMarks.Count; i++)
                         {
                             AIContextMarkCreationHelper.CreateSingleMark(tmpScene, AIFeedbackMarkType, markCreatorInput.AIFeedbackMarks[i], GenericInformation, ref this.editorProfile,
                                 AIFeedbackMarkType.gameObject.FindChildObjectRecursively(AIFeedbackMarkType.GetMarkObjectName(i + 1)).transform,
                                 AIFeedbackMarkType.gameObject.FindChildObjectRecursively(AIFeedbackMarkType.GetParticleObjectName(i + 1)).transform);
                         }
                     });
                this.editorProfile.GeneratedObjects.Add(aiFeedbackMark);
            }
        }



        protected override void OnWizardGUI()
        {
            this.GetModule<GenericInformation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<AIContextMarkTypePicker>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<SingleMarkCreator>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<AlternanceMarkCreator>().OnInspectorGUI(ref this.editorProfile.Modules);


            var AIContextMarkTypePicker = this.GetModule<AIContextMarkTypePicker>();
            var SingleMarkCreator = this.GetModule<SingleMarkCreator>();
            var AlternanceMarkCreator = this.GetModule<AlternanceMarkCreator>();
            if (AIContextMarkTypePicker.AlternanceMark)
            {
                SingleMarkCreator.ModuleEnabled = false;
                AlternanceMarkCreator.ModuleEnabled = true;
            }
            else if (AIContextMarkTypePicker.SingleMark)
            {
                SingleMarkCreator.ModuleEnabled = true;
                AlternanceMarkCreator.ModuleEnabled = false;
            }
        }
    }

}

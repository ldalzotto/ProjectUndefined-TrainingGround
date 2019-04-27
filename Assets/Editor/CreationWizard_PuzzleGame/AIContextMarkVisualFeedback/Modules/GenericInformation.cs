using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;
using System.Collections.Generic;

namespace Editor_AIContextMarkVisualFeedbackCreationWizardEditor
{
    [System.Serializable]
    public class GenericInformation : CreationModuleComponent
    {
        public string MarkObjectBaseName;
        public PathConfiguration PathConfiguration;
        public AIFeedbackMarkType SingleAIMark;
        public AIFeedbackMarkType AlternanceAIMark;
        public ParticleSystem BaseAIFeedbackParticlesPrefab;
        public RuntimeAnimatorController SingleAIFeedbackAniamtorController;

        public GenericInformation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject)
        {
            if (this.SingleAIMark == null)
            {
                this.SingleAIMark = AssetFinder.SafeSingleAssetFind<AIFeedbackMarkType>("SingleAIMark");
            }
            if (this.AlternanceAIMark == null)
            {
                this.AlternanceAIMark = AssetFinder.SafeSingleAssetFind<AIFeedbackMarkType>("AlternanceAIMark");
            }
            if (this.BaseAIFeedbackParticlesPrefab == null)
            {
                this.BaseAIFeedbackParticlesPrefab = AssetFinder.SafeSingleAssetFind<ParticleSystem>("BaseAIFeedbackParticles");
            }
            if (this.SingleAIFeedbackAniamtorController == null)
            {
                this.SingleAIFeedbackAniamtorController = AssetFinder.SafeSingleAssetFind<RuntimeAnimatorController>("AIFeedbackbaseAnimator");
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.MarkObjectBaseName)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.SingleAIMark)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.AlternanceAIMark)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.BaseAIFeedbackParticlesPrefab)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.SingleAIFeedbackAniamtorController)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.PathConfiguration)), true);
        }

    }

    [System.Serializable]
    public class PathConfiguration
    {
        public string AIFeedbackPrefabPath = "Assets/~RTPuzzleGame/UI/AIFeedback/Prefabs";
    }


}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using RTPuzzle;
using UnityEditor;

namespace Editor_AIContextMarkVisualFeedbackCreationWizardEditor
{
    public class AIContextMarkCreationHelper
    {

        public static void CreateSingleMark(Scene tmpScene, AIFeedbackMarkType AIFeedbackMarkType, SingleMarkCreatorInput singleMarkCreatorInput,
            GenericInformation GenericInformation, ref AIContextMarkVisualFeedbackEditorProfile aIContextMarkVisualFeedbackEditorProfile,
         Transform modelObjectParent = null, Transform particleObjectParent = null)
        {
            GameObject aiMarkModel = null;
            if (modelObjectParent == null)
            {
                aiMarkModel = (GameObject)PrefabUtility.InstantiatePrefab(singleMarkCreatorInput.AIMarkModel, AIFeedbackMarkType.transform);
            }
            else
            {
                aiMarkModel = (GameObject)PrefabUtility.InstantiatePrefab(singleMarkCreatorInput.AIMarkModel, modelObjectParent);
            }

            //setup base animator (rotation et appearing)
            var aiMarkModelAnimator = aiMarkModel.GetComponent<Animator>();
            if (aiMarkModelAnimator == null)
            {
                aiMarkModelAnimator = aiMarkModel.AddComponent<Animator>();
            }
            aiMarkModelAnimator.runtimeAnimatorController = GenericInformation.SingleAIFeedbackAniamtorController;

            //setup model renderer
            var renderes = aiMarkModel.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderes)
            {
                renderer.material = GenericInformation.AIFeedbackMarkUnlitMaterial;
                if (renderer.GetComponent<VertexUnlitInstanciatedPropertySetter>() == null)
                {
                    var VertexUnlitInstanciatedPropertySetter = renderer.gameObject.AddComponent<VertexUnlitInstanciatedPropertySetter>();
                    VertexUnlitInstanciatedPropertySetter.colorToSet = singleMarkCreatorInput.ModelBaseColor;
                }
            }


            //TODO -> multiple renderers
            //setup particles
            var meshRenderers = AIFeedbackMarkType.GetComponentsInChildren<MeshRenderer>();
            for (var i = 0; i < meshRenderers.Length; i++)
            {
                var aiFeedbackMeshRenderer = meshRenderers[i];
                var particleSystem = new GeneratedPrefabAssetManager<ParticleSystem>(GenericInformation.BaseAIFeedbackParticlesPrefab, tmpScene, GenericInformation.PathConfiguration.AIFeedbackPrefabPath,
                 NamingConventionHelper.BuildName(GenericInformation.MarkObjectBaseName + "_" + singleMarkCreatorInput.MarkParticleAdditionalName + i, PrefixType.AI_FEEDBACK_MARK, SufixType.PARTICLE_SYSTEM),
                 afterAssetCreation: (ParticleSystem ParticleSystem) =>
                 {
                     var mainModule = ParticleSystem.main;
                     var startColor = mainModule.startColor;
                     var gradient = startColor.gradient;
                     var gradientColorKeys = gradient.colorKeys;
                     for (var j = 0; j < gradientColorKeys.Length; j++)
                     {
                         gradientColorKeys[j].color = singleMarkCreatorInput.ParticleColor;
                     }
                     gradient.colorKeys = gradientColorKeys;
                     startColor.gradient = gradient;
                     mainModule.startColor = startColor;
                 });
                aIContextMarkVisualFeedbackEditorProfile.GeneratedObjects.Add(particleSystem.SavedAsset);
                UnityEngine.Object instanciatedParticleSystemObject = null;

                if (particleObjectParent == null)
                {
                    instanciatedParticleSystemObject = PrefabUtility.InstantiatePrefab(particleSystem.SavedAsset, AIFeedbackMarkType.transform);
                }
                else
                {
                    instanciatedParticleSystemObject = PrefabUtility.InstantiatePrefab(particleSystem.SavedAsset, particleObjectParent);
                }

                var instanciatedParticleSystem = ((GameObject)instanciatedParticleSystemObject).GetComponent<ParticleSystem>();

                //setting particle emission mesh renderer
                var shapeModule = instanciatedParticleSystem.shape;
                shapeModule.meshRenderer = aiFeedbackMeshRenderer;
            }


        }
    }
}

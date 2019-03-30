using RTPuzzle;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    public class AttractiveObjectVariantCreationWizard : EditorWindow
    {
        [MenuItem("PlayerAction/Puzzle/AttractiveObject/AttractiveObjectVariantCreationWizard")]
        public static void Init()
        {
            var window = EditorWindow.GetWindow<AttractiveObjectVariantCreationWizard>();
            window.Show();
        }

        private AttractiveObjectVariantCreationWizardEditorProfile editorProfile;

        private void OnGUI()
        {
            this.editorProfile = (AttractiveObjectVariantCreationWizardEditorProfile)EditorGUILayout.ObjectField(this.editorProfile, typeof(AttractiveObjectVariantCreationWizardEditorProfile), false);

            if (this.editorProfile == null)
            {
                this.editorProfile = AssetFinder.SafeSingleAssetFind<AttractiveObjectVariantCreationWizardEditorProfile>("t:AttractiveObjectVariantCreationWizardEditorProfile");
            }

            if (this.editorProfile != null)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("R", "Reset creation wzard."), EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                {
                    this.editorProfile.ResetEditor();
                }
                EditorGUILayout.EndHorizontal();


                editorProfile.WizardScrollPosition = EditorGUILayout.BeginScrollView(editorProfile.WizardScrollPosition);
                EditorGUILayout.BeginVertical(EditorStyles.textArea);
                this.editorProfile.GenericInfoFoldout = EditorGUILayout.Foldout(this.editorProfile.GenericInfoFoldout, "Generic info : ", true);
                if (this.editorProfile.GenericInfoFoldout)
                {
                    Editor.CreateEditor(this.editorProfile).OnInspectorGUI();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.textArea);
                this.editorProfile.ModelCreation.ModelCreationFoldout = EditorGUILayout.Foldout(this.editorProfile.ModelCreation.ModelCreationFoldout, "Model creation : ", true);
                if (this.editorProfile.ModelCreation.ModelCreationFoldout)
                {
                    this.editorProfile.ModelCreation.OnInspectorGUI();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.textArea);
                this.editorProfile.ConfigurationRetrieval.ConfigurationFilesRetrievalFoldout = EditorGUILayout.Foldout(this.editorProfile.ConfigurationRetrieval.ConfigurationFilesRetrievalFoldout, "Configuration files : ", true);
                if (this.editorProfile.ConfigurationRetrieval.ConfigurationFilesRetrievalFoldout)
                {
                    this.editorProfile.ConfigurationRetrieval.OnInspectorGUI();
                }
                if (this.editorProfile.ConfigurationRetrieval.AttractiveObjectConfiguration == null)
                {
                    this.editorProfile.ConfigurationRetrieval.AttractiveObjectConfiguration = AssetFinder.SafeSingleAssetFind<AttractiveObjectConfiguration>("t:AttractiveObjectConfiguration");
                }
                EditorGUILayout.EndVertical();


                this.editorProfile.AttractiveObjectInherentData.OnGui();


                if (GUILayout.Button("GENERATE"))
                {
                    this.editorProfile.GeneratedObjects.Clear();
                    var tmpScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    tmpScene.name = UnityEngine.Random.Range(0, 999999).ToString();

                    try
                    {
                        #region Model Prefab
                        var modelAssetGeneration = new GeneratedPrefabAssetManager<GameObject>(this.editorProfile.ModelCreation.ModelAsset, PrefabAssetGenerationWorkflow.MODEL, tmpScene, this.editorProfile.ProjectRelativeTmpFolderPath,
                               NamingConventionHelper.BuildName(editorProfile.ObjectName, editorProfile.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.MODEL), null);
                        this.editorProfile.GeneratedObjects.Add(modelAssetGeneration.SavedAsset);
                        #endregion

                        #region Attractive Object Prefab
                        var attractiveObjectAssetGeneration = new GeneratedPrefabAssetManager<AttractiveObjectType>(this.editorProfile.AttractiveObjectBasePrefab, PrefabAssetGenerationWorkflow.PREFAB, tmpScene,
                            this.editorProfile.ProjectRelativeTmpFolderPath, NamingConventionHelper.BuildName(editorProfile.ObjectName, editorProfile.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.ATTRACTIVE_OBJECT),
                            (AttractiveObjectType generatedAsset) =>
                            {
                                PrefabUtility.InstantiatePrefab(modelAssetGeneration.SavedAsset, generatedAsset.transform);
                            });
                        this.editorProfile.GeneratedObjects.Add(attractiveObjectAssetGeneration.SavedAsset);
                        #endregion

                        #region Attractive object game configuration.
                        this.editorProfile.AttractiveObjectInherentData.CreatedObject.AttractiveObjectPrefab = attractiveObjectAssetGeneration.SavedAsset.GetComponent<AttractiveObjectType>();
                        this.editorProfile.AttractiveObjectInherentData.CreatedObject.AttractiveObjectModelPrefab = modelAssetGeneration.SavedAsset;
                        var attractiveObjectInherentConfigurationDataGenerated = this.editorProfile.AttractiveObjectInherentData.CreateAsset(this.editorProfile.ProjectRelativeTmpFolderPath, NamingConventionHelper.BuildName(editorProfile.ObjectName, editorProfile.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.ATTRACTIVE_OBJECT_INHERENT_DATA));
                        this.editorProfile.GeneratedObjects.Add(attractiveObjectInherentConfigurationDataGenerated);
                        #endregion

                        #region Game configuration update
                        this.editorProfile.ConfigurationRetrieval.AttractiveObjectConfiguration.SetEntry(this.editorProfile.AttractiveObjectId, attractiveObjectInherentConfigurationDataGenerated);
                        #endregion

                        #region Assets move from tmp
                        modelAssetGeneration.MoveGeneratedAsset(this.editorProfile.PathConfiguration.ObjectPrefabFolder);
                        attractiveObjectAssetGeneration.MoveGeneratedAsset(this.editorProfile.PathConfiguration.ObjectPrefabFolder);
                        this.editorProfile.AttractiveObjectInherentData.MoveGeneratedAsset(this.editorProfile.PathConfiguration.ObjectConfigurationFolder);
                        #endregion

                    }
                    catch (AssetMoveError e)
                    {
                        Debug.LogException(e);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        Debug.LogError(e.StackTrace);
                    }
                    finally
                    {
                        this.editorProfile.AttractiveObjectInherentData.OnGenerationEnd();
                        EditorSceneManager.CloseScene(tmpScene, true);
                    }
                }

                this.DoGenereatedObject();
                EditorGUILayout.EndScrollView();

            }
        }

        private void DoGenereatedObject()
        {
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Generated objects : ");
            if (GUILayout.Button(new GUIContent("D", "Delete all generations."), EditorStyles.miniButton, GUILayout.Width(20)))
            {
                foreach (var generatedAsset in this.editorProfile.GeneratedObjects)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(generatedAsset));
                }
                this.editorProfile.GeneratedObjects.Clear();
            }
            EditorGUILayout.EndHorizontal();
            foreach (var generatedObject in this.editorProfile.GeneratedObjects)
            {
                EditorGUILayout.ObjectField(generatedObject, typeof(UnityEngine.Object), false);
            }
            EditorGUILayout.EndVertical();
        }

     
    }


}
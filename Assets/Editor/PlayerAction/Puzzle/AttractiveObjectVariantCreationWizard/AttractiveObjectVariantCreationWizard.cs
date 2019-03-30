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


                this.editorProfile.AttractiveObjectGameConfigurationManager.OnGui(this.editorProfile);


                if (GUILayout.Button("GENERATE"))
                {
                    this.editorProfile.GeneratedObjects.Clear();
                    var tmpScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    tmpScene.name = UnityEngine.Random.Range(0, 999999).ToString();

                    try
                    {
                        #region Model Prefab
                        var modelObject = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(this.editorProfile.ModelCreation.ModelAsset));
                        var instanceRoot = (GameObject)PrefabUtility.InstantiatePrefab(modelObject, tmpScene);
                        var createdPrefabpath = this.editorProfile.ProjectRelativeTmpFolderPath;
                        var createdModelFileName = "\\" + NamingConventionHelper.BuildName(editorProfile.ObjectName, editorProfile.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.MODEL) + ".prefab";
                        var modelAsset = PrefabUtility.SaveAsPrefabAsset(instanceRoot, createdPrefabpath + createdModelFileName);
                        this.editorProfile.GeneratedObjects.Add(modelAsset);
                        GameObject.DestroyImmediate(instanceRoot);
                        Debug.Log("Generation : " + createdPrefabpath);
                        #endregion

                        #region Attractive Object Prefab
                        var generatedAttractiveObjectType = (AttractiveObjectType)PrefabUtility.InstantiatePrefab(this.editorProfile.AttractiveObjectBasePrefab, tmpScene);
                        PrefabUtility.InstantiatePrefab(modelAsset, generatedAttractiveObjectType.transform);
                        var savedAttractiveObjectTypeFileName = "\\" + NamingConventionHelper.BuildName(editorProfile.ObjectName, editorProfile.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.ATTRACTIVE_OBJECT) + ".prefab";
                        var savedAttractiveObjectType = PrefabUtility.SaveAsPrefabAsset(generatedAttractiveObjectType.gameObject, this.editorProfile.ProjectRelativeTmpFolderPath + savedAttractiveObjectTypeFileName);
                        this.editorProfile.GeneratedObjects.Add(savedAttractiveObjectType);
                        GameObject.DestroyImmediate(generatedAttractiveObjectType);
                        #endregion

                        #region Attractive object game configuration.
                        this.editorProfile.AttractiveObjectInherentConfigurationData.AttractiveObjectPrefab = savedAttractiveObjectType.GetComponent<AttractiveObjectType>();
                        this.editorProfile.AttractiveObjectInherentConfigurationData.AttractiveObjectModelPrefab = modelAsset;
                        var attractiveObjectFileName = "\\" + NamingConventionHelper.BuildName(editorProfile.ObjectName, editorProfile.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.ATTRACTIVE_OBJECT_INHERENT_DATA) + ".asset";
                        if (this.editorProfile.AttractiveObjectGameConfigurationManager.IsNew)
                        {
                            AssetDatabase.CreateAsset(this.editorProfile.AttractiveObjectInherentConfigurationData, this.editorProfile.ProjectRelativeTmpFolderPath + attractiveObjectFileName);
                            this.editorProfile.GeneratedObjects.Add(this.editorProfile.AttractiveObjectInherentConfigurationData);
                        }
                        #endregion

                        #region Game configuration update
                        this.editorProfile.ConfigurationRetrieval.AttractiveObjectConfiguration.SetEntry(this.editorProfile.AttractiveObjectId, this.editorProfile.AttractiveObjectInherentConfigurationData);
                        #endregion

                        #region Assets move from tmp
                        Debug.Log("Move asset");
                        AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(modelAsset), this.editorProfile.PathConfiguration.ObjectPrefabFolder + createdModelFileName);
                        AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(savedAttractiveObjectType), this.editorProfile.PathConfiguration.ObjectPrefabFolder + savedAttractiveObjectTypeFileName);
                        if (this.editorProfile.AttractiveObjectGameConfigurationManager.IsNew)
                        {
                            AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(this.editorProfile.AttractiveObjectInherentConfigurationData), this.editorProfile.PathConfiguration.ObjectConfigurationFolder + attractiveObjectFileName);
                        }
                        #endregion
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        Debug.LogError(e.StackTrace);
                    }
                    finally
                    {
                        EditorSceneManager.CloseScene(tmpScene, true);
                    }
                }

                EditorGUILayout.BeginVertical(EditorStyles.textArea);
                EditorGUILayout.LabelField("Generated objects : ");
                foreach (var generatedObject in this.editorProfile.GeneratedObjects)
                {
                    EditorGUILayout.ObjectField(generatedObject, typeof(UnityEngine.Object), false);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndScrollView();

            }
        }
    }


    public class AttractiveObjectGameConfigurationManager
    {
        private bool isNew;

        public bool IsNew { get => isNew; }

        public void OnGui(AttractiveObjectVariantCreationWizardEditorProfile editorProfile)
        {
            #region Attractive object game configuration
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            editorProfile.AttractiveObjectConfigurationFoldout = EditorGUILayout.Foldout(editorProfile.AttractiveObjectConfigurationFoldout, "Object game configuration : ", true);
            if (editorProfile.AttractiveObjectConfigurationFoldout)
            {
                if (GUILayout.Button(new GUIContent("N"), EditorStyles.miniButton, GUILayout.Width(30f)))
                {
                    editorProfile.AttractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
                    this.isNew = true;
                }
                EditorGUI.BeginChangeCheck();
                editorProfile.AttractiveObjectInherentConfigurationData = (AttractiveObjectInherentConfigurationData)EditorGUILayout.ObjectField("Generated attractive object configuration : ", editorProfile.AttractiveObjectInherentConfigurationData, typeof(AttractiveObjectInherentConfigurationData), false);
                if (EditorGUI.EndChangeCheck())
                {
                    this.isNew = false;
                }
                if (editorProfile.AttractiveObjectInherentConfigurationData != null)
                {
                    Editor.CreateEditor(editorProfile.AttractiveObjectInherentConfigurationData).OnInspectorGUI();
                }
            }

            EditorGUILayout.EndVertical();
            #endregion
        }
    }
}
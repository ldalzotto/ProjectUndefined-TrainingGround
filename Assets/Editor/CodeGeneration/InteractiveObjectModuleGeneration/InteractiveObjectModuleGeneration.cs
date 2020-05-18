﻿using Editor_GameDesigner;
using GameConfigurationID;
using RTPuzzle;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class InteractiveObjectModuleGeneration : EditorWindow
{

    [MenuItem("Generation/InteractiveObjectModuleGeneration")]
    static void Init()
    {
        InteractiveObjectModuleGeneration window = (InteractiveObjectModuleGeneration)EditorWindow.GetWindow(typeof(InteractiveObjectModuleGeneration));
        window.Show();
    }

    #region Input
    private string baseName;
    private bool isIdentified;
    #endregion

    private void OnGUI()
    {
        this.DoInput();

        if (GUILayout.Button("GENERATE SCRIPTS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    this.DoGenerateBaseDirectory();
                    this.DoGenerateModule();
                    
                    if (this.isIdentified)
                    {
                        this.UpdateInteractiveObjectInitializationObject();
                    }
                    this.UpdatePuzzleInteractiveObjectModulePrefabs();

                    InteractiveObjectModuleDefinitionCreation.GenerateScripts(this.baseName);
                    
                    this.UpdatePuzzlePrefabConfiguration();
                }
            }
        }

        if (GUILayout.Button("GENERATE ASSETS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    this.DoGenerateBasePrefab();
                }
            }

        }
    }

    private void DoInput()
    {
        this.baseName = EditorGUILayout.TextField("Base name", this.baseName);
        this.isIdentified = EditorGUILayout.Toggle("Is Identified : ", this.isIdentified);
    }

    private void DoGenerateBaseDirectory()
    {
        DirectoryInfo baseDirectory = new DirectoryInfo(PathConstants.RTPuzzleFeaturesPath + "/" + this.baseName);
        if (!baseDirectory.Exists)
        {
            baseDirectory.Create();
        }
    }

    private void DoGenerateModule()
    {
        DirectoryInfo moduleDirectory = new DirectoryInfo(PathConstants.RTPuzzleFeaturesPath + "/" + this.baseName + "/InteractiveObjectModules");
        if (!moduleDirectory.Exists)
        {
            moduleDirectory.Create();
        }

        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(InteractiveObjectType).Namespace);
        var moduleClass = new CodeTypeDeclaration(this.baseName + "ID");
        moduleClass.Name = this.baseName + "Module";
        moduleClass.IsClass = true;
        moduleClass.Attributes = MemberAttributes.Public;
        moduleClass.BaseTypes.Add(typeof(InteractiveObjectModule));

        if (this.isIdentified)
        {
            var idMember = new CodeMemberField()
            {
                Name = this.baseName + "ID"
            };
            idMember.Attributes = MemberAttributes.Public;
            idMember.Type = new CodeTypeReference(typeof(PointOfInterestId).Namespace + "." + this.baseName + "ID");
            idMember.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(CustomEnum).Name));
            moduleClass.Members.Add(idMember);

        }
        
        var initMethod = new CodeMemberMethod();
        initMethod.Name = "Init";
        initMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
        initMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(InteractiveObjectInitializationObject), typeof(InteractiveObjectInitializationObject).Name));
        initMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IInteractiveObjectTypeDataRetrieval), typeof(IInteractiveObjectTypeDataRetrieval).Name));
        initMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IInteractiveObjectTypeEvents), typeof(IInteractiveObjectTypeEvents).Name));
        moduleClass.Members.Add(initMethod);
        

        var tickMethod = new CodeMemberMethod();
        tickMethod.Name = "Tick";
        tickMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
        tickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "d"));
        tickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "timeAttenuationFactor"));
        moduleClass.Members.Add(tickMethod);
        
        samples.Types.Add(moduleClass);
        compileUnity.Namespaces.Add(samples);

        string filename = moduleDirectory.FullName + "/" + this.baseName + "Module.cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }

    }

    private void UpdateInteractiveObjectInitializationObject()
    {
        if (!typeof(InteractiveObjectInitializationObject).GetFields().ToList().ConvertAll(f => f.Name).Contains(this.baseName + "InherentData"))
        {
            CodeCompileUnit compileUnity = new CodeCompileUnit();
            CodeNamespace samples = new CodeNamespace(typeof(InteractiveObjectType).Namespace);

            var InteractiveObjectInitializationObjectCode = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(InteractiveObjectInitializationObject));
            //add the new field
            InteractiveObjectInitializationObjectCode.Members.Add(new CodeMemberField(this.baseName + "InherentData", this.baseName + "InherentData") { Attributes = MemberAttributes.Public });

            samples.Types.Add(InteractiveObjectInitializationObjectCode);
            compileUnity.Namespaces.Add(samples);

            string filename = PathConstants.InteractiveObjectInitializationObjectPath;
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(filename))
            {
                provider.GenerateCodeFromCompileUnit(
                    compileUnity, sourceWriter, options);
            }
        }
    }

    private void UpdatePuzzleInteractiveObjectModulePrefabs()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(InteractiveObjectType).Namespace);
        var puzzleInteractiveObjectModulePrefab = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(PuzzleInteractiveObjectModulePrefabs));


        //Add the new path
        bool add = true;
        foreach (var field in typeof(PuzzleInteractiveObjectModulePrefabs).GetFields())
        {
            if (field.Name == "Base" + this.baseName + "Module")
            {
                add = false;
            }
        }
        if (add)
        {
            var interactiveObjectPrefab = new CodeMemberField(this.baseName + "Module", "Base" + this.baseName + "Module");
            interactiveObjectPrefab.Attributes = MemberAttributes.Public;
            interactiveObjectPrefab.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(ReadOnly).Name));
            puzzleInteractiveObjectModulePrefab.Members.Add(interactiveObjectPrefab);
        }


        samples.Types.Add(puzzleInteractiveObjectModulePrefab);
        compileUnity.Namespaces.Add(samples);

        string filename = PathConstants.PuzzleGameConfigurationsEditorConstantsPath + "/" + typeof(PuzzleInteractiveObjectModulePrefabs).Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void UpdatePuzzlePrefabConfiguration()
    {
        var PuzzlePrefabConfigurationFile = CodeGenerationHelper.ClassFileFromType(typeof(PuzzlePrefabConfiguration));
        CodeGenerationHelper.InsertToFile(new FileInfo(PuzzlePrefabConfigurationFile.Path), "        public ${baseName}Module Base${baseName}Module;\n", "//${PuzzlePrefabConfiguration:baseInteractiveObjectPrefabs}",
                    new Dictionary<string, string>() { { "${baseName}", this.baseName } });
    }

    private void DoGenerateBasePrefab()
    {
        DirectoryInfo modulePrefabDirectory = new DirectoryInfo(PathConstants.RTPuzzleFeaturesPath + "/" + this.baseName + "/Prefabs");
        if (!modulePrefabDirectory.Exists)
        {
            modulePrefabDirectory.Create();
        }

        var tmpScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        tmpScene.name = UnityEngine.Random.Range(0, 999999).ToString();
        
        var basePrefabGenerated = new GameObject("Base" + this.baseName + "Module");
        basePrefabGenerated.AddComponent(TypeHelper.GetType("RTPuzzle." + this.baseName + "Module"));
        var savedAssed = PrefabUtility.SaveAsPrefabAsset(basePrefabGenerated, modulePrefabDirectory.FullName + "/Base" + this.baseName + "Module.prefab");

        EditorSceneManager.CloseScene(tmpScene, true);

        var PuzzlePrefabConfiguration = AssetFinder.SafeSingleAssetFind<PuzzlePrefabConfiguration>("t:" + typeof(PuzzlePrefabConfiguration));
        PuzzlePrefabConfiguration.GetType().GetFields().ToArray().Select(f => f).Where(f => f.FieldType.Name == this.baseName + "Module").First().SetValue(PuzzlePrefabConfiguration, savedAssed.GetComponent(this.baseName + "Module"));
        EditorUtility.SetDirty(PuzzlePrefabConfiguration);
    }
}

using AdventureGame;
using GameConfigurationID;
using OdinSerializer;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PointOfInterestModuleGeneration : EditorWindow
{

    [MenuItem("Generation/PointOfInterestModuleGeneration")]
    static void Init()
    {
        PointOfInterestModuleGeneration window = (PointOfInterestModuleGeneration)EditorWindow.GetWindow(typeof(PointOfInterestModuleGeneration));
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
                    this.DoGenerateModule();
                    this.GenerateDefinitionClass(baseName);
                    this.UpdateDefinitionConfigurations(baseName);
                    this.AddDefinitionCondition(baseName);
                    this.AddCustomEditorCondition(baseName);
                    this.UpdatePointOfInterestPrefabConfiguration(baseName);
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


    private void DoGenerateModule()
    {
        DirectoryInfo moduleDirectory = new DirectoryInfo(PathConstants.PointOfInterestModulePath + "/" + this.baseName + "Module");
        if (!moduleDirectory.Exists)
        {
            moduleDirectory.Create();
        }

        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PointOfInterestType).Namespace);
        var moduleClass = new CodeTypeDeclaration(this.baseName + "ID");
        moduleClass.Name = this.baseName + "Module";
        moduleClass.IsClass = true;
        moduleClass.Attributes = MemberAttributes.Public;
        moduleClass.BaseTypes.Add(typeof(APointOfInterestModule));

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
        initMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
        moduleClass.Members.Add(initMethod);

        var tickMethod = new CodeMemberMethod();
        tickMethod.Name = "Tick";
        tickMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
        tickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "d"));
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

    private void GenerateDefinitionClass(string baseName)
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PointOfInterestType).Namespace);
        var moduleClass = new CodeTypeDeclaration(this.baseName + "ModuleDefinition");
        moduleClass.Name = this.baseName + "ModuleDefinition";
        moduleClass.IsClass = true;
        moduleClass.Attributes = MemberAttributes.Public;
        moduleClass.BaseTypes.Add(typeof(SerializedScriptableObject));

        samples.Types.Add(moduleClass);
        compileUnity.Namespaces.Add(samples);

        DirectoryInfo pointOfInterestDefinitionDirectory = new DirectoryInfo(PathConstants.PointOfInterestDefinitionPath);
        string filename = pointOfInterestDefinitionDirectory.FullName + "/" + this.baseName + "ModuleDefinition.cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void UpdateDefinitionConfigurations(string baseName)
    {
        var PointOfInterestModuleTypesConstantsFile = new FileInfo(CodeGenerationHelper.ClassFileFromType(typeof(PointOfInterestModuleTypesConstants)).Path);
        CodeGenerationHelper.InsertToFile(PointOfInterestModuleTypesConstantsFile, "            typeof(" + baseName + "ModuleDefinition),\n", "//${addNewEntry}", null);
    }

    private void AddDefinitionCondition(string baseName)
    {
        var PointOfInterestDefinitionInherentDataFile = new FileInfo(CodeGenerationHelper.ClassFileFromType(typeof(PointOfInterestDefinitionInherentData)).Path);
        var PointOfInterestDefinitionConfigurationTemplate = new FileInfo(PathConstants.PointOfInterestObjectDefinitionConditionTemplatePath);
        CodeGenerationHelper.InsertToFile(PointOfInterestDefinitionInherentDataFile, PointOfInterestDefinitionConfigurationTemplate, "//${addNewEntry}",
            new Dictionary<string, string>() { { "${baseName}", baseName } });
    }

    private void AddCustomEditorCondition(string baseName)

    {
        var PointOfInterestGizmosFile = new FileInfo(CodeGenerationHelper.ClassFileFromType(typeof(PointOfInterestGizmos)).Path);
        var PointOfInterestCustomEditorDefinitionConfigurationTemplate = new FileInfo(PathConstants.PointOfInterestObjectDefinitionCustomEditorCondition);
        CodeGenerationHelper.InsertToFile(PointOfInterestGizmosFile, PointOfInterestCustomEditorDefinitionConfigurationTemplate, "//${addNewEntry}",
            new Dictionary<string, string>() { { "${baseName}", baseName } });
    }

    private void UpdatePointOfInterestPrefabConfiguration(string baseName)
    {
        var PuzzlePrefabConfigurationFile = CodeGenerationHelper.ClassFileFromType(typeof(AdventurePrefabConfiguration));
        CodeGenerationHelper.InsertToFile(new FileInfo(PuzzlePrefabConfigurationFile.Path), "        public ${baseName}Module Base${baseName}Module;\n", "//${AdventurePrefabConfiguration:basePointOfInterestModulePrefab}",
                    new Dictionary<string, string>() { { "${baseName}", baseName } });
    }

    private void DoGenerateBasePrefab()
    {
        var tmpScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        tmpScene.name = UnityEngine.Random.Range(0, 999999).ToString();

        var basePrefabGenerated = new GameObject("Base" + this.baseName + "Module");
        basePrefabGenerated.AddComponent(TypeHelper.GetType("AdventureGame." + this.baseName + "Module"));
        var savedAssed = PrefabUtility.SaveAsPrefabAsset(basePrefabGenerated, PathConstants.PointOfInterestModulePath + "/" + this.baseName + "Module/" + "Base" + this.baseName + "Module.prefab");

        EditorSceneManager.CloseScene(tmpScene, true);

        var AdventurePrefabConfiguration = AssetFinder.SafeSingleAssetFind<AdventurePrefabConfiguration>("t:" + typeof(AdventurePrefabConfiguration));
        AdventurePrefabConfiguration.GetType().GetFields().ToArray().Select(f => f).Where(f => f.FieldType.Name == this.baseName + "Module").First().SetValue(AdventurePrefabConfiguration, savedAssed.GetComponent(this.baseName + "Module"));
        EditorUtility.SetDirty(AdventurePrefabConfiguration);
    }
}

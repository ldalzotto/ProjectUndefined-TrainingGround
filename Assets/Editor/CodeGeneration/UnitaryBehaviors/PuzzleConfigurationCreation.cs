using ConfigurationEditor;
using RTPuzzle;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PuzzleConfigurationCreation : EditorWindow
{
    [MenuItem("Generation/UnitaryBehaviors/PuzzleConfigurationCreation")]
    static void Init()
    {
        PuzzleConfigurationCreation window = (PuzzleConfigurationCreation)EditorWindow.GetWindow(typeof(PuzzleConfigurationCreation));
        window.Show();
    }

    private string baseName;

    private void OnGUI()
    {
        this.baseName = EditorGUILayout.TextField("BaseName : ", this.baseName);

        if (GUILayout.Button("GENERATE SCRIPTS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    CommonCodeGeneration.CreatePuzzleSubConfigurationFolderIfNecessary(this.baseName);
                    DoGenerateInherentData(this.baseName);
                    DoGenerateConfiguration(this.baseName);
                    UpdatePuzzleGameConfiguration(this.baseName);
                    DoGenerateConfigurationGameDesignerModule(this.baseName);
                }
            }
        }
        if (GUILayout.Button("GENERATE ASSETS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    DoCreateConfigurationAsset(this.baseName);
                }
            }
        }


    }

    public static void DoGenerateInherentData(string baseName)
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PuzzleEventsManager).Namespace);
        samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));
        CodeTypeDeclaration inherentDataClass = new CodeTypeDeclaration(baseName + "InherentData");
        inherentDataClass.IsClass = true;
        inherentDataClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
        inherentDataClass.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
        inherentDataClass.CustomAttributes.Add(CodeGenerationHelper.GenerateCreateAssetMenuAttribute(inherentDataClass.Name, "Configuration/PuzzleGame/" + baseName + "Configuration/" + inherentDataClass.Name));
        inherentDataClass.BaseTypes.Add(typeof(ScriptableObject).Name);

        var interactiveObjectReference = new CodeMemberField(typeof(InteractiveObjectType), "AssociatedInteractiveObjectType");
        interactiveObjectReference.Attributes = MemberAttributes.Public;
        inherentDataClass.Members.Add(interactiveObjectReference);

        samples.Types.Add(inherentDataClass);
        compileUnity.Namespaces.Add(samples);

        string filename = PathConstants.PuzzleSubConfigurationFolderPath + "/" + baseName + "Configuration/" + inherentDataClass.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    public static void DoGenerateConfiguration(string baseName)
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PuzzleEventsManager).Namespace);
        samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));
        samples.Imports.Add(new CodeNamespaceImport("ConfigurationEditor"));
        samples.Imports.Add(new CodeNamespaceImport("GameConfigurationID"));
        samples.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

        var configurationClass = new CodeTypeDeclaration(baseName + "Configuration");
        configurationClass.IsClass = true;
        configurationClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
        configurationClass.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
        configurationClass.CustomAttributes.Add(CodeGenerationHelper.GenerateCreateAssetMenuAttribute(configurationClass.Name, "Configuration/PuzzleGame/" + configurationClass.Name + "/" + configurationClass.Name));
        var configurationType = new CodeTypeReference(typeof(ConfigurationSerialization<,>));
        configurationType.TypeArguments.Add(baseName + "ID");
        configurationType.TypeArguments.Add(baseName + "InherentData");
        configurationClass.BaseTypes.Add(configurationType);

        samples.Types.Add(configurationClass);
        compileUnity.Namespaces.Add(samples);

        string filename = "Configuration/PuzzleGame/" + baseName + "Configuration/" + configurationClass.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }

        var configurationDataDirectory = new DirectoryInfo("Configuration/PuzzleGame/" + baseName + "Configuration/Data");
        if (!configurationDataDirectory.Exists)
        {
            configurationDataDirectory.Create();
        }
    }

    public static void UpdatePuzzleGameConfiguration(string baseName)
    {
        #region PuzzleGameConfiguration
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PuzzleGameConfiguration).Namespace);

        var generatedPuzzleGameConfiguration = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(PuzzleGameConfiguration));

        //Add the new configuration
        bool add = true;
        foreach (var field in typeof(PuzzleGameConfiguration).GetFields())
        {
            if (field.Name == (baseName + "Configuration"))
            {
                add = false;
            }
        }
        if (add)
        {
            var newManagerAddedField = new CodeMemberField(baseName + "Configuration", baseName + "Configuration");
            newManagerAddedField.Attributes = MemberAttributes.Public;
            generatedPuzzleGameConfiguration.Members.Add(newManagerAddedField);
        }

        samples.Types.Add(generatedPuzzleGameConfiguration);
        compileUnity.Namespaces.Add(samples);

        string filename = PathConstants.PuzzleConfigurationFolderPath + "/" + generatedPuzzleGameConfiguration.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
        #endregion
        #region PuzzleGameConfigurationManager
        CodeGenerationHelper.InsertToFile(new FileInfo(PathConstants.PuzzleGameConfigurationManagerPath), new FileInfo(PathConstants.PuzzleGameConfigurationManagerMethodTemplatePath), "//${addNewEntry}",
                new Dictionary<string, string>() { { "${baseName}", baseName } },
                insertionGuard: (file) => !file.Contains(baseName + "Configuration"));
        #endregion
    }

    public static void DoGenerateConfigurationGameDesignerModule(string baseName)
    {
        string gameDesignerConfigurationModulesFile = File.ReadAllText(PathConstants.GameDesignerConfigurationModulesPath);

        if (!gameDesignerConfigurationModulesFile.Contains(baseName + "ConfigurationModule"))
        {
            string configurationToAdd = configurationToAdd = CodeGenerationHelper.ApplyStringParameters(File.ReadAllText(PathConstants.GameDesignerConfigurationModuleTemplatepath), new Dictionary<string, string>() {
              {"${baseName}", baseName }
            });

            gameDesignerConfigurationModulesFile =
               gameDesignerConfigurationModulesFile.Insert(gameDesignerConfigurationModulesFile.IndexOf("//${addNewEntry}"), configurationToAdd);

            File.WriteAllText(PathConstants.GameDesignerConfigurationModulesPath, gameDesignerConfigurationModulesFile);

            CodeGenerationHelper.AddGameDesignerChoiceTree(new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("Configuration//." + baseName + "ConfigurationModule",  "Editor_GameDesigner." + baseName + "ConfigurationModule")
            });

        }
    }

    public static void DoCreateConfigurationAsset(string baseName)
    {
        var configurationSO = ScriptableObject.CreateInstance(baseName + "Configuration");
        AssetDatabase.CreateAsset(configurationSO, PathConstants.PuzzleSubConfigurationFolderPath + "/" + baseName + "Configuration/" + baseName + "Configuration.asset");
        var PuzzleGameConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleGameConfiguration>("t:" + typeof(PuzzleGameConfiguration));
        var configurationField = PuzzleGameConfiguration.GetType().GetField(baseName + "Configuration");
        configurationField.SetValue(PuzzleGameConfiguration, configurationSO);
        EditorUtility.SetDirty(PuzzleGameConfiguration);

    }
}

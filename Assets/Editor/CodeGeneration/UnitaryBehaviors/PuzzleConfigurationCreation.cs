using ConfigurationEditor;
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
    private GameTypeGeneration GameTypeGeneration;

    private void OnGUI()
    {
        this.GameTypeGeneration = (GameTypeGeneration)EditorGUILayout.EnumPopup(this.GameTypeGeneration);
        this.baseName = EditorGUILayout.TextField("BaseName : ", this.baseName);

        if (GUILayout.Button("GENERATE SCRIPTS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    CommonCodeGeneration.CreateModuleConfigurationFolderIfNecessary(this.baseName, this.GameTypeGeneration);
                    DoGenerateInherentData(this.baseName, this.GameTypeGeneration);
                    DoGenerateConfiguration(this.baseName, this.GameTypeGeneration);
                    UpdateGameConfiguration(this.baseName, this.GameTypeGeneration);
                }
            }
        }
        if (GUILayout.Button("GENERATE ASSETS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    DoCreateConfigurationAsset(this.baseName, this.GameTypeGeneration);
                }
            }
        }


    }

    public static void DoGenerateInherentData(string baseName, GameTypeGeneration GameTypeGeneration)
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetNamespace();
        samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));
        CodeTypeDeclaration inherentDataClass = new CodeTypeDeclaration(baseName + "InherentData");
        inherentDataClass.IsClass = true;
        inherentDataClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
        inherentDataClass.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
        inherentDataClass.CustomAttributes.Add(CodeGenerationHelper.GenerateCreateAssetMenuAttribute(inherentDataClass.Name, GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetConfigurationAssetMenuAttributeName(baseName, inherentDataClass.Name)));
        inherentDataClass.BaseTypes.Add(typeof(ScriptableObject).Name);

        samples.Types.Add(inherentDataClass);
        compileUnity.Namespaces.Add(samples);

        string filename = GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetSubConfigurationFolderPath(baseName) + "/" + inherentDataClass.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    public static void DoGenerateConfiguration(string baseName, GameTypeGeneration GameTypeGeneration)
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetNamespace();
        samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));
        samples.Imports.Add(new CodeNamespaceImport("ConfigurationEditor"));
        samples.Imports.Add(new CodeNamespaceImport("GameConfigurationID"));
        samples.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

        var configurationClass = new CodeTypeDeclaration(baseName + "Configuration");
        configurationClass.IsClass = true;
        configurationClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
        configurationClass.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
        configurationClass.CustomAttributes.Add(CodeGenerationHelper.GenerateCreateAssetMenuAttribute(configurationClass.Name, GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetConfigurationAssetMenuAttributeName(baseName, configurationClass.Name)));
        var configurationType = new CodeTypeReference(typeof(ConfigurationSerialization<,>));
        configurationType.TypeArguments.Add(baseName + "ID");
        configurationType.TypeArguments.Add(baseName + "InherentData");
        configurationClass.BaseTypes.Add(configurationType);

        samples.Types.Add(configurationClass);
        compileUnity.Namespaces.Add(samples);

        string filename = GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetSubConfigurationFolderPath(baseName) + "/" + configurationClass.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }

        var configurationDataDirectory = new DirectoryInfo(PathConstants.PuzzleSubConfigurationFolderPath + "/" + baseName + "Configuration/Data");
        if (!configurationDataDirectory.Exists)
        {
            configurationDataDirectory.Create();
        }
    }

    public static void UpdateGameConfiguration(string baseName, GameTypeGeneration GameTypeGeneration)
    {
        #region PuzzleGameConfiguration
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetNamespace();

        var generatedConfiguration = CopyConfigurationClass(GameTypeGeneration);

        //Add the new configuration
        bool add = true;
        foreach (var field in GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetConfigurationType().GetFields())
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
            generatedConfiguration.Members.Add(newManagerAddedField);
        }

        samples.Types.Add(generatedConfiguration);
        compileUnity.Namespaces.Add(samples);

        string filename = GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetConfigurationFolderPath() + "/" + generatedConfiguration.Name + ".cs";
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
        CodeGenerationHelper.InsertToFile(new FileInfo(GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetGameConfigurationManagerPath()),
            new FileInfo(GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetGameConfigurationManagerMethodTemplatePath()), "//${addNewEntry}",
                new Dictionary<string, string>() { { "${baseName}", baseName } },
                insertionGuard: (file) => !file.Contains(baseName + "Configuration"));
        #endregion
    }

    public static void DoCreateConfigurationAsset(string baseName, GameTypeGeneration GameTypeGeneration)
    {
        var configurationSO = ScriptableObject.CreateInstance(baseName + "Configuration");
        AssetDatabase.CreateAsset(configurationSO, GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetSubConfigurationFolderPath(baseName) + "/" + baseName + "Configuration.asset");
        var PuzzleGameConfiguration = AssetFinder.SafeSingleAssetFind<UnityEngine.Object>("t:" + GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetConfigurationType());
        var configurationField = PuzzleGameConfiguration.GetType().GetField(baseName + "Configuration");
        configurationField.SetValue(PuzzleGameConfiguration, configurationSO);
        EditorUtility.SetDirty(PuzzleGameConfiguration);

    }


    private static CodeTypeDeclaration CopyConfigurationClass(GameTypeGeneration GameTypeGeneration)
    {
        return CodeGenerationHelper.CopyClassAndFieldsFromExistingType(GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetConfigurationType());
    }
}

using Editor_MainGameCreationWizard;
using RTPuzzle;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CreationWizardCreation : EditorWindow
{
    [MenuItem("Generation/UnitaryBehaviors/CreationWizardCreation")]
    static void Init()
    {
        CreationWizardCreation window = (CreationWizardCreation)EditorWindow.GetWindow(typeof(CreationWizardCreation));
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
                    CreationWizardCreation.DoGenerateCreationWizardScripts(this.baseName, this.GameTypeGeneration);
                }
            }
        }
        if (GUILayout.Button("GENERATE ASSETS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    CreationWizardCreation.DoGenerateCreationWizardScriptsAssets(this.baseName);
                }
            }
        }
    }

    public static void DoGenerateCreationWizardScripts(string baseName, GameTypeGeneration GameTypeGeneration)
    {
        UpdatePuzzleGameConfigurationsEditorConstants(baseName, GameTypeGeneration);
        UpdateInstancePathEditorConstants(baseName, GameTypeGeneration);
        UpdateNameConstantsEditorConstant(baseName);
        DoGenerateEditorCreation(baseName, GameTypeGeneration);
    }

    public static void DoGenerateCreationWizardScriptsAssets(string baseName)
    {
        RegenerateGameCreationWizardEditorProfile();
        DoGenerateCreationWizardProfile(baseName);
    }

    private static void UpdatePuzzleGameConfigurationsEditorConstants(string baseName, GameTypeGeneration GameTypeGeneration)
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetNamespace();

        var generatedPuzzleGameConfigurations = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetEditorConfigurationsType());

        //Add the new configuration
        bool add = true;
        foreach (var field in GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetEditorConfigurationsType().GetFields())
        {
            if (field.Name == baseName + "Configuration")
            {
                add = false;
            }
        }
        if (add)
        {
            var newManagerAddedField = new CodeMemberField(GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetNamespace().Name + "." + baseName + "Configuration", baseName + "Configuration");
            newManagerAddedField.Attributes = MemberAttributes.Public;
            newManagerAddedField.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(ReadOnly).Name));
            generatedPuzzleGameConfigurations.Members.Add(newManagerAddedField);
        }

        samples.Types.Add(generatedPuzzleGameConfigurations);
        compileUnity.Namespaces.Add(samples);

        string filename = PathConstants.PuzzleGameConfigurationsEditorConstantsPath + "/" + generatedPuzzleGameConfigurations.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private static void UpdateInstancePathEditorConstants(string baseName, GameTypeGeneration GameTypeGeneration)
    {
        var puzzleConfigurationFodler = CommonCodeGeneration.CreatePuzzleSubConfigurationFolderIfNecessary(baseName, GameTypeGeneration);

        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PuzzleGameConfigurations).Namespace);
        var generatedInstancePath = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(InstancePath));


        //Add the new path
        bool add = true;
        foreach (var field in typeof(InstancePath).GetFields())
        {
            if (field.Name == (baseName + "InherentDataPath"))
            {
                add = false;
            }
        }
        if (add)
        {
            var newPathAddedField = new CodeMemberField(typeof(string), baseName + "InherentDataPath");
            newPathAddedField.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            newPathAddedField.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(ReadOnly).Name));
            newPathAddedField.InitExpression = new CodePrimitiveExpression((puzzleConfigurationFodler.FullName.Substring(puzzleConfigurationFodler.FullName.IndexOf("Asset")) + "/Data").Replace("\\", "/"));
            generatedInstancePath.Members.Add(newPathAddedField);
        }

        samples.Types.Add(generatedInstancePath);
        compileUnity.Namespaces.Add(samples);

        string filename = PathConstants.PuzzleGameConfigurationsEditorConstantsPath + "/" + generatedInstancePath.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private static void UpdateNameConstantsEditorConstant(string baseName)
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(NameConstants).Namespace);
        var generatedNameConstants = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(NameConstants));


        //Add the new path
        bool add = true;
        foreach (var field in typeof(NameConstants).GetFields())
        {
            if (field.Name == (baseName + "InherentDataPath"))
            {
                add = false;
            }
        }
        if (add)
        {
            var newPathAddedField = new CodeMemberField(typeof(string), baseName + "InherentData");
            newPathAddedField.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            newPathAddedField.InitExpression = new CodePrimitiveExpression("_" + baseName);
            generatedNameConstants.Members.Add(newPathAddedField);
        }

        samples.Types.Add(generatedNameConstants);
        compileUnity.Namespaces.Add(samples);

        string filename = PathConstants.PuzzleGameConfigurationsEditorConstantsPath2 + "/" + generatedNameConstants.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private static void DoGenerateEditorCreation(string baseName, GameTypeGeneration GameTypeGeneration)
    {
        DirectoryInfo sourceTemplateDirectory = new DirectoryInfo(PathConstants.CodeGenerationCreationWizardBasincConfigurationCreationTemplatePath);
        if (sourceTemplateDirectory.Exists)
        {

            DirectoryInfo targetDirectory = new DirectoryInfo(PathConstants.EditorCreationWizardFolderPath + "/" + baseName);
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            CodeGenerationHelper.DuplicateDirectoryWithParamtersRecursive(sourceTemplateDirectory, targetDirectory, new Dictionary<string, string>() {
                { "${baseName}", baseName},
                {"${namespaceName}", GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetNamespace().Name },
                {"${editorGameConfigurationsName}",GameTypeCodeGenerationConfiguration.Get(GameTypeGeneration).GetEditorConfigurationsType().Name }
            });
        }
    }

    private static void RegenerateGameCreationWizardEditorProfile()
    {
        var path = AssetDatabase.GetAssetPath(AssetFinder.SafeSingleAssetFind<GameCreationWizardEditorProfile>("t:" + typeof(GameCreationWizardEditorProfile).Name));
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(typeof(GameCreationWizardEditorProfile)), path);
    }

    private static void DoGenerateCreationWizardProfile(string baseName)
    {
        var creationWizardProfileSO = ScriptableObject.CreateInstance(baseName + "CreationWizardProfile");
        AssetDatabase.CreateAsset(creationWizardProfileSO, PathConstants.EditorCreationWizardFolderPath + "/" + baseName + "/" + baseName + "CreationWizardProfile.asset");
    }
}

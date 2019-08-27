using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.CodeDom;
using System.Linq;
using System.CodeDom.Compiler;
using Editor_MainGameCreationWizard;
using UnityEditor;

public class CreationWizardCreation : EditorWindow
{
    [MenuItem("Generation/UnitaryBehaviors/CreationWizardCreation")]
    static void Init()
    {
        CreationWizardCreation window = (CreationWizardCreation)EditorWindow.GetWindow(typeof(CreationWizardCreation));
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
                    CreationWizardCreation.DoGenerateCreationWizardScripts(this.baseName);
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

    public static void DoGenerateCreationWizardScripts(string baseName)
    {
        UpdatePuzzleGameConfigurationsEditorConstants(baseName);
        UpdateInstancePathEditorConstants(baseName);
        UpdateNameConstantsEditorConstant(baseName);
        DoGenerateEditorCreation(baseName);
        UpdateGameCreationWizardEditorProfileChoiceTree(baseName);
    }

    public static void DoGenerateCreationWizardScriptsAssets(string baseName)
    {
        RegenerateGameCreationWizardEditorProfile();
        DoGenerateCreationWizardProfile(baseName);
    }

    private static void UpdatePuzzleGameConfigurationsEditorConstants(string baseName)
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PuzzleGameConfigurations).Namespace);

        var generatedPuzzleGameConfigurations = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(PuzzleGameConfigurations));

        //Add the new configuration
        bool add = true;
        foreach (var field in typeof(PuzzleGameConfigurations).GetFields())
        {
            if (field.Name == baseName + "Configuration")
            {
                add = false;
            }
        }
        if (add)
        {
            var newManagerAddedField = new CodeMemberField("RTPuzzle" + "." + baseName + "Configuration", baseName + "Configuration");
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

    private static void UpdateInstancePathEditorConstants(string baseName)
    {
        var puzzleConfigurationFodler = CommonCodeGeneration.CreatePuzzleSubConfigurationFolderIfNecessary(baseName);

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
    
    private static void DoGenerateEditorCreation(string baseName)
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
                { "${baseName}", baseName}
            });
        }
    }

    private static void UpdateGameCreationWizardEditorProfileChoiceTree(string baseName)
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(GameCreationWizardEditorProfileChoiceTree).Namespace);
        var generatedCreationWizardEditorProfileChoiceTree = new CodeTypeDeclaration(typeof(GameCreationWizardEditorProfileChoiceTree).Name);

        var configurationTreeFieldName = nameof(GameCreationWizardEditorProfileChoiceTree.configurations);
        var configurationTreeField = new CodeMemberField(typeof(GameCreationWizardEditorProfileChoiceTree).GetField(configurationTreeFieldName).FieldType, configurationTreeFieldName);
        configurationTreeField.Attributes = MemberAttributes.Public | MemberAttributes.Static;
        var configurationTreeFieldDic = GameCreationWizardEditorProfileChoiceTree.configurations.ToList()
            .ConvertAll(kv => new KeyValuePair<string, string>("nameof(" + kv.Value.GetType().FullName + ")", "new " + kv.Value.GetType().FullName + "()"))
              .Union(new List<KeyValuePair<string, string>>() {
                  new KeyValuePair<string, string>("nameof(Editor_" + baseName + "CreationWizard." + baseName + "CreationWizard)", "new Editor_" + baseName + "CreationWizard." + baseName + "CreationWizard()") })
            .GroupBy(kv => kv.Key)
            .ToDictionary(kv => kv.Key, kv => kv.First().Value);
        configurationTreeField.InitExpression = new CodeSnippetExpression("new System.Collections.Generic.Dictionary<string, ICreationWizardEditor>()" +
            CodeGenerationHelper.FormatDictionaryToCodeSnippet(configurationTreeFieldDic));
        generatedCreationWizardEditorProfileChoiceTree.Members.Add(configurationTreeField);


        samples.Types.Add(generatedCreationWizardEditorProfileChoiceTree);
        compileUnity.Namespaces.Add(samples);

        string filename = PathConstants.PuzzleGameConfigurationsEditorPath + "/" + generatedCreationWizardEditorProfileChoiceTree.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
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

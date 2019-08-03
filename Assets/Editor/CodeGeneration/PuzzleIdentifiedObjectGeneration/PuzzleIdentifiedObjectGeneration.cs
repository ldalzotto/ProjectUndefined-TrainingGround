using ConfigurationEditor;
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PuzzleIdentifiedObjectGeneration : EditorWindow
{
    private const string IDBasePath = "Assets/@GameConfigurationID/IDs";
    private const string PuzzleConfigurationFolderPath = "Assets/~RTPuzzleGame/Configuration";
    private const string PuzzleSubConfigurationFolderPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration";
    private const string PuzzleGameConfigurationsEditorConstantsPath = "Assets/Editor/CreationWizard_PuzzleGame/Common";
    private const string PuzzleGameConfigurationsEditorConstantsPath2 = "Assets/Editor/CreationWizard_PuzzleGame/Constants";
    private const string PuzzleGameConfigurationsEditorPath = "Assets/Editor/CreationWizard_PuzzleGame";
    private const string EditorCreationWizardFolderPath = "Assets/Editor/CreationWizard_PuzzleGame/ObjectsCreation";
    private const string GameDesignerModulesPath = "Assets/Editor/GameDesigner/Modules";
    private const string GameDesignerConfigurationModulesPath = "Assets/Editor/GameDesigner/Modules/Configurations/ConfigurationsModule.cs";
    private const string PuzzleGameConfigurationManagerPath = "Assets/~RTPuzzleGame/Configuration/PuzzleGameConfigurationManager.cs";
    
    private const string CodeGenerationCreationWizardBasincConfigurationCreationTemplatePath = "Assets/Editor/CodeGeneration/Templates/CreationWizardBasicConfigurationCreation";
    private const string CodeGenrationGameDesignerConfigurationCreationTemplatePath = "Assets/Editor/CodeGeneration/Templates/GameDesignerCreationModule";
    private const string GameDesignerConfigurationModuleTemplatepath = "Assets/Editor/CodeGeneration/Templates/GameDesignerTemplates/GameDesignerConfigurationModuleTemplate.txt";
    private const string PuzzleGameConfigurationManagerMethodTemplatePath = "Assets/Editor/CodeGeneration/Templates/PuzzleGameConfiguration/PuzzleGameConfigurationManagerMethodTemplate.txt";

    [MenuItem("Generation/PuzzleIdentifiedObjectGeneration")]
    static void Init()
    {
        PuzzleIdentifiedObjectGeneration window = (PuzzleIdentifiedObjectGeneration)EditorWindow.GetWindow(typeof(PuzzleIdentifiedObjectGeneration));
        window.Show();
    }

    private string baseName;

    private DirectoryInfo puzzleConfigurationFodler;
    private CodeTypeDeclaration idEnumClass;
    private CodeTypeDeclaration inherentDataClass;
    private CodeTypeDeclaration configurationClass;

    private void OnGUI()
    {
        this.DoInput();

        if (GUILayout.Button("GENERATE SCRIPTS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    this.DoGenerateID();
                    this.CreatePuzzleSubConfigurationFolderIfNecessary();
                    this.DoGenerateInherentData();
                    this.DoGenerateConfiguration();
                    this.UpdatePuzzleGameConfigurationsEditorConstants();
                    this.UpdatePuzzleGameConfiguration();
                    this.UpdateInstancePathEditorConstants();
                    this.UpdateNameConstantsEditorConstant();
                    this.DoGenerateEditorCreation();
                    this.UpdateGameCreationWizardEditorProfileChoiceTree();
                    this.DoGenerateCreateGameDesignerModule();
                    this.DoGenerateConfigurationGameDesignerModule();
                    this.UpdateGameDesignerChoiceTree();
                }
            } 
        }

        if (GUILayout.Button("GENERATE ASSETS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    this.DoCreateConfigurationAsset();
                    this.DoGenerateCreationWizardProfile();
                }
            }  
        }
    }

    private void DoInput()
    {
        this.baseName = EditorGUILayout.TextField("BaseName : ", this.baseName);
    }

    private void DoGenerateID()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PointOfInterestId).Namespace);
        this.idEnumClass = new CodeTypeDeclaration(this.baseName + "ID");
        this.idEnumClass.IsEnum = true;
        this.idEnumClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
        this.idEnumClass.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));

        this.idEnumClass.Members.Add(new CodeMemberField()
        {
            Name = "NONE",
            InitExpression = new CodePrimitiveExpression(0)
        });

        samples.Types.Add(this.idEnumClass);
        compileUnity.Namespaces.Add(samples);

        string filename = IDBasePath + "/" + this.idEnumClass.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void CreatePuzzleSubConfigurationFolderIfNecessary()
    {
        this.puzzleConfigurationFodler = new DirectoryInfo(PuzzleSubConfigurationFolderPath + "/" + this.baseName + "Configuration");
        if (!puzzleConfigurationFodler.Exists)
        {
            puzzleConfigurationFodler.Create();
        }
    }

    private void DoGenerateInherentData()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PuzzleEventsManager).Namespace);
        samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));

        this.inherentDataClass = new CodeTypeDeclaration(this.baseName + "InherentData");
        this.inherentDataClass.IsClass = true;
        this.inherentDataClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
        this.inherentDataClass.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
        this.inherentDataClass.CustomAttributes.Add(CodeGenerationHelper.GenerateCreateAssetMenuAttribute(this.inherentDataClass.Name, "Configuration/PuzzleGame/" + this.baseName + "Configuration/" + this.inherentDataClass.Name));
        this.inherentDataClass.BaseTypes.Add(typeof(ScriptableObject).Name);

        var interactiveObjectReference = new CodeMemberField(typeof(InteractiveObjectType), "AssociatedInteractiveObjectType");
        interactiveObjectReference.Attributes = MemberAttributes.Public;
        this.inherentDataClass.Members.Add(interactiveObjectReference);

        samples.Types.Add(this.inherentDataClass);
        compileUnity.Namespaces.Add(samples);

        string filename = puzzleConfigurationFodler.FullName + "/" + this.inherentDataClass.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void DoGenerateConfiguration()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PuzzleEventsManager).Namespace);
        samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));
        samples.Imports.Add(new CodeNamespaceImport("ConfigurationEditor"));
        samples.Imports.Add(new CodeNamespaceImport("GameConfigurationID"));
        samples.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

        this.configurationClass = new CodeTypeDeclaration(this.baseName + "Configuration");
        this.configurationClass.IsClass = true;
        this.configurationClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
        this.configurationClass.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
        this.configurationClass.CustomAttributes.Add(CodeGenerationHelper.GenerateCreateAssetMenuAttribute(this.configurationClass.Name, "Configuration/PuzzleGame/" + this.configurationClass.Name + "/" + this.configurationClass.Name));
        var configurationType = new CodeTypeReference(typeof(ConfigurationSerialization<,>));
        configurationType.TypeArguments.Add(this.idEnumClass.Name);
        configurationType.TypeArguments.Add(this.inherentDataClass.Name);
        this.configurationClass.BaseTypes.Add(configurationType);

        samples.Types.Add(this.configurationClass);
        compileUnity.Namespaces.Add(samples);

        string filename = puzzleConfigurationFodler.FullName + "/" + this.configurationClass.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }

        var configurationDataDirectory = new DirectoryInfo(puzzleConfigurationFodler.FullName + "/Data");
        if (!configurationDataDirectory.Exists)
        {
            configurationDataDirectory.Create();
        }
    }

    private void UpdatePuzzleGameConfigurationsEditorConstants()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PuzzleGameConfigurations).Namespace);

        var generatedPuzzleGameConfigurations = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(PuzzleGameConfigurations));

        //Add the new configuration
        bool add = true;
        foreach (var field in typeof(PuzzleGameConfigurations).GetFields())
        {
            if (field.Name == this.configurationClass.Name)
            {
                add = false;
            }
        }
        if (add)
        {
            var newManagerAddedField = new CodeMemberField("RTPuzzle" + "." + this.configurationClass.Name, this.configurationClass.Name);
            newManagerAddedField.Attributes = MemberAttributes.Public;
            newManagerAddedField.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(ReadOnly).Name));
            generatedPuzzleGameConfigurations.Members.Add(newManagerAddedField);
        }

        samples.Types.Add(generatedPuzzleGameConfigurations);
        compileUnity.Namespaces.Add(samples);

        string filename = PuzzleGameConfigurationsEditorConstantsPath + "/" + generatedPuzzleGameConfigurations.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void UpdatePuzzleGameConfiguration()
    {
        #region PuzzleGameConfiguration
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PuzzleGameConfiguration).Namespace);

        var generatedPuzzleGameConfiguration = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(PuzzleGameConfiguration));

        //Add the new configuration
        bool add = true;
        foreach (var field in typeof(PuzzleGameConfiguration).GetFields())
        {
            if (field.Name == this.configurationClass.Name)
            {
                add = false;
            }
        }
        if (add)
        {
            var newManagerAddedField = new CodeMemberField(this.configurationClass.Name, this.configurationClass.Name);
            newManagerAddedField.Attributes = MemberAttributes.Public;
            generatedPuzzleGameConfiguration.Members.Add(newManagerAddedField);
        }

        samples.Types.Add(generatedPuzzleGameConfiguration);
        compileUnity.Namespaces.Add(samples);

        string filename = PuzzleConfigurationFolderPath + "/" + generatedPuzzleGameConfiguration.Name + ".cs";
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
        CodeGenerationHelper.InsertToFile(new FileInfo(PuzzleGameConfigurationManagerPath), new FileInfo(PuzzleGameConfigurationManagerMethodTemplatePath), "//${addNewEntry}",
                new Dictionary<string, string>() { { "${baseName}", this.baseName } },
                insertionGuard: (file) => !file.Contains(this.baseName + "Configuration"));
        #endregion
    }

    private void UpdateInstancePathEditorConstants()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(PuzzleGameConfigurations).Namespace);
        var generatedInstancePath = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(InstancePath));


        //Add the new path
        bool add = true;
        foreach (var field in typeof(InstancePath).GetFields())
        {
            if (field.Name == this.inherentDataClass.Name + "Path")
            {
                add = false;
            }
        }
        if (add)
        {
            var newPathAddedField = new CodeMemberField(typeof(string), this.inherentDataClass.Name + "Path");
            newPathAddedField.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            newPathAddedField.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(ReadOnly).Name));
            newPathAddedField.InitExpression = new CodePrimitiveExpression((this.puzzleConfigurationFodler.FullName.Substring(this.puzzleConfigurationFodler.FullName.IndexOf("Asset")) + "/Data").Replace("\\", "/"));
            generatedInstancePath.Members.Add(newPathAddedField);
        }

        samples.Types.Add(generatedInstancePath);
        compileUnity.Namespaces.Add(samples);

        string filename = PuzzleGameConfigurationsEditorConstantsPath + "/" + generatedInstancePath.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void UpdateNameConstantsEditorConstant()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(NameConstants).Namespace);
        var generatedNameConstants = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(NameConstants));


        //Add the new path
        bool add = true;
        foreach (var field in typeof(NameConstants).GetFields())
        {
            if (field.Name == this.inherentDataClass.Name)
            {
                add = false;
            }
        }
        if (add)
        {
            var newPathAddedField = new CodeMemberField(typeof(string), this.inherentDataClass.Name);
            newPathAddedField.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            newPathAddedField.InitExpression = new CodePrimitiveExpression("_" + this.baseName);
            generatedNameConstants.Members.Add(newPathAddedField);
        }

        samples.Types.Add(generatedNameConstants);
        compileUnity.Namespaces.Add(samples);

        string filename = PuzzleGameConfigurationsEditorConstantsPath2 + "/" + generatedNameConstants.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void DoGenerateEditorCreation()
    {
        DirectoryInfo sourceTemplateDirectory = new DirectoryInfo(CodeGenerationCreationWizardBasincConfigurationCreationTemplatePath);
        if (sourceTemplateDirectory.Exists)
        {

            DirectoryInfo targetDirectory = new DirectoryInfo(EditorCreationWizardFolderPath + "/" + this.baseName);
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            DuplicateDirectoryWithParamtersRecursive(sourceTemplateDirectory, targetDirectory, new Dictionary<string, string>() {
                { "${baseName}", this.baseName}
            });
        }
    }

    private void UpdateGameCreationWizardEditorProfileChoiceTree()
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
                  new KeyValuePair<string, string>("nameof(Editor_" + this.baseName + "CreationWizard." + this.baseName + "CreationWizard)", "new Editor_" + this.baseName + "CreationWizard." + this.baseName + "CreationWizard()") })
            .GroupBy(kv => kv.Key)
            .ToDictionary(kv => kv.Key, kv => kv.First().Value);
        configurationTreeField.InitExpression = new CodeSnippetExpression("new System.Collections.Generic.Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>>()" +
            CodeGenerationHelper.FormatDictionaryToCodeSnippet(configurationTreeFieldDic));
        generatedCreationWizardEditorProfileChoiceTree.Members.Add(configurationTreeField);


        samples.Types.Add(generatedCreationWizardEditorProfileChoiceTree);
        compileUnity.Namespaces.Add(samples);

        string filename = PuzzleGameConfigurationsEditorPath + "/" + generatedCreationWizardEditorProfileChoiceTree.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void DoGenerateCreateGameDesignerModule()
    {
        DirectoryInfo sourceTemplateDirectory = new DirectoryInfo(CodeGenrationGameDesignerConfigurationCreationTemplatePath);
        if (sourceTemplateDirectory.Exists)
        {

            DirectoryInfo targetDirectory = new DirectoryInfo(GameDesignerModulesPath + "/" + this.baseName);
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            DuplicateDirectoryWithParamtersRecursive(sourceTemplateDirectory, targetDirectory, new Dictionary<string, string>() {
                { "${baseName}", this.baseName}
            });
        }

    }

    private void DoGenerateConfigurationGameDesignerModule()
    {
        string gameDesignerConfigurationModulesFile = File.ReadAllText(GameDesignerConfigurationModulesPath);

        if (!gameDesignerConfigurationModulesFile.Contains(this.baseName + "ConfigurationModule"))
        {
            string configurationToAdd = configurationToAdd = CodeGenerationHelper.ApplyStringParameters(File.ReadAllText(GameDesignerConfigurationModuleTemplatepath), new Dictionary<string, string>() {
              {"${baseName}", this.baseName }
            });

            gameDesignerConfigurationModulesFile =
               gameDesignerConfigurationModulesFile.Insert(gameDesignerConfigurationModulesFile.IndexOf("//${addNewEntry}"), configurationToAdd);

            File.WriteAllText(GameDesignerConfigurationModulesPath, gameDesignerConfigurationModulesFile);
        }
    }

    private void UpdateGameDesignerChoiceTree()
    {
        CodeGenerationHelper.AddGameDesignerChoiceTree(new List<KeyValuePair<string, string>>() {
            new KeyValuePair<string, string>("Puzzle//" + this.baseName + "//.Create" + this.baseName, "Editor_GameDesigner.Create" + this.baseName),
            new KeyValuePair<string, string>("Configuration//." + this.baseName + "ConfigurationModule",  "Editor_GameDesigner." + this.baseName + "ConfigurationModule")
        });
    }

    private static void DuplicateDirectoryWithParamtersRecursive(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory, Dictionary<string, string> parameters)
    {
        if (!targetDirectory.Exists)
        {
            targetDirectory.Create();
        }

        foreach (var fileToCopy in sourceDirectory.GetFiles())
        {
            CodeGenerationHelper.CopyFile(targetDirectory, parameters, fileToCopy);
        }

        foreach (var directoryToCopy in sourceDirectory.GetDirectories())
        {
            DuplicateDirectoryWithParamtersRecursive(directoryToCopy, new DirectoryInfo(targetDirectory.FullName + "/" + directoryToCopy.Name), parameters);
        }
    }

    private void DoCreateConfigurationAsset()
    {
        var configurationSO = ScriptableObject.CreateInstance(this.baseName + "Configuration");
        AssetDatabase.CreateAsset(configurationSO, PuzzleSubConfigurationFolderPath + "/" + this.baseName + "Configuration/" + this.baseName + "Configuration.asset");
        var PuzzleGameConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleGameConfiguration>("t:" + typeof(PuzzleGameConfiguration));
        var configurationField = PuzzleGameConfiguration.GetType().GetField(this.baseName + "Configuration");
        configurationField.SetValue(PuzzleGameConfiguration, configurationSO);
        EditorUtility.SetDirty(PuzzleGameConfiguration);

        //regenerate profile
        var path = AssetDatabase.GetAssetPath(AssetFinder.SafeSingleAssetFind<GameCreationWizardEditorProfile>("t:" + typeof(GameCreationWizardEditorProfile).Name));
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(typeof(GameCreationWizardEditorProfile)), path);
    }

    private void DoGenerateCreationWizardProfile()
    {
        var creationWizardProfileSO = ScriptableObject.CreateInstance(this.baseName + "CreationWizardProfile");
        AssetDatabase.CreateAsset(creationWizardProfileSO, EditorCreationWizardFolderPath + "/" + this.baseName + "/" + this.baseName + "CreationWizardProfile.asset");
    }
}
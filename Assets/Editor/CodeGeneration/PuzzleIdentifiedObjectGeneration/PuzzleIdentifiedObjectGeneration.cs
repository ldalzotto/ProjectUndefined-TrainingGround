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
                    this.puzzleConfigurationFodler = CommonCodeGeneration.CreatePuzzleSubConfigurationFolderIfNecessary(this.baseName);
                    this.DoGenerateInherentData();
                    this.DoGenerateConfiguration();
                    this.UpdatePuzzleGameConfiguration();
                    CreationWizardCreation.DoGenerateCreationWizardScripts(this.baseName);
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
                    CreationWizardCreation.DoGenerateCreationWizardScriptsAssets(this.baseName);
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

        string filename = PathConstants.IDBasePath + "/" + this.idEnumClass.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
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
                new Dictionary<string, string>() { { "${baseName}", this.baseName } },
                insertionGuard: (file) => !file.Contains(this.baseName + "Configuration"));
        #endregion
    }

    private void DoGenerateCreateGameDesignerModule()
    {
        DirectoryInfo sourceTemplateDirectory = new DirectoryInfo(PathConstants.CodeGenrationGameDesignerConfigurationCreationTemplatePath);
        if (sourceTemplateDirectory.Exists)
        {

            DirectoryInfo targetDirectory = new DirectoryInfo(PathConstants.GameDesignerModulesPath + "/" + this.baseName);
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            CodeGenerationHelper.DuplicateDirectoryWithParamtersRecursive(sourceTemplateDirectory, targetDirectory, new Dictionary<string, string>() {
                { "${baseName}", this.baseName}
            });
        }

    }

    private void DoGenerateConfigurationGameDesignerModule()
    {
        string gameDesignerConfigurationModulesFile = File.ReadAllText(PathConstants.GameDesignerConfigurationModulesPath);

        if (!gameDesignerConfigurationModulesFile.Contains(this.baseName + "ConfigurationModule"))
        {
            string configurationToAdd = configurationToAdd = CodeGenerationHelper.ApplyStringParameters(File.ReadAllText(PathConstants.GameDesignerConfigurationModuleTemplatepath), new Dictionary<string, string>() {
              {"${baseName}", this.baseName }
            });

            gameDesignerConfigurationModulesFile =
               gameDesignerConfigurationModulesFile.Insert(gameDesignerConfigurationModulesFile.IndexOf("//${addNewEntry}"), configurationToAdd);

            File.WriteAllText(PathConstants.GameDesignerConfigurationModulesPath, gameDesignerConfigurationModulesFile);
        }
    }

    private void UpdateGameDesignerChoiceTree()
    {
        CodeGenerationHelper.AddGameDesignerChoiceTree(new List<KeyValuePair<string, string>>() {
            new KeyValuePair<string, string>("Puzzle//" + this.baseName + "//.Create" + this.baseName, "Editor_GameDesigner.Create" + this.baseName),
            new KeyValuePair<string, string>("Configuration//." + this.baseName + "ConfigurationModule",  "Editor_GameDesigner." + this.baseName + "ConfigurationModule")
        });
    }


    private void DoCreateConfigurationAsset()
    {
        var configurationSO = ScriptableObject.CreateInstance(this.baseName + "Configuration");
        AssetDatabase.CreateAsset(configurationSO, PathConstants.PuzzleSubConfigurationFolderPath + "/" + this.baseName + "Configuration/" + this.baseName + "Configuration.asset");
        var PuzzleGameConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleGameConfiguration>("t:" + typeof(PuzzleGameConfiguration));
        var configurationField = PuzzleGameConfiguration.GetType().GetField(this.baseName + "Configuration");
        configurationField.SetValue(PuzzleGameConfiguration, configurationSO);
        EditorUtility.SetDirty(PuzzleGameConfiguration);
        
    }
}
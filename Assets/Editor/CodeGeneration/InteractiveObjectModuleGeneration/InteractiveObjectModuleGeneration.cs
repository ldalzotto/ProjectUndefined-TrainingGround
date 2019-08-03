using Editor_GameDesigner;
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
    private const string InteractiveObjectModulePath = "Assets/~RTPuzzleGame/InteractiveObject/Modules";
    private const string INteractiveObjectStaticConfigurationPath = "Assets/~RTPuzzleGame/InteractiveObject/Script/StaticConfiguration";
    private const string PuzzleGameConfigurationsEditorConstantsPath = "Assets/Editor/CreationWizard_PuzzleGame/Common";
    private const string GameDesignerBasePath = "Assets/Editor/GameDesigner";
    private const string GameDesignerModulesPath = "Assets/Editor/GameDesigner/Modules";
    private const string CustomEditorPath = "Assets/Editor/GameCustomEditors";

    private const string InteractiveObjectIdentifiedModuleWizardConfigurationTemplatePath = "Assets/Editor/CodeGeneration/Templates/InteractiveObjectModuleWizardConfiguration/InteractiveObjetIdentifiedModuleWizardConfigurationTemplate.txt";
    private const string InteractiveObjectNonIdentifiedModuleWizardConfigurationTemplatePath = "Assets/Editor/CodeGeneration/Templates/InteractiveObjectModuleWizardConfiguration/InteractiveObjetNonIdentifiedModuleWizardConfigurationTemplate.txt";
    private const string InteractiveObjectModulesInitializationOperationsMethodTemplatePath = "Assets/Editor/CodeGeneration/Templates/InteractiveObjectModulesInitializationOperations/InteractiveObjectModulesInitializationOperationsMethodTemplate.txt";
    private const string GameDesignerEditModuleTemplate = "Assets/Editor/CodeGeneration/Templates/GameDesignerTemplates/Edit${baseName}.cs.txt";
    private const string CustomEditorTemplatePath = "Assets/Editor/CodeGeneration/Templates/CustomEditorTemplate/${baseName}CustomEditor.cs.txt";

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
                    this.DoGenerateModule();
                    this.UpdateInteractiveObjectModulesConfiguration();
                    this.UpdatePuzzleInteractiveObjectModulePrefabs();
                    if (this.isIdentified)
                    {
                        this.UpdateInteractiveObjectModuleWizardID();
                    }

                    this.UpdateInteractiveObjectModuleWizardConfiguration();

                    if (this.isIdentified)
                    {
                        this.DoGenerateGameDesignerEditModule();
                        this.DoGenerateModuleCustomEditor();
                    }

                }
            }
        }

        if (GUILayout.Button("GENERATE ASSETS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    this.DoGenerateBasePrefad();
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
        DirectoryInfo moduleDirectory = new DirectoryInfo(InteractiveObjectModulePath + "/" + this.baseName + "Module");
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

        var tickMethod = new CodeMemberMethod();
        tickMethod.Name = "Tick";
        tickMethod.Attributes = MemberAttributes.Public;
        tickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "d"));
        tickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "timeAttenuationFactor"));
        moduleClass.Members.Add(tickMethod);

        var initMethod = new CodeMemberMethod();
        initMethod.Name = "Tick";
        initMethod.Attributes = MemberAttributes.Public;
        moduleClass.Members.Add(initMethod);

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

    private void UpdateInteractiveObjectModulesConfiguration()
    {
        //Generate a new initialisation method
        var interactiveObjectModulesInitializationOperationsClassFile = CodeGenerationHelper.ClassFileFromType(typeof(InteractiveObjectModulesInitializationOperations));
        if (!interactiveObjectModulesInitializationOperationsClassFile.Content.Contains("Initialize" + this.baseName + "Module"))
        {
            interactiveObjectModulesInitializationOperationsClassFile.Content =
                interactiveObjectModulesInitializationOperationsClassFile.Content.Insert(interactiveObjectModulesInitializationOperationsClassFile.Content.IndexOf("//${addNewEntry}"),
                   CodeGenerationHelper.ApplyStringParameters(File.ReadAllText(InteractiveObjectModulesInitializationOperationsMethodTemplatePath), new Dictionary<string, string>() {
                         {"${baseName}", this.baseName }
                 }));

            File.WriteAllText(interactiveObjectModulesInitializationOperationsClassFile.Path, interactiveObjectModulesInitializationOperationsClassFile.Content);
        }

        //Regenerate coniguration
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(InteractiveObjectTypeConfiguration).Namespace);
        var moduleClass = new CodeTypeDeclaration();
        moduleClass.Name = typeof(InteractiveObjectTypeConfiguration).Name;
        moduleClass.IsClass = true;
        moduleClass.Attributes = MemberAttributes.Public | MemberAttributes.Static;

        var initializationConfigurationField = new CodeMemberField();
        initializationConfigurationField.Name = nameof(InteractiveObjectTypeConfiguration.InitializationConfiguration);
        initializationConfigurationField.Attributes = MemberAttributes.Public | MemberAttributes.Static;
        initializationConfigurationField.Type = new CodeTypeReference(InteractiveObjectTypeConfiguration.InitializationConfiguration.GetType());
        var initializationConfigurationFieldDic = InteractiveObjectTypeConfiguration.InitializationConfiguration.ToList()
           .ConvertAll(kv => new KeyValuePair<string, string>("typeof(" + kv.Key.FullName + ")", "InteractiveObjectModulesInitializationOperations.Initialize" + kv.Key.Name))
             .Union(new List<KeyValuePair<string, string>>() {
                  new KeyValuePair<string, string>("typeof(" + "RTPuzzle." + this.baseName + "Module)", "InteractiveObjectModulesInitializationOperations.Initialize" + this.baseName + "Module") })
           .GroupBy(kv => kv.Key)
           .ToDictionary(kv => kv.Key, kv => kv.First().Value);
        initializationConfigurationField.InitExpression = new CodeSnippetExpression("new System.Collections.Generic.Dictionary<System.Type, System.Action<InteractiveObjectInitializationObject, InteractiveObjectType>>()" +
            CodeGenerationHelper.FormatDictionaryToCodeSnippet(initializationConfigurationFieldDic));
        moduleClass.Members.Add(initializationConfigurationField);

        samples.Types.Add(moduleClass);
        compileUnity.Namespaces.Add(samples);

        string filename = INteractiveObjectStaticConfigurationPath + "/" + typeof(InteractiveObjectTypeConfiguration).Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }

    }

    private void UpdatePuzzleInteractiveObjectModulePrefabs()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(InteractiveObjectTypeConfiguration).Namespace);
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

        string filename = PuzzleGameConfigurationsEditorConstantsPath + "/" + typeof(PuzzleInteractiveObjectModulePrefabs).Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void UpdateInteractiveObjectModuleWizardID()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(InteractiveObjectModuleWizardID).Namespace);
        var interactiveObjectModuleWizardID = CodeGenerationHelper.CopyClassAndFieldsFromExistingType(typeof(InteractiveObjectModuleWizardID));

        //Add the new
        bool add = true;
        foreach (var field in typeof(InteractiveObjectModuleWizardID).GetFields())
        {
            if (field.Name == this.baseName + "ID")
            {
                add = false;
            }
        }
        if (add)
        {
            var interactiveObjectPrefab = new CodeMemberField("GameConfigurationID." + this.baseName + "ID", this.baseName + "ID");
            interactiveObjectPrefab.Attributes = MemberAttributes.Public;
            interactiveObjectPrefab.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(CustomEnum).Name));
            interactiveObjectModuleWizardID.Members.Add(interactiveObjectPrefab);
        }


        samples.Types.Add(interactiveObjectModuleWizardID);
        compileUnity.Namespaces.Add(samples);

        string filename = GameDesignerBasePath + "/" + typeof(InteractiveObjectModuleWizardID).Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void UpdateInteractiveObjectModuleWizardConfiguration()
    {
        if (!InteractiveObjectModuleWizardConfiguration.InteractiveObjectModuleConfiguration.Keys.ToList().ConvertAll(t => t.Name).ToList().Contains(this.baseName + "Module"))
        {
            string configurationToAdd = string.Empty;
            if (this.isIdentified)
            {
                configurationToAdd = CodeGenerationHelper.ApplyStringParameters(File.ReadAllText(InteractiveObjectIdentifiedModuleWizardConfigurationTemplatePath), new Dictionary<string, string>() {
              {"${baseName}", this.baseName }
          });
            }
            else
            {
                configurationToAdd = CodeGenerationHelper.ApplyStringParameters(File.ReadAllText(InteractiveObjectNonIdentifiedModuleWizardConfigurationTemplatePath), new Dictionary<string, string>() {
              {"${baseName}", this.baseName }
          });
            }

            var interactiveObjectModuleWizardConfigurationClassFile = CodeGenerationHelper.ClassFileFromType(typeof(InteractiveObjectModuleWizardConfiguration));
            interactiveObjectModuleWizardConfigurationClassFile.Content =
                interactiveObjectModuleWizardConfigurationClassFile.Content.Insert(interactiveObjectModuleWizardConfigurationClassFile.Content.IndexOf("//${addNewEntry}"), configurationToAdd) + "\r\n";

            File.WriteAllText(interactiveObjectModuleWizardConfigurationClassFile.Path, interactiveObjectModuleWizardConfigurationClassFile.Content);
        }
    }

    private void DoGenerateGameDesignerEditModule()
    {
        CodeGenerationHelper.CopyFile(new DirectoryInfo(GameDesignerModulesPath + "/" + this.baseName), new Dictionary<string, string>() {
              {"${baseName}", this.baseName }
            }, new FileInfo(GameDesignerEditModuleTemplate));

        CodeGenerationHelper.AddGameDesignerChoiceTree(new List<KeyValuePair<string, string>>() {
            new KeyValuePair<string, string>("Puzzle//" + this.baseName + "//.Edit" + this.baseName, "Edit" + this.baseName)
        });
    }

    private void DoGenerateModuleCustomEditor()
    {
        CodeGenerationHelper.CopyFile(new DirectoryInfo(CustomEditorPath), new Dictionary<string, string>() {
              {"${baseName}", this.baseName }
            }, new FileInfo(CustomEditorTemplatePath));
    }

    private void DoGenerateBasePrefad()
    {
        var tmpScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        tmpScene.name = UnityEngine.Random.Range(0, 999999).ToString();

        var basePrefabGenerated = new GameObject("Base" + this.baseName + "Module");
        basePrefabGenerated.AddComponent(TypeHelper.GetType("RTPuzzle." + this.baseName + "Module"));
        PrefabUtility.SaveAsPrefabAsset(basePrefabGenerated, InteractiveObjectModulePath + "/" + this.baseName + "Module/" + "Base" + this.baseName + "Module.prefab");

        EditorSceneManager.CloseScene(tmpScene, true);
    }
}

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

public class InteractiveObjectModuleGeneration : EditorWindow
{
    private const string InteractiveObjectModulePath = "Assets/~RTPuzzleGame/InteractiveObject/Modules";
    private const string INteractiveObjectStaticConfigurationPath = "Assets/~RTPuzzleGame/InteractiveObject/Script/StaticConfiguration";
    private const string PuzzleGameConfigurationsEditorConstantsPath = "Assets/Editor/CreationWizard_PuzzleGame/Common";

    [MenuItem("Generation/InteractiveObjectModuleGeneration")]
    static void Init()
    {
        InteractiveObjectModuleGeneration window = (InteractiveObjectModuleGeneration)EditorWindow.GetWindow(typeof(InteractiveObjectModuleGeneration));
        window.Show();
    }

    #region Input
    private string baseName;
    private bool isIdentified;
    private EnumPicker idEnumPicker;
    #endregion

    private void OnEnable()
    {
        this.idEnumPicker = new EnumPicker(typeof(PointOfInterestId).Namespace, null);
    }

    private void OnGUI()
    {
        this.DoInput();

        if (GUILayout.Button("GENERATE"))
        {
            if (!string.IsNullOrEmpty(this.baseName))
            {
                this.DoGenerateModule();
                this.UpdateInteractiveObjectModulesConfiguration();
                this.UpdatePuzzleInteractiveObjectModulePrefabs();
            }
        }
    }

    private void DoInput()
    {
        this.baseName = EditorGUILayout.TextField("Base name", this.baseName);
        this.isIdentified = EditorGUILayout.Toggle("Is Identified : ", this.isIdentified);
        if (this.isIdentified)
        {
            this.idEnumPicker.OnGUI();
        }
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
            idMember.Type = new CodeTypeReference(typeof(PointOfInterestId).Namespace + "." + this.idEnumPicker.GetSelectedKey());
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
                  new KeyValuePair<string, string>("typeof(" + "RTPuzzle." + this.baseName + "Module)", "InteractiveObjectModulesInitializationOperations.DummyInitialization") })
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
}


/*
 * 
 * 
 using UnityEngine;
using System.Collections;
using RTPuzzle;

namespace Editor_MainGameCreationWizard
{
    [System.Serializable]
    public class PuzzleInteractiveObjectModulePrefabs
    {
        [ReadOnly]
        public ModelObjectModule BaseModelObjectModule;
        [ReadOnly]
        public ObjectRepelTypeModule BaseObjectRepelTypeModule;
        [ReadOnly]
        public AttractiveObjectTypeModule BaseAttractiveObjectModule;
        [ReadOnly]
        public TargetZoneObjectModule BaseTargetZoneObjectModule;
        [ReadOnly]
        public LevelCompletionTriggerModule BaseLevelCompletionTriggerModule;
        [ReadOnly]
        public LaunchProjectileModule BaseLaunchProjectileModule;
        [ReadOnly]
        public DisarmObjectModule BaseDisarmObjectModule;
        [ReadOnly]
        public ActionInteractableObjectModule BaseActionInteractableObjectModule;
    }
}
     */

using Editor_GameDesigner;
using RTPuzzle;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AIComponentGeneration : EditorWindow
{
    [MenuItem("Generation/AIComponentGeneration")]
    static void Init()
    {
        AIComponentGeneration window = (AIComponentGeneration)EditorWindow.GetWindow(typeof(AIComponentGeneration));
        window.Show();
    }

    private string AIComponentBaseName;

    private DirectoryInfo componentDirectory;
    private CodeTypeDeclaration componentClass;
    private CodeTypeDeclaration abstractManagerClass;
    private CodeTypeDeclaration managerClass;

    private void OnGUI()
    {
        this.AIComponentBaseName = EditorGUILayout.TextField("AI Component base name : ", this.AIComponentBaseName);
        if (GUILayout.Button("GENERATE"))
        {
            this.DoGeneration();
        }
    }

    private void DoGeneration()
    {
        if (!string.IsNullOrEmpty(this.AIComponentBaseName))
        {
            this.CreateComponentFolderIfNecessary();
            this.GenerateComponentWithAbstractManager();
            this.GenerateManager();
            this.UpdateGenericPuzzleAIComponentsV2();
            this.AddEditorWizardConstants();
        }
    }

    private void CreateComponentFolderIfNecessary()
    {
        this.componentDirectory = new DirectoryInfo(PathConstants.AIComponentBasePath + "/" + this.AIComponentBaseName);
        if (!componentDirectory.Exists)
        {
            componentDirectory.Create();
        }
    }

    private void GenerateComponentWithAbstractManager()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(AttractiveObjectAction).Namespace);
        samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));
        samples.Imports.Add(new CodeNamespaceImport("System"));
        this.componentClass = new CodeTypeDeclaration(this.AIComponentBaseName + "Component");
        componentClass.IsClass = true;

        componentClass.BaseTypes.Add(typeof(AbstractAIComponent).Name);
        componentClass.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));

        componentClass.CustomAttributes.Add(CodeGenerationHelper.GenerateCreateAssetMenuAttribute(componentClass.Name, "Configuration/PuzzleGame/AIComponentsConfiguration/" + componentClass.Name));

        this.abstractManagerClass = new CodeTypeDeclaration("Abstract" + this.AIComponentBaseName + "Manager");
        abstractManagerClass.IsClass = true;
        abstractManagerClass.TypeAttributes = System.Reflection.TypeAttributes.Abstract | System.Reflection.TypeAttributes.Public;

        abstractManagerClass.BaseTypes.Add("AbstractAIManager<" + this.AIComponentBaseName + "Component>");
        abstractManagerClass.BaseTypes.Add(typeof(InterfaceAIManager).Name);

        var defaultConstructor = new CodeConstructor();
        defaultConstructor.Attributes = MemberAttributes.Public;
        defaultConstructor.Parameters.Add(new CodeParameterDeclarationExpression(this.AIComponentBaseName + "Component", this.AIComponentBaseName + "Component"));
        defaultConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(this.AIComponentBaseName + "Component"));
        abstractManagerClass.Members.Add(defaultConstructor);

        var BeforeManagersUpdateMethod = new CodeMemberMethod();
        BeforeManagersUpdateMethod.Attributes = MemberAttributes.Abstract | MemberAttributes.Public;
        BeforeManagersUpdateMethod.Name = "BeforeManagersUpdate";
        BeforeManagersUpdateMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "d"));
        BeforeManagersUpdateMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "timeAttenuationFactor"));

        abstractManagerClass.Members.Add(BeforeManagersUpdateMethod);

        var IsManagerEnabledMethod = new CodeMemberMethod();
        IsManagerEnabledMethod.Attributes = MemberAttributes.Abstract | MemberAttributes.Public;
        IsManagerEnabledMethod.Name = "IsManagerEnabled";
        IsManagerEnabledMethod.ReturnType = new CodeTypeReference(typeof(bool));

        abstractManagerClass.Members.Add(IsManagerEnabledMethod);

        var OnManagerTickMethod = new CodeMemberMethod();
        OnManagerTickMethod.Attributes = MemberAttributes.Abstract | MemberAttributes.Public;
        OnManagerTickMethod.Name = "OnManagerTick";
        OnManagerTickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "d"));
        OnManagerTickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "timeAttenuationFactor"));
        OnManagerTickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(NPCAIDestinationContext), "NPCAIDestinationContext") { Direction = FieldDirection.Ref });

        abstractManagerClass.Members.Add(OnManagerTickMethod);

        var OnDestinationReachedMethod = new CodeMemberMethod();
        OnDestinationReachedMethod.Attributes = MemberAttributes.Abstract | MemberAttributes.Public;
        OnDestinationReachedMethod.Name = "OnDestinationReached";

        abstractManagerClass.Members.Add(OnDestinationReachedMethod);

        var OnStateResetMethod = new CodeMemberMethod();
        OnStateResetMethod.Attributes = MemberAttributes.Abstract | MemberAttributes.Public;
        OnStateResetMethod.Name = "OnStateReset";

        abstractManagerClass.Members.Add(OnStateResetMethod);

        samples.Types.Add(componentClass);
        samples.Types.Add(abstractManagerClass);

        compileUnity.Namespaces.Add(samples);

        string filename = this.componentDirectory.FullName + "/" + componentClass.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void GenerateManager()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(AttractiveObjectAction).Namespace);
        samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));
        this.managerClass = new CodeTypeDeclaration(this.abstractManagerClass.Name.Replace("Abstract", ""));
        managerClass.IsClass = true;
        managerClass.TypeAttributes = System.Reflection.TypeAttributes.Public;

        managerClass.BaseTypes.Add(this.abstractManagerClass.Name);

        var defaultConstructor = new CodeConstructor();
        defaultConstructor.Attributes = MemberAttributes.Public;
        defaultConstructor.Parameters.Add(new CodeParameterDeclarationExpression(this.AIComponentBaseName + "Component", this.AIComponentBaseName + "Component"));
        defaultConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(this.AIComponentBaseName + "Component"));
        managerClass.Members.Add(defaultConstructor);

        var BeforeManagersUpdateMethod = new CodeMemberMethod();
        BeforeManagersUpdateMethod.Attributes = MemberAttributes.Override | MemberAttributes.Public;
        BeforeManagersUpdateMethod.Name = "BeforeManagersUpdate";
        BeforeManagersUpdateMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "d"));
        BeforeManagersUpdateMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "timeAttenuationFactor"));

        managerClass.Members.Add(BeforeManagersUpdateMethod);

        var IsManagerEnabledMethod = new CodeMemberMethod();
        IsManagerEnabledMethod.Attributes = MemberAttributes.Override | MemberAttributes.Public;
        IsManagerEnabledMethod.Name = "IsManagerEnabled";
        IsManagerEnabledMethod.ReturnType = new CodeTypeReference(typeof(bool));
        IsManagerEnabledMethod.Statements.Add(new CodeSnippetStatement("return true;"));

        managerClass.Members.Add(IsManagerEnabledMethod);

        var OnManagerTickMethod = new CodeMemberMethod();
        OnManagerTickMethod.Attributes = MemberAttributes.Override | MemberAttributes.Public;
        OnManagerTickMethod.Name = "OnManagerTick";
        OnManagerTickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "d"));
        OnManagerTickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(float), "timeAttenuationFactor"));
        OnManagerTickMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(NPCAIDestinationContext), "NPCAIDestinationContext") { Direction = FieldDirection.Ref });

        managerClass.Members.Add(OnManagerTickMethod);

        var OnDestinationReachedMethod = new CodeMemberMethod();
        OnDestinationReachedMethod.Attributes = MemberAttributes.Override | MemberAttributes.Public;
        OnDestinationReachedMethod.Name = "OnDestinationReached";

        managerClass.Members.Add(OnDestinationReachedMethod);

        var OnStateResetMethod = new CodeMemberMethod();
        OnStateResetMethod.Attributes = MemberAttributes.Override | MemberAttributes.Public;
        OnStateResetMethod.Name = "OnStateReset";

        managerClass.Members.Add(OnStateResetMethod);

        samples.Types.Add(managerClass);
        compileUnity.Namespaces.Add(samples);

        string filename = this.componentDirectory.FullName + "/" + managerClass.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }

    private void UpdateGenericPuzzleAIComponentsV2()
    {
        var GenericPuzzleAIComponentsV2File = CodeGenerationHelper.ClassFileFromType(typeof(GenericPuzzleAIComponentsV2));
        GenericPuzzleAIComponentsV2File.Content = GenericPuzzleAIComponentsV2File.Content.Insert(GenericPuzzleAIComponentsV2File.Content.IndexOf("//${addNewEntry}"), "typeof(" + this.AIComponentBaseName + "Component)\n");
        File.WriteAllText(GenericPuzzleAIComponentsV2File.Path, GenericPuzzleAIComponentsV2File.Content);
    }

    private void AddEditorWizardConstants()
    {
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(AIManagerModuleWizardConstants).Namespace);
        samples.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
        samples.Imports.Add(new CodeNamespaceImport("RTPuzzle"));
        samples.Imports.Add(new CodeNamespaceImport("System"));
        var AIManagerModuleWizardConstantsClass = new CodeTypeDeclaration(typeof(AIManagerModuleWizardConstants).Name);
        AIManagerModuleWizardConstantsClass.IsClass = true;
        AIManagerModuleWizardConstantsClass.TypeAttributes = System.Reflection.TypeAttributes.Public;

        var AIManagerDescriptionMessageFieldName = nameof(AIManagerModuleWizardConstants.AIManagerDescriptionMessage);
        var AIManagerDescriptionMessageField = new CodeMemberField(typeof(AIManagerModuleWizardConstants).GetField(AIManagerDescriptionMessageFieldName).FieldType, AIManagerDescriptionMessageFieldName);
        AIManagerDescriptionMessageField.Attributes = MemberAttributes.Static | MemberAttributes.Public;
        var AIManagerDescriptionMessageDic = AIManagerModuleWizardConstants.AIManagerDescriptionMessage.ToList()
                .ConvertAll(kv => new KeyValuePair<string, string>("typeof(" + kv.Key.Name + ")", "\"" + kv.Value + "\""))
                 .Union(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("typeof(" + this.managerClass.Name + ")", "\"\"") }) //Add the newly created comonent
                 .GroupBy(kv => kv.Key)
                .ToDictionary(kv => kv.Key, kv => kv.First().Value);
        //Add new the new component
        AIManagerDescriptionMessageField.InitExpression = new CodeSnippetExpression("new Dictionary<Type, string>()" + CodeGenerationHelper.FormatDictionaryToCodeSnippet(AIManagerDescriptionMessageDic));

        AIManagerModuleWizardConstantsClass.Members.Add(AIManagerDescriptionMessageField);

        var AIManagerAssociatedComponentFieldName = nameof(AIManagerModuleWizardConstants.AIManagerAssociatedComponent);
        var AIManagerAssociatedComponentField = new CodeMemberField(typeof(AIManagerModuleWizardConstants).GetField(AIManagerAssociatedComponentFieldName).FieldType, AIManagerAssociatedComponentFieldName);
        AIManagerAssociatedComponentField.Attributes = MemberAttributes.Static | MemberAttributes.Public;
        var AIManagerAssociatedComponentFieldDic = AIManagerModuleWizardConstants.AIManagerAssociatedComponent.ToList()
               .ConvertAll(kv => new KeyValuePair<string, string>("typeof(" + kv.Key.Name + ")", "\"" + kv.Value + "\""))
               .Union(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("typeof(" + this.managerClass.Name + ")", "\"" + this.componentClass.Name + "\"") }) //Add the newly created comonent
                 .GroupBy(kv => kv.Key)
               .ToDictionary(kv => kv.Key, kv => kv.First().Value);
        AIManagerAssociatedComponentField.InitExpression = new CodeSnippetExpression("new Dictionary<Type, string>()" + CodeGenerationHelper.FormatDictionaryToCodeSnippet(AIManagerAssociatedComponentFieldDic));

        AIManagerModuleWizardConstantsClass.Members.Add(AIManagerAssociatedComponentField);

        samples.Types.Add(AIManagerModuleWizardConstantsClass);
        compileUnity.Namespaces.Add(samples);

        string filename = PathConstants.AIModuleWizardConstant;
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

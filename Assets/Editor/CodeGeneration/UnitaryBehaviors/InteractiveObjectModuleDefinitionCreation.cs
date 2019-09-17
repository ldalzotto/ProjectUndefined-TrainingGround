using OdinSerializer;
using RTPuzzle;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class InteractiveObjectModuleDefinitionCreation : EditorWindow
{
    [MenuItem("Generation/UnitaryBehaviors/InteractiveObjectModuleDefinitionCreation")]
    static void Init()
    {
        InteractiveObjectModuleDefinitionCreation window = (InteractiveObjectModuleDefinitionCreation)EditorWindow.GetWindow(typeof(InteractiveObjectModuleDefinitionCreation));
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
                GenerateScripts(this.baseName);
            }
        }
    }

    public static void GenerateScripts(string baseName)
    {
        if (!string.IsNullOrEmpty(baseName))
        {
            GenerateDefinitionClass(baseName);
            UpdateDefinitionConfigurations(baseName);
            AddDefinitionCondition(baseName);
            AddCustomEditorCondition(baseName);
        }
    }

    private static void GenerateDefinitionClass(string baseName)
    {
        DirectoryInfo interactiveObjectDefinitionsDirectory = new DirectoryInfo(PathConstants.InteractiveObjectModuleDefinitionPath);
        if (!interactiveObjectDefinitionsDirectory.Exists)
        {
            interactiveObjectDefinitionsDirectory.Create();
        }

        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(TargetZoneModuleDefinition).Namespace);

        var moduleClass = new CodeTypeDeclaration(baseName + "ModuleDefinition");
        moduleClass.Name = baseName + "ModuleDefinition";
        moduleClass.IsClass = true;
        moduleClass.Attributes = MemberAttributes.Public;
        moduleClass.BaseTypes.Add(typeof(SerializedScriptableObject));

        samples.Types.Add(moduleClass);
        compileUnity.Namespaces.Add(samples);

        string filename = interactiveObjectDefinitionsDirectory.FullName + "/" + baseName + "ModuleDefinition.cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }

    }

    private static void UpdateDefinitionConfigurations(string baseName)
    {
        /*
        var InteractiveObjectModuleTypesConstantsClassFile = CodeGenerationHelper.ClassFileFromType(typeof(InteractiveObjectModuleTypesConstants));

        if (!InteractiveObjectModuleTypesConstantsClassFile.Content.Contains(baseName + "ModuleDefinition"))
        {

            InteractiveObjectModuleTypesConstantsClassFile.Content =
               InteractiveObjectModuleTypesConstantsClassFile.Content.Insert(InteractiveObjectModuleTypesConstantsClassFile.Content.IndexOf("//${addNewEntry}"),
                    "            typeof(" + baseName + "ModuleDefinition" + "),\n"
                );
            File.WriteAllText(InteractiveObjectModuleTypesConstantsClassFile.Path, InteractiveObjectModuleTypesConstantsClassFile.Content);

        }
        */
    }

    private static void AddDefinitionCondition(string baseName)
    {
        //Generate a new initialisation method
        var InteractiveObjectTypeDefinitionInherentDataClassFile = CodeGenerationHelper.ClassFileFromType(typeof(InteractiveObjectTypeDefinitionInherentData));
        if (!InteractiveObjectTypeDefinitionInherentDataClassFile.Content.Contains(baseName + "ModuleDefinition"))
        {
            InteractiveObjectTypeDefinitionInherentDataClassFile.Content =
                       InteractiveObjectTypeDefinitionInherentDataClassFile.Content.Insert(InteractiveObjectTypeDefinitionInherentDataClassFile.Content.IndexOf("//${addNewEntry}"),
                          CodeGenerationHelper.ApplyStringParameters(File.ReadAllText(PathConstants.InteractiveObjectDefinitionConditionTemplatePath), new Dictionary<string, string>() {
                                             {"${baseName}", baseName }
                        }));

            File.WriteAllText(InteractiveObjectTypeDefinitionInherentDataClassFile.Path, InteractiveObjectTypeDefinitionInherentDataClassFile.Content);
        }
    }

    private static void AddCustomEditorCondition(string baseName)
    {
        var InteractiveObjectTypeCustomEditorClassFile = CodeGenerationHelper.ClassFileFromType(typeof(InteractiveObjectTypeGizmos));
        if (!InteractiveObjectTypeCustomEditorClassFile.Content.Contains(baseName + "ModuleDefinition"))
        {
            InteractiveObjectTypeCustomEditorClassFile.Content =
                       InteractiveObjectTypeCustomEditorClassFile.Content.Insert(InteractiveObjectTypeCustomEditorClassFile.Content.IndexOf("//${addNewEntry}"),
                          CodeGenerationHelper.ApplyStringParameters(File.ReadAllText(PathConstants.InteractiveObjectDefinitionCustomEditorCondition), new Dictionary<string, string>() {
                                             {"${baseName}", baseName }
                        }));

            File.WriteAllText(InteractiveObjectTypeCustomEditorClassFile.Path, InteractiveObjectTypeCustomEditorClassFile.Content);
        }
    }
}

using OdinSerializer;
using RTPuzzle;
using System.CodeDom;
using System.CodeDom.Compiler;
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
        }
    }

    private static void GenerateDefinitionClass(string baseName)
    {
        DirectoryInfo interactiveObjectDefinitionsDirectory = new DirectoryInfo(PathConstants.RTPuzzleFeaturesPath + "/" + baseName + "/InteractiveObjectDefinitions");
        if (!interactiveObjectDefinitionsDirectory.Exists)
        {
            interactiveObjectDefinitionsDirectory.Create();
        }

        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(TargetZoneModuleDefinition).Namespace);
        samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));

        var moduleClass = new CodeTypeDeclaration(baseName + "ModuleDefinition");
        moduleClass.Name = baseName + "ModuleDefinition";
        moduleClass.IsClass = true;
        moduleClass.Attributes = MemberAttributes.Public;
        moduleClass.BaseTypes.Add(typeof(AbstractInteractiveObjectDefinition));

        moduleClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ModuleMetadata)),
            new CodeAttributeArgument(new CodePrimitiveExpression("")), new CodeAttributeArgument(new CodePrimitiveExpression(""))));


        var createObjectMethod = new CodeMemberMethod();
        createObjectMethod.Name = "CreateObject";
        createObjectMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
        createObjectMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Transform), "parent"));
        createObjectMethod.Statements.Add(new CodeSnippetStatement(
           "MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration().Base" + baseName + "Module);"
            ));
        moduleClass.Members.Add(createObjectMethod);

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
}

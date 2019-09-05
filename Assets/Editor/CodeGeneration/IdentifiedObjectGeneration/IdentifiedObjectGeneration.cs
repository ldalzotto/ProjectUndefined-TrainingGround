using GameConfigurationID;
using RTPuzzle;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class IdentifiedObjectGeneration : EditorWindow
{

    [MenuItem("Generation/PuzzleIdentifiedObjectGeneration")]
    static void Init()
    {
        IdentifiedObjectGeneration window = (IdentifiedObjectGeneration)EditorWindow.GetWindow(typeof(IdentifiedObjectGeneration));
        window.Show();
    }

    private string baseName;
    private GameTypeGeneration GameTypeGeneration;

    private DirectoryInfo puzzleConfigurationFodler;
    private CodeTypeDeclaration idEnumClass;

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
                    this.puzzleConfigurationFodler = CommonCodeGeneration.CreatePuzzleSubConfigurationFolderIfNecessary(this.baseName, this.GameTypeGeneration);
                    PuzzleConfigurationCreation.DoGenerateInherentData(this.baseName, this.GameTypeGeneration);
                    PuzzleConfigurationCreation.DoGenerateConfiguration(this.baseName, this.GameTypeGeneration);
                    PuzzleConfigurationCreation.UpdateGameConfiguration(this.baseName, this.GameTypeGeneration);
                    CreationWizardCreation.DoGenerateCreationWizardScripts(this.baseName, this.GameTypeGeneration);
                    this.DoGenerateCreateGameDesignerModule();
                    PuzzleConfigurationCreation.DoGenerateConfigurationGameDesignerModule(this.baseName);
                }
            }
        }

        if (GUILayout.Button("GENERATE ASSETS"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                if (!string.IsNullOrEmpty(this.baseName))
                {
                    PuzzleConfigurationCreation.DoCreateConfigurationAsset(this.baseName, this.GameTypeGeneration);
                    CreationWizardCreation.DoGenerateCreationWizardScriptsAssets(this.baseName);
                }
            }
        }
    }

    private void DoInput()
    {
        this.GameTypeGeneration = (GameTypeGeneration)EditorGUILayout.EnumPopup(this.GameTypeGeneration);
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
                {"${baseName}", this.baseName}
            });

            CodeGenerationHelper.AddGameDesignerChoiceTree(new List<KeyValuePair<string, string>>() {
            new KeyValuePair<string, string>("Puzzle//" + this.baseName + "//.Create" + this.baseName, "Editor_GameDesigner.Create" + this.baseName)
            });

        }
    }


}
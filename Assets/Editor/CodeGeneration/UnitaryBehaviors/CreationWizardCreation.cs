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
        UpdateNameConstantsEditorConstant(baseName);
    }

    public static void DoGenerateCreationWizardScriptsAssets(string baseName)
    {
        RegenerateGameCreationWizardEditorProfile();
        DoGenerateCreationWizardProfile(baseName);
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

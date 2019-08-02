using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnumIDGeneration : EditorWindow
{
    [MenuItem("Generation/IDGeneration")]
    static void Init()
    {
        EnumIDGeneration window = (EnumIDGeneration)EditorWindow.GetWindow(typeof(EnumIDGeneration));
        window.Show();
    }

    public static void Init(Type initialEnumType)
    {
        EnumIDGeneration window = (EnumIDGeneration)EditorWindow.GetWindow(typeof(EnumIDGeneration));
        window.Show();
        window.OnEnable();
        window.enumSelectionPicker.SetSelectedKey(initialEnumType.Name);
    }

    public static void Init(Type initialEnumType, string initialEnumName)
    {
        EnumIDGeneration window = (EnumIDGeneration)EditorWindow.GetWindow(typeof(EnumIDGeneration));
        window.Show();
        window.OnEnable();
        window.enumSelectionPicker.SetSelectedKey(initialEnumType.Name);
        window.KeyName = initialEnumName;
    }

    private EnumPicker enumSelectionPicker;
    private ButtonTreePickerGUI enumDeletionPicker;

    public void OnEnable()
    {
        this.enumSelectionPicker = new EnumPicker("GameConfigurationID", this.OnEnumSelected);
    }

    private void OnEnumSelected()
    {
        this.enumDeletionPicker = new ButtonTreePickerGUI(Enum.GetValues(this.enumSelectionPicker.EnumType).Cast<Enum>().ToList().ConvertAll(e => e.ToString()), null);
    }

    private void OnGUI()
    {
        this.DoPickEnum();
        this.DoAddKey();
        this.DoRemoveKey();
        this.DoRegenerateAll();
    }


    private void DoPickEnum()
    {
        this.enumSelectionPicker.OnGUI();
    }

    private string KeyName;

    private void DoAddKey()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("ADD : ");
        this.KeyName = EditorGUILayout.TextField(this.KeyName);
        if (GUILayout.Button("GENERATE") && this.enumSelectionPicker.EnumType != null)
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                this.RegenerateEnum(true, false, this.enumSelectionPicker.EnumType);
                this.OnEnable();
            }
        }
        EditorGUILayout.Separator();
    }

    private Rect EnumDeletionDropdownButton;

    private void DoRemoveKey()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("REMOVE : ");
        if (this.enumDeletionPicker != null)
        {
            this.enumDeletionPicker.OnGUI();

            if (GUILayout.Button("GENERATE"))
            {
                if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
                {
                    this.RegenerateEnum(false, true, this.enumSelectionPicker.EnumType);
                    this.OnEnable();
                }
            }

        }
        EditorGUILayout.Separator();
    }

    private void RegenerateEnum(bool add, bool remove, Type involedEnumType)
    {
        //build existing keys
        Dictionary<string, int> existingValues = new Dictionary<string, int>();
        foreach (var genEnum in involedEnumType.GetEnumValues())
        {
            existingValues.Add(genEnum.ToString(), (int)genEnum);
        }

        if ((add && !existingValues.ContainsKey(KeyName)) || (remove && existingValues.ContainsKey(this.enumDeletionPicker.GetSelectedKey())) || (!add && !remove))
        {
            var cUnit = new CodeCompileUnit();

            CodeNamespace emptyNamespace = new CodeNamespace("GameConfigurationID");
            cUnit.Namespaces.Add(emptyNamespace);


            var enumTypeDeclaration = new CodeTypeDeclaration(involedEnumType.Name);
            enumTypeDeclaration.IsEnum = true;
            enumTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));

            //add existing entry
            foreach (var existingValue in existingValues)
            {
                bool skip = false;
                if (remove && existingValue.Key == this.enumDeletionPicker.GetSelectedKey())
                {
                    skip = true;
                }

                if (!skip)
                {
                    CodeMemberField f1 = new CodeMemberField()
                    {
                        Name = existingValue.Key,
                        InitExpression = new CodePrimitiveExpression(existingValue.Value)
                    };

                    enumTypeDeclaration.Members.Add(f1);
                }
            }


            if (add)
            {
                //add new entry
                var maxIndexIncremented = existingValues.Values.ToList().Max() + 1;
                CodeMemberField f = new CodeMemberField()
                {
                    Name = this.KeyName,
                    InitExpression = new CodePrimitiveExpression(maxIndexIncremented)
                };
                enumTypeDeclaration.Members.Add(f);
            }

            emptyNamespace.Types.Add(enumTypeDeclaration);

            CodeGeneratorOptions lOptions = new CodeGeneratorOptions();
            lOptions.IndentString = "  "; // or "\t";
            lOptions.BlankLinesBetweenMembers = true;
            CSharpCodeProvider lCSharpCodeProvider = new CSharpCodeProvider();
            using (StreamWriter lStreamWriter = new StreamWriter(Application.dataPath + "\\@GameConfigurationID\\IDs\\" + involedEnumType.Name + ".cs", false))
            {
                lCSharpCodeProvider.GenerateCodeFromCompileUnit(cUnit, lStreamWriter, lOptions);
            }

        }
        else
        {
            Debug.LogError("The name : " + this.KeyName + " already exist in enum : " + involedEnumType.Name);
        }
    }


    private void DoRegenerateAll()
    {
        EditorGUILayout.Separator();
        if (GUILayout.Button("REGENERATE ALL"))
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                foreach (var availableEnum in this.enumSelectionPicker.AvailableEnums)
                {
                    this.RegenerateEnum(false, false, availableEnum);
                }
            }
        }
    }
}

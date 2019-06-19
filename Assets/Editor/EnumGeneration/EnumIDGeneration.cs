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

    private TreePickerPopup enumSelectionPicker;
    private List<Type> availableEnums;
    private List<string> availableEnumTypesString;
    private Type EnumType;


    private TreePickerPopup enumDeletionPicker;
    private List<Enum> selectedEnumPossibilities;
    private List<string> selectedEnumPossibilitiesString;
    private Enum deletedEnum;

    public void OnEnable()
    {
        this.availableEnums = AppDomain.CurrentDomain.GetAssemblies().ToList().SelectMany(l => l.GetTypes().ToList())
                .Select(t => t).Where(t => t.IsEnum && t.Namespace == "GameConfigurationID")
                .ToList();
        this.availableEnumTypesString = this.availableEnums.ConvertAll(t => t.Name);
        this.enumSelectionPicker = new TreePickerPopup(this.availableEnumTypesString, this.OnEnumSelected, string.Empty);
    }

    private void OnEnumSelected()
    {
        this.EnumType = this.availableEnums[this.availableEnumTypesString.IndexOf(this.enumSelectionPicker.SelectedKey)];

        this.selectedEnumPossibilities = Enum.GetValues(this.EnumType).Cast<Enum>().ToList();
        this.selectedEnumPossibilitiesString = this.selectedEnumPossibilities.ConvertAll(e => e.ToString());
        this.enumDeletionPicker = new TreePickerPopup(this.selectedEnumPossibilitiesString, () => { this.deletedEnum = this.selectedEnumPossibilities[this.selectedEnumPossibilitiesString.IndexOf(this.enumDeletionPicker.SelectedKey)]; }, string.Empty);
    }

    private void OnGUI()
    {
        this.DoPickEnum();
        this.DoAddKey();
        this.DoRemoveKey();
        this.DoRegenerateAll();
    }

    private Rect EnumChoiceDropdownButton;

    private void DoPickEnum()
    {
        if (EditorGUILayout.DropdownButton(new GUIContent(this.enumSelectionPicker.SelectedKey), FocusType.Keyboard))
        {
            this.enumSelectionPicker.WindowDimensions = new Vector2(EnumChoiceDropdownButton.width, 250f);
            PopupWindow.Show(EnumChoiceDropdownButton, this.enumSelectionPicker);
        }
        if (Event.current.type == EventType.Repaint)
        {
            this.EnumChoiceDropdownButton = GUILayoutUtility.GetLastRect();
        }
    }

    private string KeyName;

    private void DoAddKey()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("ADD : ");
        this.KeyName = EditorGUILayout.TextField(this.KeyName);
        if (GUILayout.Button("GENERATE") && this.EnumType != null)
        {
            if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
            {
                this.RegenerateEnum(true, false, this.EnumType);
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
            if (EditorGUILayout.DropdownButton(new GUIContent(this.enumDeletionPicker.SelectedKey), FocusType.Keyboard))
            {
                this.enumDeletionPicker.WindowDimensions = new Vector2(EnumDeletionDropdownButton.width, 250f);
                PopupWindow.Show(EnumDeletionDropdownButton, this.enumDeletionPicker);
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.EnumDeletionDropdownButton = GUILayoutUtility.GetLastRect();
            }

            if (GUILayout.Button("GENERATE"))
            {
                if (EditorUtility.DisplayDialog("GENERATE ?", "Confirm generation.", "YES", "NO"))
                {
                    this.RegenerateEnum(false, true, this.EnumType);
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

        if ((add && !existingValues.ContainsKey(KeyName)) || (remove && existingValues.ContainsKey(this.deletedEnum.ToString())) || (!add && !remove))
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
                if (remove && existingValue.Key == this.deletedEnum.ToString())
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
                foreach (var availableEnum in this.availableEnums)
                {
                    this.RegenerateEnum(false, false, availableEnum);
                }
            }
        }
    }
}

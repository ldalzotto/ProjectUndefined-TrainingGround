using System;
using System.CodeDom;
using System.Collections.Generic;
using UnityEngine;

public static class CodeGenerationHelper
{
    public static CodeAttributeDeclaration GenerateCreateAssetMenuAttribute(string filename, string menuName)
    {
        var createAssetMenuAttribute = new CodeAttributeDeclaration("UnityEngine.CreateAssetMenu");
        createAssetMenuAttribute.Arguments.Add(new CodeAttributeArgument("fileName", new CodePrimitiveExpression(filename)));
        createAssetMenuAttribute.Arguments.Add(new CodeAttributeArgument("menuName", new CodePrimitiveExpression(menuName)));
        createAssetMenuAttribute.Arguments.Add(new CodeAttributeArgument("order", new CodePrimitiveExpression(1)));
        return createAssetMenuAttribute;
    }

    public static CodeTypeDeclaration CopyClassAndFieldsFromExistingType(Type sourceType)
    {
        var targetClass = new CodeTypeDeclaration(sourceType.Name);
        targetClass.IsClass = true;
        targetClass.TypeAttributes = System.Reflection.TypeAttributes.Public;

        var baseType = sourceType.BaseType;
        if (baseType != null)
        {
            targetClass.BaseTypes.Add(baseType);
        }

        /*
        if (codeNameSpaceForImport != null)
        {
            //using dependencies by file search
            var sourceClassFiles = Directory.GetFiles(Application.dataPath, sourceType.Name + ".cs", SearchOption.AllDirectories);
            string sourceClassFile = string.Empty;
            if (sourceClassFiles != null && sourceClassFiles.Length > 0)
            {
                Debug.Log(sourceClassFiles[0]);
                using (StreamReader sr = new StreamReader(sourceClassFiles[0]))
                {
                    sourceClassFile = sr.ReadToEnd();
                }
            }
            foreach (Match usingMatch in new Regex("using(.*?) (.*?);").Matches(sourceClassFile))
            {
                codeNameSpaceForImport.Imports.Add(new CodeNamespaceImport(usingMatch.Groups[1].Value));
            }
        }
        */

        foreach (var customAttribute in sourceType.GetCustomAttributes(false))
        {
            targetClass.CustomAttributes = UpdateCustomAttribute(targetClass.CustomAttributes, customAttribute);
        }

        foreach (var field in sourceType.GetFields())
        {
            var addedField = new CodeMemberField(field.FieldType, field.Name);
            addedField.Attributes = MemberAttributes.Public;

            if (field.IsStatic)
            {
                addedField.Attributes = addedField.Attributes | MemberAttributes.Static;
            }

            foreach (var fieldCustomAttribute in field.GetCustomAttributes(false))
            {
                addedField.CustomAttributes = UpdateCustomAttribute(addedField.CustomAttributes, fieldCustomAttribute);
            }

            if (field.FieldType == typeof(string) && field.IsStatic)
            {
                addedField.InitExpression = new CodePrimitiveExpression(field.GetValue(null));
            }

            targetClass.Members.Add(addedField);
        }

        return targetClass;
    }

    private static CodeAttributeDeclarationCollection UpdateCustomAttribute(CodeAttributeDeclarationCollection cutomAttributes, object sourceCustomAttribute)
    {
        if (sourceCustomAttribute.GetType() == typeof(SerializableAttribute))
        {
            cutomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
        }
        else if (sourceCustomAttribute.GetType() == typeof(ReadOnly))
        {
            cutomAttributes.Add(new CodeAttributeDeclaration(typeof(ReadOnly).Name));
        }
        else if (sourceCustomAttribute.GetType() == typeof(CreateAssetMenuAttribute))
        {
            var CreateAssetMenuAttribute = (CreateAssetMenuAttribute)sourceCustomAttribute;
            cutomAttributes.Add(GenerateCreateAssetMenuAttribute(CreateAssetMenuAttribute.fileName, CreateAssetMenuAttribute.menuName));
        }
        return cutomAttributes;
    }

    public static string FormatDictionaryToCodeSnippet(Dictionary<string, string> dic)
    {
        string snippetString = "{\n";
        foreach (var dicEntry in dic)
        {
            snippetString += "{" + dicEntry.Key + "," + dicEntry.Value + "},\n";
        }
        snippetString += "}";
        return snippetString;
    }
}

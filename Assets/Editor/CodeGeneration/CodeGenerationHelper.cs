using Editor_GameDesigner;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        if (baseType != null && baseType != typeof(object))
        {
            targetClass.BaseTypes.Add(baseType);
        }

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
        else if (sourceCustomAttribute.GetType() == typeof(CustomEnum))
        {
            var CustomEnumAttribute = (CustomEnum)sourceCustomAttribute;
            cutomAttributes.Add(new CodeAttributeDeclaration(typeof(CustomEnum).FullName, new CodeAttributeArgument[] {
                new CodeAttributeArgument("IsSearchable", new CodePrimitiveExpression(CustomEnumAttribute.IsSearchable)),
                new CodeAttributeArgument("IsCreateable", new CodePrimitiveExpression(CustomEnumAttribute.IsCreateable)),
                new CodeAttributeArgument("ChoosedOpenRepertoire", new CodePrimitiveExpression(CustomEnumAttribute.ChoosedOpenRepertoire))
            }));

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

    public static string ApplyStringParameters(string sourceString, Dictionary<string, string> parameters)
    {
        foreach (var parameter in parameters)
        {
            sourceString = sourceString.Replace(parameter.Key, parameter.Value);
        }
        return sourceString;
    }

    public static ClassFile ClassFileFromType(Type sourceType)
    {
        //using dependencies by file search
        var sourceClassFiles = Directory.GetFiles(Application.dataPath, sourceType.Name + ".cs", SearchOption.AllDirectories);
        string sourceClassFile = string.Empty;
        string sourceClassPath = string.Empty;
        if (sourceClassFiles != null && sourceClassFiles.Length > 0)
        {
            Debug.Log(sourceClassFiles[0]);
            using (StreamReader sr = new StreamReader(sourceClassFiles[0]))
            {
                sourceClassFile = sr.ReadToEnd();
            }
            sourceClassPath = sourceClassFiles[0];
        }
        return new ClassFile(sourceClassFile, sourceClassPath);
    }

    public class ClassFile
    {
        public string Content;
        public string Path;

        public ClassFile(string content, string path)
        {
            Content = content;
            Path = path;
        }
    }

    public static void AddGameDesignerChoiceTree(List<KeyValuePair<string, string>> entries)
    {
        string GameDesignerChoiceTreeConstantPath = "Assets/Editor/GameDesigner/ChoiceTree";
        CodeCompileUnit compileUnity = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace(typeof(ChoiceTreeConstant).Namespace);
        var generatedChoiceTreeConstant = new CodeTypeDeclaration(typeof(ChoiceTreeConstant).Name);

        var generatedChoiceModuleFieldName = nameof(ChoiceTreeConstant.Modules);
        var generatedChoiceModuleField = new CodeMemberField(typeof(ChoiceTreeConstant).GetField(generatedChoiceModuleFieldName).FieldType, generatedChoiceModuleFieldName);
        generatedChoiceModuleField.Attributes = MemberAttributes.Public | MemberAttributes.Static;
        var generatedChoiceModuleDic = ChoiceTreeConstant.Modules.ToList()
            .ConvertAll(kv => new KeyValuePair<string, string>("\"" + kv.Key + "\"", "typeof(" + kv.Value.FullName + ")"))
              .Union(entries.ConvertAll(kv => new KeyValuePair<string, string>("\"" + kv.Key + "\"", "typeof(" + kv.Value + ")")))
            .GroupBy(kv => kv.Key)
            .ToDictionary(kv => kv.Key, kv => kv.First().Value);
        generatedChoiceModuleField.InitExpression = new CodeSnippetExpression("new System.Collections.Generic.Dictionary<string, System.Type>()" +
            CodeGenerationHelper.FormatDictionaryToCodeSnippet(generatedChoiceModuleDic));
        generatedChoiceTreeConstant.Members.Add(generatedChoiceModuleField);


        samples.Types.Add(generatedChoiceTreeConstant);
        compileUnity.Namespaces.Add(samples);

        string filename = GameDesignerChoiceTreeConstantPath + "/" + generatedChoiceTreeConstant.Name + ".cs";
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        using (StreamWriter sourceWriter = new StreamWriter(filename))
        {
            provider.GenerateCodeFromCompileUnit(
                compileUnity, sourceWriter, options);
        }
    }
    
    public static void CopyFile(DirectoryInfo targetDirectory, Dictionary<string, string> parameters, FileInfo fileToCopy)
    {
        if (fileToCopy.Extension == ".txt")
        {
            var sourceText = fileToCopy.OpenText().ReadToEnd();
            var targetText = CodeGenerationHelper.ApplyStringParameters(sourceText, parameters);
            var targetFileName = CodeGenerationHelper.ApplyStringParameters(fileToCopy.Name, parameters).Replace(".txt", "");

            Debug.Log(targetDirectory.FullName + "/" + targetFileName);
            var fileToCreate = new FileInfo(targetDirectory.FullName + "/" + targetFileName);

            using (StreamWriter sw = fileToCreate.CreateText())
            {
                sw.Write(targetText);
            }
        }
    }
}

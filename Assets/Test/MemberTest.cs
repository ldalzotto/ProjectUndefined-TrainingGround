using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class MemberTest
{

    [MenuItem("Test/Do")]
    public static void Do()
    {
        var testClass = new TestClass(5, 10);
        foreach (var fields in GetAllFields(testClass.GetType()))
        {
            Debug.Log(fields.Name);
            Debug.Log(fields.GetValue(testClass));
            Debug.Log(fields.GetCustomAttribute<MyCustomProperty>() != null);
            var attribues = fields.GetCustomAttributes().ToArray();
        }
    }

    public static IEnumerable<FieldInfo> GetAllFields(Type t)
    {
        if (t == null)
            return Enumerable.Empty<FieldInfo>();

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                             BindingFlags.Static | BindingFlags.Instance |
                             BindingFlags.DeclaredOnly;
        return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
    }
}

class ATestClass
{
    [MyCustomProperty]
    private int baseMyProp;
    public int BaseMyProp { get => baseMyProp; set => baseMyProp = value; }

    public ATestClass(int baseMyProp)
    {
        BaseMyProp = baseMyProp;
    }

}

class TestClass : ATestClass
{
    public int ChildMyProp { get; private set; }

    public TestClass(int childMyProp, int baseMyProp) : base(baseMyProp)
    {
        this.ChildMyProp = childMyProp;
    }
}

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public class MyCustomProperty : PropertyAttribute
{

}
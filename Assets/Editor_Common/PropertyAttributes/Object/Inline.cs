using UnityEngine;
using System.Collections;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class Inline : PropertyAttribute
{
    public bool CreateSubIfAbsent = false;
    public string FileName;

    public Inline(bool createSubIfAbsent = false)
    {
        CreateSubIfAbsent = createSubIfAbsent;
    }
}

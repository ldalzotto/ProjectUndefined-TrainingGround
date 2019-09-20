using UnityEngine;
using System.Collections;

[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
public class ModuleMetadata : PropertyAttribute
{
    private string header;

    public ModuleMetadata(string header)
    {
        this.header = header;
    }

    public string Header { get => header;  }
}

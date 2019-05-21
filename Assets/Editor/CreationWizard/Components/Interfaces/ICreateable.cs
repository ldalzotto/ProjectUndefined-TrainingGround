using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICreateable 
{
    void InstanciateInEditor(ref Dictionary<string, CreationModuleComponent> editorModules);
}

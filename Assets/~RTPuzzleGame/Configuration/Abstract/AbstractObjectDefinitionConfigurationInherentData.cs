﻿using OdinSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class AbstractObjectDefinitionConfigurationInherentData : SerializedScriptableObject
{
    [SerializeField]
    [FormerlySerializedAs("RangeDefinitionModulesActivation")]
    public Dictionary<Type, bool> RangeDefinitionModulesActivation;
    [SerializeField]
    [FormerlySerializedAs("RangeDefinitionModules")]
    public Dictionary<Type, ScriptableObject> RangeDefinitionModules;

    public abstract List<Type> ModuleTypes { get; }

    protected void DestroyExistingModules(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            bool destroy = false;
            foreach (var component in child.GetComponents<Component>())
            {
                if (ModuleTypes.Contains(component.GetType()))
                {
                    destroy = true;
                    break;
                }
            }
            if (destroy)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

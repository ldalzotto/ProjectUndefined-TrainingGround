﻿using System.Collections.Generic;
using UnityEngine;

public static class ComponentSearchExtensions
{
    public static GameObject FindChildObjectRecursively(this GameObject gameObject, string name)
    {
        foreach (Transform childTransform in gameObject.transform)
        {
            if (childTransform.name == name)
            {
                return childTransform.gameObject;
            }
            else
            {
                if (childTransform.childCount > 0)
                {
                    var recursiveResult = childTransform.gameObject.FindChildObjectRecursively(name);
                    if (recursiveResult != null)
                    {
                        return recursiveResult;
                    }
                }
            }
        }
        return null;
    }

    public static List<GameObject> FindOneLevelDownChilds(this GameObject gameObject)
    {
        List<GameObject> returnValue = new List<GameObject>();
        foreach (Transform childTransform in gameObject.transform)
        {
            returnValue.Add(childTransform.gameObject);
        }
        return returnValue;
    }

}

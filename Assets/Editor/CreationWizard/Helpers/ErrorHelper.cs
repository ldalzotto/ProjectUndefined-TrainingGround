﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace CreationWizard
{
    public class ErrorHelper
    {
        public static string NonNullity(UnityEngine.Object obj, string fieldName)
        {
            if (obj == null)
            {
                return ErrorMessages.NonNullityErrorMessage(fieldName);
            }
            return string.Empty;
        }

        public static string NonNullity(string obj, string fieldName)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return ErrorMessages.NonNullityErrorMessage(fieldName);
            }
            return string.Empty;
        }

        public static string NotAlreadyPresentInConfiguration(Enum key, List<Enum> keys, string configurationName)
        {
            if (keys.Contains(key))
            {
                return ErrorMessages.GetConfigurationOverriteMessage(key, configurationName);
            }
            return string.Empty;
        }

        public static string ModuleIgnoredIfIsNew(bool isNew, string ignoredModuleName, string comparedModuleName)
        {
            if (!isNew)
            {
                return ErrorMessages.ModuleIgnoredMessage(ignoredModuleName, "the module " + comparedModuleName + " is not new");
            }
            return string.Empty;
        }
    }

}

using UnityEngine;
using System.Collections;
using System;

namespace CreationWizard
{
    public class ErrorMessages 
    {

        public static string GetConfigurationOverriteMessage(Enum key, string configurationName)
        {
          return  "On generation, the key " + key + " of " + configurationName + " will be overriten.";
        }

        public static string NonNullityErrorMessage(string fieldName)
        {
            return "The field : " + fieldName + " must not be null.";
        }

        public static string ModuleIgnoredMessage(string ignoredModuleName, string reason)
        {
            return "The module : " + ignoredModuleName + " is ignored because " + reason + ".";
        }
    }
    
}

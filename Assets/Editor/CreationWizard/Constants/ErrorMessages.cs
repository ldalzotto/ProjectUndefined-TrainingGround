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
    }
    
}

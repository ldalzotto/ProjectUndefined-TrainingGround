using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class InteractiveObjectModuleWizard : BaseModuleWizardModule<InteractiveObjectType, InteractiveObjectModule>
    {
        protected override List<InteractiveObjectModule> GetModules(InteractiveObjectType RootModuleObject)
        {
            return RootModuleObject.GetComponentsInChildren<InteractiveObjectModule>().ToList();
        }

        protected override void OnEdit(InteractiveObjectType InteractiveObjectType, Type selectedType)
        {
            this.InteractiveObjectModuleSwitch(selectedType,
               InteractiveObjectModuleAction: () =>
               {
                   if (this.add)
                   {
                       EditorInteractiveObjectModulesOperation.AddPrefabModule(InteractiveObjectType, this.CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseModelObjectModule);
                   }
                   else if (this.remove)
                   {
                       EditorInteractiveObjectModulesOperation.RemovePrefabModule<ModelObjectModule>(InteractiveObjectType);
                   }
               }
             );
        }

        protected override string POIModuleDescription(Type selectedType)
        {
            string returnDescription = string.Empty;
            this.InteractiveObjectModuleSwitch(selectedType,
                    InteractiveObjectModuleAction: () => { returnDescription = "Model object definition."; }
                );
            return returnDescription;
        }

        private void InteractiveObjectModuleSwitch(Type selectedType, Action InteractiveObjectModuleAction)
        {
            if (selectedType == typeof(ModelObjectModule))
            {
                InteractiveObjectModuleAction.Invoke();
            }
        }
    }
}
